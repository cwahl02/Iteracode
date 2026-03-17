import { apiFetch } from "./client";

export type SubmitRequest = {
  problemSlug: string;
  language:    string;
  code:        string;
};

export type SubmissionResult = {
  passed:      boolean;
  submittedAt: string;
};

export const submissionsApi = {
  async submit(body: SubmitRequest): Promise<SubmissionResult> {
    const res = await apiFetch("/api/submissions", {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify(body),
    });
    if (!res.ok) throw new Error("Submission failed");
    return res.json();
  },

  async getResult(slug: string): Promise<SubmissionResult | null> {
    const res = await apiFetch(`/api/submissions/${slug}`);
    if (res.status === 404) return null;
    if (!res.ok) throw new Error("Failed to load submission");
    return res.json();
  },
};