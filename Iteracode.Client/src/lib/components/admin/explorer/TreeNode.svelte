<script lang="ts">
  import type { VFSNode, VFSFolder } from "$lib/types/admin";
  import { vfs } from "$lib/stores/vfs.svelte";
  import { explorer } from "$lib/stores/explorer.svelte";
  import { isProtected, isRecyclable, canRename, canMove, isProblemJson } from "$lib/vfs/protected";
  import * as ContextMenu from "$lib/components/ui/context-menu";
  import InlineInput from "./InlineInput.svelte";
  import DeleteConfirm from "./DeleteConfirm.svelte";

  let {
    node,
    depth,
    onopennewitem,
    onopenfile,
  }: {
    node: VFSNode;
    depth: number;
    onopennewitem: (parentPath: string, type: "file" | "folder") => void;
    onopenfile: (path: string, label: string) => void;
  } = $props();

  const isSelected = $derived(explorer.selectedPath === node.path);
  const isRenaming = $derived(explorer.renamingPath === node.path);
  const isDragOver = $derived(
    explorer.treeHoverPath === node.path && !!explorer.dragSourcePath
  );
  const protected_  = $derived(isProtected(node.path));
  const recyclable  = $derived(isRecyclable(node.path));
  const renameable  = $derived(canRename(node.path));
  const moveable    = $derived(canMove(node.path));
  const isJson      = $derived(isProblemJson(node.path));

  const indent = $derived(`padding-left: ${8 + depth * 12}px; padding-right: 8px`);

  // delete confirm state
  let confirmOpen      = $state(false);
  let confirmMode      = $state<"confirm" | "named">("confirm");
  let pendingDeletePath = $state<string | null>(null);

  function requestDelete(path: string, permanent = false) {
    pendingDeletePath = path;
    confirmMode       = permanent ? "named" : "confirm";
    confirmOpen       = true;
  }

  function executeDelete() {
    if (!pendingDeletePath) return;
    if (confirmMode === "named") {
      vfs.deleteNodePermanent(pendingDeletePath);
    } else {
      // file or problem.json → triggers recycle of parent if json
      vfs.deleteNode(pendingDeletePath);
    }
    pendingDeletePath = null;
  }

  function langIcon(name: string) {
    if (name.endsWith(".json")) return "📋";
    if (name.endsWith(".py"))   return "🐍";
    if (name.endsWith(".sh"))   return "⚙";
    if (name.endsWith(".java")) return "☕";
    if (name.endsWith(".cpp") || name.endsWith(".c")) return "⚡";
    return "📄";
  }

  let clickTimer: ReturnType<typeof setTimeout> | null = null;

  function handleClick() {
    if (isRenaming) return;
    if (node.type === "folder") {
      vfs.toggleFolder(node.path);
      explorer.focusNode(node as VFSFolder);
      return;
    }
    if (explorer.selectedPath === node.path) {
      if (clickTimer) {
        clearTimeout(clickTimer);
        clickTimer = null;
        onopenfile(node.path, node.name);
      } else {
        clickTimer = setTimeout(() => { clickTimer = null; }, 400);
      }
    } else {
      explorer.selectedPath = node.path;
      if (clickTimer) { clearTimeout(clickTimer); clickTimer = null; }
      clickTimer = setTimeout(() => { clickTimer = null; }, 400);
    }
  }
</script>

<!-- Delete confirm dialog -->
<DeleteConfirm
  bind:open={confirmOpen}
  mode={confirmMode}
  itemName={node.name}
  onconfirm={executeDelete}
  oncancel={() => pendingDeletePath = null}
/>

{#if node.type === "folder"}
  <ContextMenu.Root>
    <ContextMenu.Trigger class="block w-full">
      <div
        class="flex items-center gap-1 py-0.5 rounded cursor-pointer
          hover:bg-gray-200 transition-colors
          {isSelected ? 'bg-orange-50 text-orange-700' : 'text-gray-700'}
          {isDragOver ? 'ring-1 ring-orange-400 bg-orange-50' : ''}"
        style={indent}
        onclick={handleClick}
        draggable={moveable}
        ondragstart={moveable ? (e) => explorer.startDrag(e, node.path) : undefined}
        ondragend={explorer.endDrag}
        ondragover={(e) => { e.preventDefault(); explorer.treeHoverPath = node.path; }}
        ondragleave={() => explorer.treeHoverPath = null}
        ondrop={(e) => explorer.dropOnFolder(e, node as VFSFolder)}
        role="button"
        tabindex="0"
        onkeydown={(e) => e.key === "Enter" && handleClick()}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-3 w-3 text-gray-400 transition-transform shrink-0
            {(node as VFSFolder).expanded ? 'rotate-90' : ''}"
          viewBox="0 0 20 20" fill="currentColor"
        >
          <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
        </svg>
        <span class="shrink-0">{(node as VFSFolder).expanded ? "📂" : "📁"}</span>

        {#if isRenaming}
          <InlineInput
            bind:value={explorer.renameValue}
            oncommit={() => explorer.commitRename(node.path)}
            oncancel={explorer.cancelRename}
          />
        {:else}
          <span class="truncate text-xs">
            {node.name}
            {#if protected_}
              <span class="text-gray-300 ml-0.5">🔒</span>
            {/if}
          </span>
        {/if}
      </div>
    </ContextMenu.Trigger>

    <ContextMenu.Content class="w-48">
      <ContextMenu.Item class="text-xs" onclick={() => explorer.focusNode(node as VFSFolder)}>
        Focus Folder
      </ContextMenu.Item>

      {#if !protected_}
        <ContextMenu.Separator />
        <ContextMenu.Item class="text-xs" onclick={() => onopennewitem(node.path, "file")}>New File</ContextMenu.Item>
        <ContextMenu.Item class="text-xs" onclick={() => onopennewitem(node.path, "folder")}>New Folder</ContextMenu.Item>
      {/if}

      {#if renameable}
        <ContextMenu.Separator />
        <ContextMenu.Item class="text-xs" onclick={() => explorer.startRename(node)}>Rename</ContextMenu.Item>
      {/if}

      {#if !protected_}
        <ContextMenu.Separator />
        {#if recyclable}
          <ContextMenu.Item class="text-xs text-orange-600" onclick={() => requestDelete(node.path, false)}>
            Move to Recycle Bin
          </ContextMenu.Item>
        {/if}
        <ContextMenu.Item class="text-xs text-red-600" onclick={() => requestDelete(node.path, true)}>
          Permanently Delete
        </ContextMenu.Item>
      {/if}
    </ContextMenu.Content>
  </ContextMenu.Root>

{:else}
  <ContextMenu.Root>
    <ContextMenu.Trigger class="block w-full">
      <div
        class="flex items-center gap-1.5 py-0.5 rounded cursor-pointer
          hover:bg-gray-200 transition-colors
          {isSelected ? 'bg-orange-50 text-orange-700' : 'text-gray-700'}"
        style={indent}
        onclick={handleClick}
        draggable={moveable}
        ondragstart={moveable ? (e) => explorer.startDrag(e, node.path) : undefined}
        ondragend={explorer.endDrag}
        role="button"
        tabindex="0"
        onkeydown={(e) => e.key === "Enter" && handleClick()}
      >
        <span class="shrink-0 text-xs">{langIcon(node.name)}</span>

        {#if isRenaming}
          <InlineInput
            bind:value={explorer.renameValue}
            oncommit={() => explorer.commitRename(node.path)}
            oncancel={explorer.cancelRename}
          />
        {:else}
          <span class="truncate text-xs">{node.name}</span>
        {/if}
      </div>
    </ContextMenu.Trigger>

    <ContextMenu.Content class="w-48">
      <ContextMenu.Item class="text-xs" onclick={() => onopenfile(node.path, node.name)}>Open</ContextMenu.Item>

      {#if renameable}
        <ContextMenu.Separator />
        <ContextMenu.Item class="text-xs" onclick={() => explorer.startRename(node)}>Rename</ContextMenu.Item>
      {/if}

      <ContextMenu.Separator />
      <!-- Files always hard delete with simple confirm -->
      <ContextMenu.Item class="text-xs text-red-600" onclick={() => requestDelete(node.path, false)}>
        {isJson ? "Delete Problem" : "Delete"}
      </ContextMenu.Item>
    </ContextMenu.Content>
  </ContextMenu.Root>
{/if}