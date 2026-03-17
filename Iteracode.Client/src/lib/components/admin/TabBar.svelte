<script lang="ts">
  import type { Tab } from "$lib/types/admin";
  import WindowTab from "./WindowTab.svelte";
  import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
  import { Button } from "$lib/components/ui/button";

  let {
    tabs,
    activeId,
    onfocus,
    onclose,
    onnewexplorer,
    onneweditor,
  }: {
    tabs: Tab[];
    activeId: string;
    onfocus: (id: string) => void;
    onclose: (id: string) => void;
    onnewexplorer: () => void;
    onneweditor: () => void;
  } = $props();
</script>

<div class="flex items-stretch bg-gray-100 border-b border-gray-200 overflow-x-auto shrink-0 h-9" style="scrollbar-width: none;">
  {#each tabs as tab (tab.id)}
    <WindowTab
      {tab}
      active={tab.id === activeId}
      {onfocus}
      {onclose}
    />
  {/each}

  <!-- New tab dropdown -->
  <div class="flex items-center px-1 sticky right-0 bg-gray-100 border-l border-gray-200 shrink-0">
    <DropdownMenu.Root>
      <DropdownMenu.Trigger>
        <Button
          variant="ghost"
          size="sm"
          class="h-6 w-6 p-0 text-gray-400 hover:text-gray-700 hover:bg-gray-200"
          title="New tab"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
          </svg>
        </Button>
      </DropdownMenu.Trigger>
      <DropdownMenu.Content align="end" class="w-44">
        <DropdownMenu.Item onclick={onnewexplorer} class="text-xs gap-2">
          <span>🗂</span> New Explorer
        </DropdownMenu.Item>
        <DropdownMenu.Item onclick={onneweditor} class="text-xs gap-2">
          <span>📄</span> New Editor
        </DropdownMenu.Item>
      </DropdownMenu.Content>
    </DropdownMenu.Root>
  </div>
</div>