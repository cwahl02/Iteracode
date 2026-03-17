<script lang="ts">
  import type { Tab, EditorTab, ExplorerTab } from "$lib/types/admin";
  import { vfs } from "$lib/stores/vfs.svelte";
  import TabBar from "./TabBar.svelte";
  import VFSExplorer from "./VFSExplorer.svelte";
  import EditorPane from "./EditorPane.svelte";

  // Seed with one pinned explorer tab
  let tabs = $state<Tab[]>([
    {
      id: "explorer-main",
      type: "explorer",
      label: "Explorer",
      pinned: true,
    },
  ]);

  let activeId = $state("explorer-main");

  const activeTab = $derived(tabs.find((t) => t.id === activeId) ?? tabs[0]);

  // Open a file — dedupes by path
  function openFile(path: string, label: string) {
    const existing = tabs.find((t) => t.type === "editor" && t.path === path);
    if (existing) {
      activeId = existing.id;
      return;
    }
    const id = crypto.randomUUID();
    const content = vfs.getFileContent(path);
    const newTab: EditorTab = {
      id,
      type: "editor",
      path,
      label,
      language: "python",
      content,
      savedContent: content,
      pinned: false,
    };
    tabs = [...tabs, newTab];
    activeId = id;
  }

  function closeTab(id: string) {
    const idx   = tabs.findIndex((t) => t.id === id);
    const tab   = tabs[idx];
    if (!tab || tab.pinned) return;

    // Warn on unsaved
    if (tab.type === "editor" && tab.content !== tab.savedContent) {
      if (!confirm(`"${tab.label}" has unsaved changes. Close anyway?`)) return;
    }

    tabs = tabs.filter((t) => t.id !== id);

    // Focus adjacent tab
    if (activeId === id) {
      const next = tabs[Math.min(idx, tabs.length - 1)];
      activeId = next?.id ?? "";
    }
  }

  function focusTab(id: string) {
    activeId = id;
  }

  function newExplorer() {
    const id: string = crypto.randomUUID();
    const newTab: ExplorerTab = {
      id,
      type: "explorer",
      label: "Explorer",
      pinned: false,
    };
    tabs = [...tabs, newTab];
    activeId = id;
  }

  function newEditor() {
    const id: string = crypto.randomUUID();
    const newTab: EditorTab = {
      id,
      type: "editor",
      path: "",
      label: "untitled",
      language: "python",
      content: "",
      savedContent: "",
      pinned: false,
    };
    tabs = [...tabs, newTab];
    activeId = id;
  }

  function updateContent(id: string, content: string) {
    tabs = tabs.map((t) =>
      t.id === id && t.type === "editor" ? { ...t, content } : t
    );
  }

  function markSaved(id: string) {
    tabs = tabs.map((t) =>
      t.id === id && t.type === "editor" ? { ...t, savedContent: t.content } : t
    );
  }
</script>

<div class="h-full flex flex-col overflow-hidden">
  <TabBar
    {tabs}
    {activeId}
    onfocus={focusTab}
    onclose={closeTab}
    onnewexplorer={newExplorer}
    onneweditor={newEditor}
  />

  <!-- Content area -->
  <div class="flex-1 overflow-hidden">
    {#if activeTab?.type === "explorer"}
      <VFSExplorer onopenfile={openFile} />

    {:else if activeTab?.type === "editor"}
      {#key activeTab.id}
        <EditorPane
          tab={activeTab}
          onupdatecontent={updateContent}
          onsave={markSaved}
        />
      {/key}
    {/if}
  </div>
</div>