export const PROTECTED_PATHS = new Set([
  "root",
  "root/recycled",
  "root/problems",
  "root/problems/scripts",
]);

export function isProtected(path: string): boolean {
  return PROTECTED_PATHS.has(path);
}

export function isRecyclable(path: string): boolean {
  // only direct children of root/problems
  const parts = path.split("/");
  return parts.length === 3 && parts[0] === "root" && parts[1] === "problems";
}

export function isProblemJson(path: string): boolean {
  return path.endsWith("/problem.json");
}

export function canDelete(path: string): boolean {
  return !isProtected(path);
}

export function canRename(path: string): boolean {
  return !isProtected(path) && !isProblemJson(path);
}

export function canMove(path: string): boolean {
  return !isProtected(path) && !isProblemJson(path);
}