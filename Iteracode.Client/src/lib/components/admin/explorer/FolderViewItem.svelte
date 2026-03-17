<script lang="ts">
  import type { VFSNode, VFSFolder } from "$lib/types/admin";
  import { vfs } from "$lib/stores/vfs.svelte";
  import { explorer } from "$lib/stores/explorer.svelte";
  import * as ContextMenu from "$lib/components/ui/context-menu";
  import InlineInput from "./InlineInput.svelte";

  let {
    node,
    onopenfile,
    onopennewitem,
  }: {
    node: VFSNode;
    onopenfile: (path: string, label: string) => void;
    onopennewitem: (parentPath: string, type: "file" | "folder") => void;
  } = $props();

  const isSelected = $derived(explorer.selectedPath === node.path);
  const isRenaming = $derived(explorer.renamingPath === node.path);

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

  function handleDragOver(e: DragEvent) {
    if (node.type !== "folder") return;
    e.preventDefault();
    e.stopPropagation();
    explorer.treeHoverPath = node.path;
  }

  function handleDrop(e: DragEvent) {
    if (node.type !== "folder") return;
    explorer.dropOnFolder(e, node as VFSFolder);
  }
</script>

<ContextMenu.Root>
  <ContextMenu.Trigger class="block w-full">
    <div
      class="grid grid-cols-[1fr_80px] gap-2 px-4 py-1.5 items-center
        hover:bg-gray-50 cursor-pointer transition-colors
        {isSelected ? 'bg-orange-50' : ''}
        {explorer.treeHoverPath === node.path && node.type === 'folder' ? 'ring-1 ring-inset ring-orange-300' : ''}"
      onclick={handleClick}
      draggable="true"
      ondragstart={(e) => explorer.startDrag(e, node.path)}
      ondragend={explorer.endDrag}
      ondragover={handleDragOver}
      ondragleave={() => explorer.treeHoverPath = null}
      ondrop={handleDrop}
      role="button"
      tabindex="0"
      onkeydown={(e) => e.key === "Enter" && handleClick()}
    >
      <div class="flex items-center gap-2 min-w-0">
        <span class="shrink-0 text-sm">
          {node.type === "folder" ? "📁" : langIcon(node.name)}
        </span>

        {#if isRenaming}
          <InlineInput
            bind:value={explorer.renameValue}
            oncommit={() => explorer.commitRename(node.path)}
            oncancel={explorer.cancelRename}
          />
        {:else}
          <span class="truncate text-xs text-gray-700">{node.name}</span>
        {/if}
      </div>

      <span class="text-xs text-gray-400">
        {node.type === "folder"
          ? "Folder"
          : (node.name.split(".").pop()?.toUpperCase() ?? "File")}
      </span>
    </div>
  </ContextMenu.Trigger>

  <ContextMenu.Content class="w-44">
    {#if node.type === "folder"}
      <ContextMenu.Item class="text-xs" onclick={() => explorer.focusNode(node as VFSFolder)}>Open</ContextMenu.Item>
      <ContextMenu.Separator />
      <ContextMenu.Item class="text-xs" onclick={() => onopennewitem(node.path, "file")}>New File Inside</ContextMenu.Item>
      <ContextMenu.Item class="text-xs" onclick={() => onopennewitem(node.path, "folder")}>New Folder Inside</ContextMenu.Item>
    {:else}
      <ContextMenu.Item class="text-xs" onclick={() => onopenfile(node.path, node.name)}>Open</ContextMenu.Item>
    {/if}
    <ContextMenu.Separator />
    <ContextMenu.Item class="text-xs" onclick={() => explorer.startRename(node)}>Rename</ContextMenu.Item>
    <ContextMenu.Separator />
    <ContextMenu.Item class="text-xs text-red-600" onclick={() => vfs.deleteNode(node.path)}>Delete</ContextMenu.Item>
  </ContextMenu.Content>
</ContextMenu.Root>