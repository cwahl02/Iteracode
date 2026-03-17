<script lang="ts">
  import { explorer } from "$lib/stores/explorer.svelte";
  import * as ContextMenu from "$lib/components/ui/context-menu";
  import { Button } from "$lib/components/ui/button";
  import FolderViewItem from "./FolderViewItem.svelte";
  import InlineInput from "./InlineInput.svelte";

  let {
    onopenfile,
  }: {
    onopenfile: (path: string, label: string) => void;
  } = $props();

  let newItemState = $state<{ parentPath: string; type: "file" | "folder" } | null>(null);
  let newItemName  = $state("");
  let folderViewHover = $state(false);

  import { vfs } from "$lib/stores/vfs.svelte";

  function startNewItem(parentPath: string, type: "file" | "folder") {
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

  const breadcrumbs = $derived((() => {
    const parts = explorer.focusedFolder.path.split("/");
    return parts.map((name, i) => ({
      name,
      path: parts.slice(0, i + 1).join("/"),
    }));
  })());

  function handleFolderViewDrop(e: DragEvent) {
    e.preventDefault();
    folderViewHover = false;
    if (explorer.isOsFiles(e)) {
      explorer.readAndUpload(e.dataTransfer?.files ?? null, explorer.focusedFolder.path);
      return;
    }
    const src = explorer.dragSourcePath;
    if (!src || src === explorer.focusedFolder.path) return;
    if (explorer.focusedFolder.path.startsWith(src + "/")) return;
    vfs.moveNode(src, explorer.focusedFolder.path);
  }
</script>

<div class="flex-1 flex flex-col overflow-hidden bg-white">

  <!-- Toolbar -->
  <div class="flex items-center gap-2 px-3 py-2 border-b border-gray-200 bg-gray-50 shrink-0">
    <!-- Breadcrumb -->
    <div class="flex items-center gap-1 flex-1 min-w-0 flex-wrap">
      {#each breadcrumbs as crumb, i}
        <button
          onclick={() => explorer.navigateTo(crumb.path)}
          class="text-xs transition-colors truncate
            {i === breadcrumbs.length - 1
              ? 'text-gray-800 font-semibold'
              : 'text-gray-400 hover:text-orange-500'}"
        >{crumb.name}</button>
        {#if i < breadcrumbs.length - 1}
          <span class="text-gray-300 shrink-0 text-xs">/</span>
        {/if}
      {/each}
    </div>

    <Button
      variant="outline"
      size="sm"
      class="h-6 text-xs gap-1 shrink-0"
      onclick={() => explorer.triggerUpload(explorer.focusedFolder.path)}
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a2 2 0 002 2h12a2 2 0 002-2v-1M8 12l4-4m0 0l4 4m-4-4v12" />
      </svg>
      Upload
    </Button>

    <button
      onclick={() => startNewItem(explorer.focusedFolder.path, "folder")}
      class="p-1 rounded hover:bg-gray-200 text-gray-400 hover:text-gray-700 transition-colors shrink-0"
      title="New folder here"
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 13h6m-3-3v6m-9 1V7a2 2 0 012-2h6l2 2h6a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
      </svg>
    </button>

    <button
      onclick={() => startNewItem(explorer.focusedFolder.path, "file")}
      class="p-1 rounded hover:bg-gray-200 text-gray-400 hover:text-gray-700 transition-colors shrink-0"
      title="New file here"
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
      </svg>
    </button>
  </div>

  <!-- Content -->
  <ContextMenu.Root>
    <ContextMenu.Trigger class="flex-1 overflow-hidden block">
      <div
        class="h-full overflow-y-auto transition-colors
          {folderViewHover ? 'bg-orange-50 ring-1 ring-inset ring-orange-300' : 'bg-white'}"
        ondragover={(e) => { e.preventDefault(); folderViewHover = true; }}
        ondragleave={(e) => {
          if (!e.currentTarget.contains(e.relatedTarget as Node)) folderViewHover = false;
        }}
        ondrop={handleFolderViewDrop}
      >
        {#if explorer.focusedFolder.children.length === 0 && !newItemState}
          <div class="flex flex-col items-center justify-center h-full gap-3 text-gray-300 pointer-events-none">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v9a2 2 0 01-2 2H5a2 2 0 01-2-2V7z" />
            </svg>
            <p class="text-xs">Empty — drop files here or use Upload</p>
          </div>

        {:else}
          <!-- Column headers -->
          <div class="grid grid-cols-[1fr_80px] gap-2 px-4 py-1.5 border-b border-gray-100
            text-xs text-gray-400 font-medium sticky top-0 bg-white z-10">
            <span>Name</span>
            <span>Type</span>
          </div>

          {#each explorer.focusedFolder.children as node (node.path)}
            <FolderViewItem {node} {onopenfile} onopennewitem={startNewItem} />
          {/each}

          <!-- Inline new item -->
          {#if newItemState?.parentPath === explorer.focusedFolder.path}
            <div class="grid grid-cols-[1fr_80px] gap-2 px-4 py-1.5 items-center">
              <div class="flex items-center gap-2">
                <span class="text-sm">{newItemState.type === "folder" ? "📁" : "📄"}</span>
                <InlineInput
                  bind:value={newItemName}
                  placeholder={newItemState.type === "folder" ? "folder-name" : "file.py"}
                  oncommit={commitNewItem}
                  oncancel={() => newItemState = null}
                />
              </div>
              <span class="text-xs text-gray-300">
                {newItemState.type === "folder" ? "Folder" : "File"}
              </span>
            </div>
          {/if}
        {/if}
      </div>
    </ContextMenu.Trigger>

    <ContextMenu.Content class="w-44">
      <ContextMenu.Item class="text-xs font-medium text-gray-400 pointer-events-none">
        {explorer.focusedFolder.name}/
      </ContextMenu.Item>
      <ContextMenu.Separator />
      <ContextMenu.Item class="text-xs" onclick={() => startNewItem(explorer.focusedFolder.path, "file")}>New File</ContextMenu.Item>
      <ContextMenu.Item class="text-xs" onclick={() => startNewItem(explorer.focusedFolder.path, "folder")}>New Folder</ContextMenu.Item>
      <ContextMenu.Separator />
      <ContextMenu.Item class="text-xs" onclick={() => explorer.triggerUpload(explorer.focusedFolder.path)}>Upload Files</ContextMenu.Item>
    </ContextMenu.Content>
  </ContextMenu.Root>
</div>