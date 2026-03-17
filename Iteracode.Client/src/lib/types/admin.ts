export type VFSFile = {
  type: "file";
  name: string;
  path: string;
};

export type VFSFolder = {
  type: "folder";
  name: string;
  path: string;
  children: VFSNode[];
  expanded: boolean;
};

export type VFSNode = VFSFile | VFSFolder;

export type EditorTab = {
  id: string;
  type: "editor";
  path: string;
  label: string;
  language: string;
  content: string;
  savedContent: string;
  pinned: false;
};

export type ExplorerTab = {
  id: string;
  type: "explorer";
  label: string;
  pinned: boolean;
};

export type Tab = EditorTab | ExplorerTab;