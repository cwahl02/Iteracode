<script lang="ts">
  import { vfs } from "$lib/stores/vfs.svelte";
  import { explorer } from "$lib/stores/explorer.svelte";
  import type { VFSNode } from "$lib/types/admin";
  import TreeNode from "./TreeNode.svelte";
  import InlineInput from "./InlineInput.svelte";

  let {
    onopenfile,
  }: {
    onopenfile: (path: string, label: string) => void;
  } = $props();

  // new item state lives here since it's UI-only
  let newItemState = $state<{ parentPath: string; type: "file" | "folder" } | null>(null);
  let newItemName  = $state("");

  function startNewItem(parentPath: string, type: "file" | "folder") {
    const node = vfs.getNode(parentPath);
    if (node?.type === "folder" && !node.expanded) vfs.toggleFolder(parentPath);
    newItemState = { parentPath, type };
    newItemName  = "";
  }

  function commitNewItem() {
    if (!newItemState || !newItemName.trim()) { newItemState = null; return; }
    if (newItemState.type === "folder") {
      vfs.createFolder(newItemState.parentPath, newItemName.trim());
    } else {
      vfs.createFile(newItemState.parentPath, newItemName.trim());
    }
    newItemState = null;
    newItemName  = "";
  }

  // Recursive snippet: renders a node + its new item input if applicable
  // We need to thread newItemState down for inline rendering after children
</script>

<div class="h-full flex flex-col bg-gray-50">
  <!-- Header -->
  <div class="flex items-center justify-between px-3 py-2 border-b border-gray-200 shrink-0">
    <span class="text-xs font-semibold text-gray-500 uppercase tracking-wider">Files</span>
    <div class="flex gap-0.5">
      <button
        onclick={() => startNewItem(explorer.focusedFolder.path, "folder")}
        class="p-1 rounded hover:bg-gray-200 text-gray-400 hover:text-gray-700 transition-colors"
        title="New folder"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 13h6m-3-3v6m-9 1V7a2 2 0 012-2h6l2 2h6a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
        </svg>
      </button>
      <button
        onclick={() => startNewItem(explorer.focusedFolder.path, "file")}
        class="p-1 rounded hover:bg-gray-200 text-gray-400 hover:text-gray-700 transition-colors"
        title="New file"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
      </button>
    </div>
  </div>

  <!-- Tree -->
  <div class="flex-1 overflow-y-auto py-1">

    {#snippet renderTree(nodes: VFSNode[], depth: number, parentPath: string)}
      {#each nodes as node (node.path)}
        <TreeNode
          {node}
          {depth}
          onopennewitem={startNewItem}
          {onopenfile}
        />

        <!-- Inline new item under this folder's children -->
        {#if node.type === "folder" && node.expanded && newItemState?.parentPath === node.path}
          <div
            class="flex items-center gap-1 py-0.5"
            style="padding-left: {8 + (depth + 1) * 12}px"
          >
            <span class="text-xs">{newItemState.type === "folder" ? "📁" : "📄"}</span>
            <InlineInput
              bind:value={newItemName}
              placeholder={newItemState.type === "folder" ? "folder-name" : "file.py"}
              oncommit={commitNewItem}
              oncancel={() => newItemState = null}
            />
          </div>
        {/if}

        <!-- Recurse into expanded folders -->
        {#if node.type === "folder" && node.expanded}
          {@render renderTree(node.children, depth + 1, node.path)}
        {/if}
      {/each}
    {/snippet}

    <!-- Root level new item -->
    {#if newItemState?.parentPath === "root"}
      <div class="flex items-center gap-1 py-0.5 px-2">
        <span class="text-xs">{newItemState.type === "folder" ? "📁" : "📄"}</span>
        <InlineInput
          bind:value={newItemName}
          placeholder={newItemState.type === "folder" ? "folder-name" : "file.py"}
          oncommit={commitNewItem}
          oncancel={() => newItemState = null}
        />
      </div>
    {/if}

    {@render renderTree(vfs.root.children, 0, "root")}
  </div>
</div>