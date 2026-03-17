export function createContentsStore() {
  // path → content cache
  let cache = $state<Record<string, string>>({});
  let loadingPaths = $state<Set<string>>(new Set());

  function get(path: string): string | undefined {
    return cache[path];
  }

  function set(path: string, content: string) {
    cache[path] = content;
  }

  function rekey(oldPath: string, newPath: string) {
    if (cache[oldPath] !== undefined) {
      cache[newPath] = cache[oldPath];
      delete cache[oldPath];
    }
  }

  function rekeyMany(pairs: [string, string][]) {
    for (const [oldPath, newPath] of pairs) rekey(oldPath, newPath);
  }

  function isLoading(path: string): boolean {
    return loadingPaths.has(path);
  }

  function setLoading(path: string, val: boolean) {
    const next = new Set(loadingPaths);
    if (val) next.add(path);
    else next.delete(path);
    loadingPaths = next;
  }

  function evict(path: string) {
    delete cache[path];
  }

  return {
    get,
    set,
    rekey,
    rekeyMany,
    isLoading,
    setLoading,
    evict,
  };
}