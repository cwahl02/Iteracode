import { vfs } from "$lib/stores/vfs.svelte";
import type { VFSFolder, VFSNode } from "$lib/types/admin";

function createExplorerStore() {
  let focusedFolder  = $state<VFSFolder>(vfs.root);
  let selectedPath   = $state<string | null>(null);
  let renamingPath   = $state<string | null>(null);
  let renameValue    = $state("");
  let renameOriginal = $state("");
  let dragSourcePath = $state<string | null>(null);
  let treeHoverPath  = $state<string | null>(null);

  // ── Rename ──────────────────────────────────────────────────────────
  function startRename(node: VFSNode) {
    renamingPath   = node.path;
    renameValue    = node.name;
    renameOriginal = node.name;
  }

  function commitRename(path: string) {
    const trimmed = renameValue.trim();
    if (trimmed && trimmed !== renameOriginal) {
      vfs.renameNode(path, trimmed);
      if (focusedFolder.path === path) {
        const newPath  = path.split("/").slice(0, -1).join("/") + "/" + trimmed;
        const updated  = vfs.getNode(newPath);
        if (updated?.type === "folder") focusedFolder = updated;
      }
    }
    renamingPath = null;
  }

  function cancelRename() {
    renameValue  = renameOriginal;
    renamingPath = null;
  }

  // ── Focus ────────────────────────────────────────────────────────────
  function focusNode(node: VFSFolder) {
    focusedFolder = node;
    selectedPath  = node.path;
    if (!node.expanded) vfs.toggleFolder(node.path);
  }

  function navigateTo(path: string) {
    const node = vfs.getNode(path);
    if (node?.type === "folder") focusNode(node);
  }

  // ── Drag ─────────────────────────────────────────────────────────────
  function startDrag(e: DragEvent, path: string) {
    dragSourcePath = path;
    e.dataTransfer?.setData("text/plain", path);
    e.dataTransfer!.effectAllowed = "move";
  }

  function endDrag() {
    dragSourcePath = null;
    treeHoverPath  = null;
  }

  function isOsFiles(e: DragEvent) {
    return Array.from(e.dataTransfer?.types ?? []).includes("Files");
  }

  function dropOnFolder(e: DragEvent, targetFolder: VFSFolder) {
    e.preventDefault();
    treeHoverPath = null;

    if (isOsFiles(e)) {
      readAndUpload(e.dataTransfer?.files ?? null, targetFolder.path);
      return;
    }

    const src = dragSourcePath;
    if (!src || src === targetFolder.path) return;
    if (targetFolder.path.startsWith(src + "/")) return;
    vfs.moveNode(src, targetFolder.path);
    dragSourcePath = null;
  }

  // ── Upload ───────────────────────────────────────────────────────────
  async function readAndUpload(fileList: FileList | null, targetPath: string) {
    if (!fileList) return;
    const results = await Promise.all(
      Array.from(fileList).map(
        (f) => new Promise<{ name: string; content: string }>((res) => {
          const reader = new FileReader();
          reader.onload = () => res({ name: f.name, content: reader.result as string });
          reader.readAsText(f);
        })
      )
    );
    vfs.uploadFiles(targetPath, results);
  }

  function triggerUpload(targetPath: string) {
    const input    = document.createElement("input");
    input.type     = "file";
    input.multiple = true;
    input.onchange = () => readAndUpload(input.files, targetPath);
    input.click();
  }

  return {
    get focusedFolder()  { return focusedFolder;  },
    get selectedPath()   { return selectedPath;   },
    get renamingPath()   { return renamingPath;   },
    get renameValue()    { return renameValue;     },
    set renameValue(v)   { renameValue = v;        },
    get dragSourcePath() { return dragSourcePath;  },
    get treeHoverPath()  { return treeHoverPath;   },
    set treeHoverPath(v) { treeHoverPath = v;      },
    set selectedPath(v)  { selectedPath = v;       },

    startRename,
    commitRename,
    cancelRename,
    focusNode,
    navigateTo,
    startDrag,
    endDrag,
    isOsFiles,
    dropOnFolder,
    readAndUpload,
    triggerUpload,
  };
}

export const explorer = createExplorerStore();