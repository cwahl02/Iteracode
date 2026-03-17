import { toast } from "svelte-sonner";

export type ProblemSummary = {
  slug:       string;
  published:  boolean;
  title:      string;
  tags:       string[];
  languages:  string[];
  createdAt:  string;
};

export type ProblemDetail = ProblemSummary & {
  description: string;
  languages: Record<string, {
    fileDir: string;
    files:   string[];
    runner:  string;
  }>;
};

function createProblemsStore() {
  let problems = $state<ProblemSummary[]>([]);
  let loading  = $state(false);
  let error    = $state<string | null>(null);

  async function fetchProblems() {
    loading = true;
    error   = null;
    try {
      const res = await fetch("/api/problems");
      if (!res.ok) throw new Error("Failed to load problems");
      problems = await res.json();
    } catch (e) {
      error = (e as Error).message;
      toast.error("Could not load problems");
    } finally {
      loading = false;
    }
  }

  async function publish(slug: string) {
    const idx = problems.findIndex((p) => p.slug === slug);
    if (idx === -1) return;

    // optimistic
    problems[idx] = { ...problems[idx], published: true };
    try {
      const res = await fetch(`/api/problems/${slug}/publish`, { method: "POST" });
      if (!res.ok) throw new Error(`Failed to publish ${slug}`);
      toast.success(`"${problems[idx].title}" published`);
    } catch (e) {
      problems[idx] = { ...problems[idx], published: false };
      toast.error((e as Error).message);
    }
  }

  async function unpublish(slug: string) {
    const idx = problems.findIndex((p) => p.slug === slug);
    if (idx === -1) return;

    problems[idx] = { ...problems[idx], published: false };
    try {
      const res = await fetch(`/api/problems/${slug}/unpublish`, { method: "POST" });
      if (!res.ok) throw new Error(`Failed to unpublish ${slug}`);
      toast.success(`"${problems[idx].title}" unpublished`);
    } catch (e) {
      problems[idx] = { ...problems[idx], published: true };
      toast.error((e as Error).message);
    }
  }

  return {
    get problems() { return problems; },
    get loading()  { return loading;  },
    get error()    { return error;    },
    fetchProblems,
    publish,
    unpublish,
  };
}

export const problemsStore = createProblemsStore();