export async function uploadFiles(
  parentPath: string,
  files: FileList | File[],
  baseUrl = "/api/fs/upload"
): Promise<void> {
  const formData = new FormData();
  formData.append("parentPath", parentPath);
  for (const file of Array.from(files)) {
    formData.append("files", file);
  }
  const res = await fetch(baseUrl, { method: "POST", body: formData });
  if (!res.ok) {
    const text = await res.text().catch(() => "Upload failed");
    throw new Error(text);
  }
}