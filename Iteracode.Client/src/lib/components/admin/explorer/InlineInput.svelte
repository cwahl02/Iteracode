<script lang="ts">
  let {
    value = $bindable(""),
    placeholder = "",
    oncommit,
    oncancel,
  }: {
    value?: string;
    placeholder?: string;
    oncommit: () => void;
    oncancel: () => void;
  } = $props();
</script>

<input
  bind:value
  {placeholder}
  autofocus
  class="h-5 text-xs px-1 py-0 w-32 rounded border border-orange-300
    bg-white outline-none ring-1 ring-orange-300 text-gray-800"
  onclick={(e) => e.stopPropagation()}
  onblur={oncommit}
  onkeydown={(e) => {
    e.stopPropagation();
    if (e.key === "Enter")  { e.preventDefault(); oncommit();  }
    if (e.key === "Escape") { e.preventDefault(); oncancel(); }
  }}
/>