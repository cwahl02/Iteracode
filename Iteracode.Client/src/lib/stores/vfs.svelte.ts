import { createTreeStore }     from "./vfs/tree.svelte";
import { createContentsStore } from "./vfs/contents.svelte";
import { fsApi }               from "$lib/api/fs";
import { toast }               from "svelte-sonner";

function createVFSStore() {
  const tree     = createTreeStore();
  const contents = createContentsStore();

  async function fetchTree() {
    tree.loading = true;
    tree.error   = null;
    try {
      const root = await fsApi.getTree();
      tree.setRoot(root);
    } catch (e) {
      tree.error = (e as Error).message;
      toast.error("Could not load file tree");
    } finally {
      tree.loading = false;
    }
  }

  async function getFileContent(path: string): Promise<string> {
    const cached = contents.get(path);
    if (cached !== undefined) return cached;
    contents.setLoading(path, true);
    try {
      const content = await fsApi.getFile(path);
      contents.set(path, content);
      return content;
    } catch (e) {
      toast.error((e as Error).message);
      return "";
    } finally {
      contents.setLoading(path, false);
    }
  }

  async function saveFileContent(path: string, content: string) {
    try {
      await fsApi.saveFile(path, content);
      contents.set(path, content);
    } catch (e) {
      toast.error((e as Error).message);
      throw e;
    }
  }

  async function createFile(parentPath: string, name: string, content = "") {
    const path = `${parentPath}/${name}`;
    tree.localCreateFile(parentPath, name);
    contents.set(path, content);
    try {
      await fsApi.createFile(path, content);
    } catch (e) {
      tree.localDeleteNode(path);
      contents.evict(path);
      toast.error((e as Error).message);
    }
  }

  async function createFolder(parentPath: string, name: string) {
    const path = `${parentPath}/${name}`;
    tree.localCreateFolder(parentPath, name);
    try {
      await fsApi.createFolder(path);
    } catch (e) {
      tree.localDeleteNode(path);
      toast.error((e as Error).message);
    }
  }

  async function deleteNode(path: string) {
    const node = tree.getNode(path);
    if (!node) return;
    tree.localDeleteNode(path);
    try {
      if (node.type === "folder") await fsApi.deleteFolder(path);
      else await fsApi.deleteFile(path);
      if (node.type === "file") contents.evict(path);
    } catch (e) {
      toast.error((e as Error).message);
      await fetchTree();
    }
  }

  async function renameNode(path: string, newName: string) {
    const remapped = tree.localRenameNode(path, newName);
    contents.rekeyMany(remapped);
    try {
      await fsApi.rename(path, newName);
    } catch (e) {
      toast.error((e as Error).message);
      await fetchTree();
    }
  }

  async function moveNode(sourcePath: string, targetFolderPath: string) {
    const remapped = tree.localMoveNode(sourcePath, targetFolderPath);
    contents.rekeyMany(remapped);
    try {
      await fsApi.move(sourcePath, targetFolderPath);
    } catch (e) {
      toast.error((e as Error).message);
      await fetchTree();
    }
  }

  async function upload(parentPath: string, files: FileList | File[]) {
    try {
      await fsApi.upload(parentPath, files);
      await fetchTree();
    } catch (e) {
      toast.error((e as Error).message);
    }
  }

  return {
    get root()    { return tree.root;    },
    get loading() { return tree.loading; },
    get error()   { return tree.error;   },
    getNode:       tree.getNode.bind(tree),
    toggleFolder:  tree.toggleFolder.bind(tree),
    isFileLoading: contents.isLoading.bind(contents),
    fetchTree,
    getFileContent,
    saveFileContent,
    createFile,
    createFolder,
    deleteNode,
    renameNode,
    moveNode,
    upload,
  };
}

export const vfs = createVFSStore();