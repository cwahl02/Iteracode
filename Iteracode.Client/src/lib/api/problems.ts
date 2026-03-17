import { apiFetch } from "./client";

export type ProblemSummary = {
  slug:       string;
  published:  boolean;
  title:      string;
  tags:       string[];
  languages:  string[];
  createdAt:  string;
};

export type ProblemDetail = Omit<ProblemSummary, "languages"> & {
  description: string;
  languages: Record<string, {
    fileDir: string;
    files:   string[];
    runner:  string;
  }>;
};

export const problemsApi = {
  async list(): Promise<ProblemSummary[]> {
    const res = await apiFetch("/api/problems");
    if (!res.ok) throw new Error("Failed to load problems");
    return res.json();
  },

  async get(slug: string): Promise<ProblemDetail> {
    const res = await apiFetch(`/api/problems/${slug}`);
    if (!res.ok) throw new Error(`Failed to load problem ${slug}`);
    return res.json();
  },

  async delete(slug: string): Promise<void> {
    const res = await apiFetch(`/api/problems/${slug}`, { method: "DELETE" });
    if (!res.ok) throw new Error(`Failed to delete problem ${slug}`);
  },
};