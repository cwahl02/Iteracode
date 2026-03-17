<script lang="ts">
  import { onMount } from "svelte";
  import { problemsStore, type ProblemSummary } from "$lib/stores/problems.svelte";

  onMount(() => problemsStore.fetchProblems());

  const languageColors: Record<string, string> = {
    python:     "bg-blue-100 text-blue-700",
    java:       "bg-orange-100 text-orange-700",
    javascript: "bg-yellow-100 text-yellow-700",
    cpp:        "bg-purple-100 text-purple-700",
    csharp:     "bg-green-100 text-green-700",
  };
</script>

<svelte:head>
  <title>Problems — MiniLeet</title>
</svelte:head>

<div class="max-w-3xl mx-auto px-4 py-10">
  <!-- Header -->
  <div class="mb-8">
    <h1 class="text-2xl font-bold text-gray-900">Problems</h1>
    <p class="text-sm text-gray-400 mt-1">Pick a problem and start solving</p>
  </div>

  {#if problemsStore.loading}
    <div class="flex flex-col gap-3">
      {#each Array(5) as _}
        <div class="h-16 rounded-lg bg-gray-100 animate-pulse"></div>
      {/each}
    </div>

  {:else if problemsStore.error}
    <div class="text-center py-16 text-gray-400">
      <p class="text-sm">Failed to load problems</p>
      <button
        onclick={() => problemsStore.fetchProblems()}
        class="mt-3 text-xs text-orange-500 hover:underline"
      >Try again</button>
    </div>

  {:else if problemsStore.problems.length === 0}
    <div class="text-center py-16 text-gray-300">
      <p class="text-sm">No problems published yet</p>
    </div>

  {:else}
    <!-- Column headers -->
    <div class="grid grid-cols-[1fr_auto] gap-4 px-4 mb-2 text-xs font-medium text-gray-400 uppercase tracking-wider">
      <span>Problem</span>
      <span>Languages</span>
    </div>

    <div class="flex flex-col divide-y divide-gray-100 border border-gray-100 rounded-lg overflow-hidden">
      {#each problemsStore.problems as problem (problem.slug)}
        <a
          href="/problems/{problem.slug}"
          class="grid grid-cols-[1fr_auto] gap-4 px-4 py-3 items-center
            hover:bg-orange-50 transition-colors group"
        >
          <!-- Left: title + tags -->
          <div class="min-w-0">
            <p class="text-sm font-medium text-gray-800 group-hover:text-orange-600 transition-colors truncate">
              {problem.title}
            </p>
            <div class="flex flex-wrap gap-1 mt-1">
              {#each problem.tags as tag}
                <span class="text-xs px-1.5 py-0.5 rounded bg-gray-100 text-gray-500">
                  {tag}
                </span>
              {/each}
            </div>
          </div>

          <!-- Right: language badges -->
          <div class="flex gap-1 flex-wrap justify-end shrink-0">
            {#each problem.languages as lang}
              <span class="text-xs px-2 py-0.5 rounded-full font-medium
                {languageColors[lang] ?? 'bg-gray-100 text-gray-600'}">
                {lang}
              </span>
            {/each}
          </div>
        </a>
      {/each}
    </div>
  {/if}
</div>