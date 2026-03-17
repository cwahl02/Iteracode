<script lang="ts">
  import type { Tab } from "$lib/types/admin";

  let {
    tab,
    active = false,
    onclose,
    onfocus,
  }: {
    tab: Tab;
    active?: boolean;
    onclose?: (id: string) => void;
    onfocus?: (id: string) => void;
  } = $props();

  const unsaved = $derived(
    tab.type === "editor" && tab.content !== tab.savedContent
  );

  const icon = $derived(
    tab.type === "explorer"
      ? "🗂"
      : tab.label.endsWith(".json")  ? "📋"
      : tab.label.endsWith(".py")    ? "🐍"
      : tab.label.endsWith(".sh")    ? "⚙"
      : tab.label.endsWith(".java")  ? "☕"
      : "📄"
  );
</script>

<button
  onclick={() => onfocus?.(tab.id)}
  class="group relative flex items-center gap-1.5 px-3 py-1.5 text-xs border-r border-gray-200
    whitespace-nowrap select-none transition-colors shrink-0
    {active
      ? 'bg-white text-gray-800 border-t-2 border-t-orange-500'
      : 'bg-gray-100 text-gray-500 hover:bg-gray-50 hover:text-gray-700 border-t-2 border-t-transparent'}"
>
  <span class="text-sm leading-none">{icon}</span>
  <span class="max-w-[120px] truncate">{tab.label}</span>

  {#if unsaved}
    <span class="w-1.5 h-1.5 rounded-full bg-orange-400 shrink-0" title="Unsaved changes"></span>
  {/if}

  {#if !tab.pinned}
    <button
      onclick={(e) => { e.stopPropagation(); onclose?.(tab.id); }}
      class="ml-0.5 rounded p-0.5 opacity-0 group-hover:opacity-100 hover:bg-gray-200
        transition-opacity shrink-0 text-gray-400 hover:text-gray-700"
      title="Close"
      aria-label="Close tab"
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" viewBox="0 0 20 20" fill="currentColor">
        <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
      </svg>
    </button>
  {:else}
    <!-- pinned spacer to keep consistent width -->
    <span class="w-4 shrink-0"></span>
  {/if}
</button>