<script lang="ts">
  import TabManager from "$lib/components/admin/TabManager.svelte";
</script>

<svelte:head>
  <title>Admin — MiniLeet</title>
</svelte:head>

<div class="h-screen flex flex-col overflow-hidden bg-white">
  <!-- Top bar -->
  <header class="border-b border-gray-200 px-4 py-2 flex items-center justify-between shrink-0">
    <div class="flex items-center gap-2 text-sm">
      <a href="/" class="font-bold text-gray-900 hover:text-orange-500 transition-colors">MiniLeet</a>
      <span class="text-gray-300">/</span>
      <span class="text-gray-500 font-medium">Admin</span>
    </div>
    <div class="flex items-center gap-3">
      <a href="/problems" class="text-xs text-gray-400 hover:text-gray-600 transition-colors">← Problems</a>
    </div>
  </header>

  <!-- TabManager fills remaining height -->
  <div class="flex-1 overflow-hidden">
    <TabManager />
  </div>
</div>