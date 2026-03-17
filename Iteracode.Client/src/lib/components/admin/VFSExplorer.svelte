<script lang="ts">
  import FileTree from "./explorer/FileTree.svelte";
  import FolderView from "./explorer/FolderView.svelte";

  let {
    onopenfile,
  }: {
    onopenfile: (path: string, label: string) => void;
  } = $props();
</script>

<div class="h-full flex overflow-hidden text-xs select-none">
  <div class="w-56 shrink-0 border-r border-gray-200">
    <FileTree {onopenfile} />
  </div>
  <FolderView {onopenfile} />
</div>