import type { VFSFolder, VFSNode } from "$lib/types/admin";

export function createTreeStore() {
  let root = $state<VFSFolder>({
    type: "folder",
    name: "root",
    path: "root",
    expanded: true,
    children: [],
  });

  let loading = $state(false);
  let error   = $state<string | null>(null);

  // ── Helpers ────────────────────────────────────────────────────────
  function getNode(path: string): VFSNode | null {
    if (path === "root") return root;
    const parts = path.split("/").slice(1);
    let current: VFSNode = root;
    for (const part of parts) {
      if (current.type !== "folder") return null;
      const found: VFSNode | undefined = current.children.find((c) => c.name === part);
      if (!found) return null;
      current = found;
    }
    return current;
  }

  function toggleFolder(path: string) {
    const node = getNode(path);
    if (node?.type === "folder") node.expanded = !node.expanded;
  }

  // ── Optimistic local mutations ─────────────────────────────────────
  function localCreateFolder(parentPath: string, name: string) {
    const parent = getNode(parentPath);
    if (parent?.type !== "folder") return;
    if (parent.children.find((c) => c.name === name)) return;
    parent.children.push({
      type: "folder",
      name,
      path: `${parentPath}/${name}`,
      expanded: false,
      children: [],
    });
  }

  function localCreateFile(parentPath: string, name: string) {
    const parent = getNode(parentPath);
    if (parent?.type !== "folder") return;
    if (parent.children.find((c) => c.name === name)) return;
    parent.children.push({
      type: "file",
      name,
      path: `${parentPath}/${name}`,
    });
  }

  function localDeleteNode(path: string) {
    const parts      = path.split("/");
    const parentPath = parts.slice(0, -1).join("/");
    const name       = parts[parts.length - 1];
    const parent     = getNode(parentPath);
    if (parent?.type !== "folder") return;
    parent.children  = parent.children.filter((c) => c.name !== name);
  }

  // Returns [oldPath, newPath] pairs for all affected file nodes
  function localRenameNode(path: string, newName: string): [string, string][] {
    const parts      = path.split("/");
    const parentPath = parts.slice(0, -1).join("/");
    const parent     = getNode(parentPath);
    if (parent?.type !== "folder") return [];

    const node = parent.children.find((c) => c.name === parts[parts.length - 1]);
    if (!node) return [];

    const newPath  = `${parentPath}/${newName}`;
    const remapped = collectRemap(node, path, newPath);
    applyRemap(node, path, newPath);
    node.name = newName;
    return remapped;
  }

  // Returns [oldPath, newPath] pairs for all affected file nodes
  function localMoveNode(sourcePath: string, targetFolderPath: string): [string, string][] {
    const srcParts      = sourcePath.split("/");
    const srcName       = srcParts[srcParts.length - 1];
    const srcParentPath = srcParts.slice(0, -1).join("/");

    const srcParent = getNode(srcParentPath);
    const target    = getNode(targetFolderPath);

    if (srcParent?.type !== "folder" || target?.type !== "folder") return [];
    if (targetFolderPath === srcParentPath) return [];
    if (targetFolderPath.startsWith(sourcePath + "/"))             return [];
    if (target.children.find((c) => c.name === srcName))          return [];

    const node = srcParent.children.find((c) => c.name === srcName);
    if (!node) return [];

    srcParent.children    = srcParent.children.filter((c) => c.name !== srcName);
    const newPath         = `${targetFolderPath}/${srcName}`;
    const remapped        = collectRemap(node, sourcePath, newPath);
    applyRemap(node, sourcePath, newPath);
    target.children       = [...target.children, node];
    return remapped;
  }

  function collectRemap(node: VFSNode, oldBase: string, newBase: string): [string, string][] {
    const pairs: [string, string][] = [];
    if (node.type === "file") {
      pairs.push([node.path, node.path.replace(oldBase, newBase)]);
    } else {
      for (const child of node.children) {
        pairs.push(...collectRemap(child, oldBase, newBase));
      }
    }
    return pairs;
  }

  function applyRemap(node: VFSNode, oldBase: string, newBase: string) {
    node.path = node.path.replace(oldBase, newBase);
    if (node.type === "folder") {
      for (const child of node.children) applyRemap(child, oldBase, newBase);
    }
  }

  function setRoot(newRoot: VFSFolder) {
    root = newRoot;
  }

  return {
    get root()    { return root;    },
    get loading() { return loading; },
    get error()   { return error;   },
    set loading(v) { loading = v;   },
    set error(v)   { error = v;     },
    getNode,
    toggleFolder,
    setRoot,
    localCreateFolder,
    localCreateFile,
    localDeleteNode,
    localRenameNode,
    localMoveNode,
  };
}