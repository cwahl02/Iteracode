<script lang="ts">
  import { vfs } from "$lib/stores/vfs.svelte";
  import CodeMirrorEditor from "$lib/components/CodeMirrorEditor.svelte";
  import { Button } from "$lib/components/ui/button";
  import type { EditorTab } from "$lib/types/admin";

  let {
    tab,
    onupdatecontent,
    onsave,
  }: {
    tab: EditorTab;
    onupdatecontent: (id: string, content: string) => void;
    onsave: (id: string) => void;
  } = $props();

  const unsaved  = $derived(tab.content !== tab.savedContent);
  const language = $derived(tab.label.endsWith(".py")   ? "python"
                          : tab.label.endsWith(".js")   ? "javascript"
                          : tab.label.endsWith(".java") ? "java"
                          : tab.label.endsWith(".cpp")  ? "cpp"
                          : tab.label.endsWith(".json") ? "json"
                          : "python");

  const languageBadgeColor: Record<string, string> = {
    python:     "bg-blue-100 text-blue-700",
    javascript: "bg-yellow-100 text-yellow-700",
    java:       "bg-orange-100 text-orange-700",
    cpp:        "bg-purple-100 text-purple-700",
    json:       "bg-green-100 text-green-700",
  };

  function handleKeydown(e: KeyboardEvent) {
    if ((e.ctrlKey || e.metaKey) && e.key === "s") {
      e.preventDefault();
      handleSave();
    }
  }

  function handleSave() {
    vfs.setFileContent(tab.path, tab.content);
    onsave(tab.id);
  }

  // Breadcrumb parts from path
  const breadcrumbs = $derived(tab.path.split("/"));
</script>

<svelte:window onkeydown={handleKeydown} />

<div class="h-full flex flex-col overflow-hidden">
  <!-- Toolbar -->
  <div class="flex items-center gap-2 px-3 py-1.5 border-b border-gray-200 bg-gray-50 shrink-0">
    <!-- Breadcrumb -->
    <div class="flex items-center gap-1 text-xs text-gray-400 flex-1 min-w-0">
      {#each breadcrumbs as crumb, i}
        <span
          class="{i === breadcrumbs.length - 1
            ? 'text-gray-700 font-medium'
            : 'hover:text-gray-600 cursor-default'} truncate"
        >{crumb}</span>
        {#if i < breadcrumbs.length - 1}
          <span class="text-gray-300 shrink-0">/</span>
        {/if}
      {/each}
    </div>

    <!-- Language badge -->
    <span class="text-xs px-2 py-0.5 rounded-full font-medium shrink-0
      {languageBadgeColor[language] ?? 'bg-gray-100 text-gray-600'}">
      {language}
    </span>

    <!-- Unsaved indicator -->
    {#if unsaved}
      <span class="text-xs text-orange-500 shrink-0">unsaved</span>
    {/if}

    <!-- Save button -->
    <Button
      size="sm"
      variant={unsaved ? "default" : "outline"}
      class="h-6 text-xs px-2 shrink-0
        {unsaved ? 'bg-orange-500 hover:bg-orange-600 text-white' : ''}"
      onclick={handleSave}
      disabled={!unsaved}
    >
      {unsaved ? "Save" : "Saved"}
    </Button>
  </div>

  <!-- Editor -->
  <div class="flex-1 overflow-hidden">
    <CodeMirrorEditor
      bind:value={tab.content}
      {language}
      onChange={(v) => onupdatecontent(tab.id, v)}
    />
  </div>
</div>