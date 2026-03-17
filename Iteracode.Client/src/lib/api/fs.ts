import { apiFetch } from "./client";
import type { VFSFolder } from "$lib/types/admin";

export type TreeResponse = {
  node: VFSFolder;
};

export type FileContentResponse = {
  content: string;
};

export type RenameResponse = {
  newPath: string;
};

export const fsApi = {
  async getTree(): Promise<VFSFolder> {
    const res = await apiFetch("/api/fs/tree");
    if (!res.ok) throw new Error("Failed to load file tree");
    const { node }: TreeResponse = await res.json();
    return node;
  },

  async getFile(path: string): Promise<string> {
    const res = await apiFetch(`/api/fs/file?path=${encodeURIComponent(path)}`);
    if (!res.ok) throw new Error(`Failed to load ${path}`);
    const { content }: FileContentResponse = await res.json();
    return content;
  },

  async saveFile(path: string, content: string): Promise<void> {
    const res = await apiFetch("/api/fs/file", {
      method:  "PUT",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify({ path, content }),
    });
    if (!res.ok) throw new Error(`Failed to save ${path}`);
  },

  async createFile(path: string, content = ""): Promise<void> {
    const res = await apiFetch("/api/fs/file", {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify({ path, content }),
    });
    if (!res.ok) throw new Error(`Failed to create ${path}`);
  },

  async deleteFile(path: string): Promise<void> {
    const res = await apiFetch(`/api/fs/file?path=${encodeURIComponent(path)}`, {
      method: "DELETE",
    });
    if (!res.ok) throw new Error(`Failed to delete ${path}`);
  },

  async createFolder(path: string): Promise<void> {
    const res = await apiFetch("/api/fs/folder", {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify({ path }),
    });
    if (!res.ok) throw new Error(`Failed to create folder ${path}`);
  },

  async deleteFolder(path: string): Promise<void> {
    const res = await apiFetch(`/api/fs/folder?path=${encodeURIComponent(path)}`, {
      method: "DELETE",
    });
    if (!res.ok) throw new Error(`Failed to delete folder ${path}`);
  },

  async move(sourcePath: string, targetPath: string): Promise<void> {
    const res = await apiFetch("/api/fs/move", {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify({ sourcePath, targetPath }),
    });
    if (!res.ok) throw new Error("Failed to move");
  },

  async rename(path: string, newName: string): Promise<string> {
    const res = await apiFetch("/api/fs/rename", {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify({ path, newName }),
    });
    if (!res.ok) throw new Error(`Failed to rename ${path}`);
    const { newPath }: RenameResponse = await res.json();
    return newPath;
  },

  async upload(parentPath: string, files: FileList | File[]): Promise<void> {
    const formData = new FormData();
    formData.append("parentPath", parentPath);
    for (const file of Array.from(files)) {
      formData.append("files", file);
    }
    const res = await apiFetch("/api/fs/upload", {
      method: "POST",
      body:   formData,
    });
    if (!res.ok) throw new Error("Upload failed");
  },
};