<script lang="ts">
  import * as Resizable from "$lib/components/ui/resizable";
  import * as Select from "$lib/components/ui/select";
  import { Button } from "$lib/components/ui/button";
  import { Badge } from "$lib/components/ui/badge";
  import { ScrollArea } from "$lib/components/ui/scroll-area";
  import { Separator } from "$lib/components/ui/separator";
  import * as Tabs from "$lib/components/ui/tabs";
  import CodeMirrorEditor from "$lib/components/CodeMirrorEditor.svelte";

  // TODO: replace with real API data
  const problem = {
    title: "Two Sum",
    slug: "two-sum",
    difficulty: "Easy",
    tags: ["Array", "Hash Table"],
    description: `Given an array of integers \`nums\` and an integer \`target\`, return indices of the two numbers such that they add up to \`target\`.

You may assume that each input would have exactly one solution, and you may not use the same element twice.

You can return the answer in any order.`,
    examples: [
      {
        input: "nums = [2,7,11,15], target = 9",
        output: "[0,1]",
        explanation: "Because nums[0] + nums[1] == 9, we return [0, 1].",
      },
      {
        input: "nums = [3,2,4], target = 6",
        output: "[1,2]",
        explanation: undefined,
      },
    ],
    constraints: [
      "2 ≤ nums.length ≤ 10⁴",
      "-10⁹ ≤ nums[i] ≤ 10⁹",
      "-10⁹ ≤ target ≤ 10⁹",
      "Only one valid answer exists.",
    ],
  };

  let language = $state("python");
  let code = $state(`def two_sum(nums, target):\n    # Your solution here\n    pass`);
  let isRunning = $state(false);
  let isSubmitting = $state(false);
  let activeOutputTab = $state("testcases");

  type TestResult = { status: "passed" | "failed" | "error"; input: string; expected: string; got: string };
  let testResults = $state<TestResult[]>([]);
  let submitResult = $state<{ status: "accepted" | "wrong" | "error"; message: string } | null>(null);

  const languages = [
    { value: "python",     label: "Python" },
    { value: "javascript", label: "JavaScript" },
    { value: "cpp",        label: "C++" },
    { value: "java",       label: "Java" },
  ];

  const difficultyColor: Record<string, string> = {
    Easy:   "bg-green-100 text-green-700",
    Medium: "bg-yellow-100 text-yellow-700",
    Hard:   "bg-red-100 text-red-700",
  };

  async function handleRun() {
    isRunning = true;
    activeOutputTab = "testcases";
    // TODO: wire up API
    await new Promise((r) => setTimeout(r, 700));
    testResults = [
      { status: "passed", input: problem.examples[0].input, expected: problem.examples[0].output, got: "[0,1]" },
      { status: "passed", input: problem.examples[1].input, expected: problem.examples[1].output, got: "[1,2]" },
    ];
    isRunning = false;
  }

  async function handleSubmit() {
    isSubmitting = true;
    activeOutputTab = "result";
    // TODO: wire up API
    await new Promise((r) => setTimeout(r, 900));
    submitResult = { status: "accepted", message: "Accepted — 42ms runtime, beats 88% of submissions" };
    isSubmitting = false;
  }
</script>

<svelte:head>
  <title>{problem.title} — MiniLeet</title>
</svelte:head>

<div class="h-screen flex flex-col bg-white overflow-hidden">
  <!-- Top Nav -->
  <header class="border-b border-gray-200 px-4 py-2 flex items-center justify-between shrink-0">
    <div class="flex items-center gap-2">
      <a href="/" class="text-sm font-bold tracking-tight text-gray-900 hover:text-orange-500 transition-colors">MiniLeet</a>
      <span class="text-gray-300">/</span>
      <a href="/problems" class="text-sm text-gray-500 hover:text-gray-700 transition-colors">Problems</a>
      <span class="text-gray-300">/</span>
      <span class="text-sm text-gray-700 font-medium">{problem.title}</span>
    </div>
    <a href="/playground" class="text-xs text-gray-400 hover:text-gray-600 transition-colors">Playground</a>
  </header>

  <!-- Main -->
  <div class="flex-1 overflow-hidden">
    <Resizable.PaneGroup direction="horizontal" class="h-full">

      <!-- LEFT: Problem Description -->
      <Resizable.Pane defaultSize={38} minSize={25} class="flex flex-col">
        <ScrollArea class="flex-1">
          <div class="p-5 space-y-5">
            <!-- Title + Difficulty -->
            <div class="space-y-2">
              <div class="flex items-center gap-2 flex-wrap">
                <h1 class="text-lg font-bold text-gray-900">{problem.title}</h1>
                <span class="text-xs px-2 py-0.5 rounded-full font-medium {difficultyColor[problem.difficulty]}">
                  {problem.difficulty}
                </span>
              </div>
              <div class="flex flex-wrap gap-1">
                {#each problem.tags as tag}
                  <Badge variant="secondary" class="text-xs">{tag}</Badge>
                {/each}
              </div>
            </div>

            <Separator />

            <!-- Description -->
            <div class="text-sm text-gray-700 leading-relaxed whitespace-pre-line">
              {problem.description}
            </div>

            <!-- Examples -->
            <div class="space-y-3">
              {#each problem.examples as example, i}
                <div class="rounded-lg bg-gray-50 border border-gray-200 p-3 text-xs font-mono space-y-1">
                  <div class="font-semibold text-gray-500 font-sans text-xs mb-1">Example {i + 1}</div>
                  <div><span class="text-gray-500">Input:</span>  {example.input}</div>
                  <div><span class="text-gray-500">Output:</span> {example.output}</div>
                  {#if example.explanation}
                    <div><span class="text-gray-500">Explanation:</span> {example.explanation}</div>
                  {/if}
                </div>
              {/each}
            </div>

            <!-- Constraints -->
            <div class="space-y-1">
              <div class="text-xs font-semibold text-gray-600">Constraints</div>
              <ul class="list-disc list-inside space-y-0.5">
                {#each problem.constraints as c}
                  <li class="text-xs text-gray-600 font-mono">{c}</li>
                {/each}
              </ul>
            </div>
          </div>
        </ScrollArea>
      </Resizable.Pane>

      <Resizable.Handle withHandle />

      <!-- RIGHT: Editor + Output stacked vertically -->
      <Resizable.Pane defaultSize={62} minSize={35} class="flex flex-col overflow-hidden">
        <Resizable.PaneGroup direction="vertical" class="h-full">

          <!-- TOP RIGHT: Editor -->
          <Resizable.Pane defaultSize={65} minSize={30} class="flex flex-col">
            <!-- Editor Toolbar -->
            <div class="flex items-center gap-2 px-3 py-2 border-b border-gray-200 bg-gray-50 shrink-0">
              <Select.Root type="single" bind:value={language}>
                <Select.Trigger class="h-7 w-36 text-xs">
                  {languages.find(l => l.value === language)?.label ?? "Language"}
                </Select.Trigger>
                <Select.Content>
                  {#each languages as lang}
                    <Select.Item value={lang.value}>{lang.label}</Select.Item>
                  {/each}
                </Select.Content>
              </Select.Root>

              <div class="flex-1" />

              <Button
                variant="outline"
                size="sm"
                class="h-7 text-xs gap-1"
                onclick={handleRun}
                disabled={isRunning || isSubmitting}
              >
                {#if isRunning}
                  <svg class="animate-spin h-3 w-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                  </svg>
                  Running...
                {:else}
                  Run
                {/if}
              </Button>

              <Button
                size="sm"
                class="h-7 text-xs gap-1 bg-orange-500 hover:bg-orange-600 text-white"
                onclick={handleSubmit}
                disabled={isRunning || isSubmitting}
              >
                {#if isSubmitting}
                  <svg class="animate-spin h-3 w-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                  </svg>
                  Submitting...
                {:else}
                  Submit
                {/if}
              </Button>
            </div>

            <!-- Code Editor -->
            <div class="flex-1 overflow-hidden">
              <CodeMirrorEditor bind:value={code} {language} onChange={(v) => (code = v)} />
            </div>
          </Resizable.Pane>

          <Resizable.Handle withHandle />

          <!-- BOTTOM RIGHT: Output -->
          <Resizable.Pane defaultSize={35} minSize={15} class="flex flex-col">
            <div class="flex items-center px-3 border-b border-gray-200 bg-gray-50 shrink-0">
              <Tabs.Root bind:value={activeOutputTab} class="w-full">
                <Tabs.List class="h-9 bg-transparent gap-0 p-0 border-0 rounded-none">
                  <Tabs.Trigger value="testcases" class="text-xs h-9 rounded-none border-b-2 border-transparent data-[state=active]:border-orange-500 data-[state=active]:shadow-none bg-transparent">
                    Test Cases
                  </Tabs.Trigger>
                  <Tabs.Trigger value="result" class="text-xs h-9 rounded-none border-b-2 border-transparent data-[state=active]:border-orange-500 data-[state=active]:shadow-none bg-transparent">
                    Result
                  </Tabs.Trigger>
                </Tabs.List>

                <Tabs.Content value="testcases">
                  <ScrollArea class="h-full bg-white">
                    <div class="p-4">
                      {#if testResults.length === 0}
                        <p class="text-xs text-gray-400">Run your code to see test case results.</p>
                      {:else}
                        <div class="space-y-2">
                          {#each testResults as result, i}
                            <div class="rounded-lg border p-3 text-xs font-mono space-y-1
                              {result.status === 'passed' ? 'border-green-200 bg-green-50' : 'border-red-200 bg-red-50'}">
                              <div class="flex items-center gap-2 font-sans mb-1">
                                <span class="font-semibold {result.status === 'passed' ? 'text-green-700' : 'text-red-700'}">
                                  {result.status === 'passed' ? '✓' : '✗'} Case {i + 1}
                                </span>
                              </div>
                              <div><span class="text-gray-500">Input:</span>    {result.input}</div>
                              <div><span class="text-gray-500">Expected:</span> {result.expected}</div>
                              <div><span class="text-gray-500">Got:</span>      {result.got}</div>
                            </div>
                          {/each}
                        </div>
                      {/if}
                    </div>
                  </ScrollArea>
                </Tabs.Content>

                <Tabs.Content value="result">
                  <ScrollArea class="h-full bg-white">
                    <div class="p-4">
                      {#if !submitResult}
                        <p class="text-xs text-gray-400">Submit your code to see the result.</p>
                      {:else}
                        <div class="rounded-lg border p-4 text-sm
                          {submitResult.status === 'accepted' ? 'border-green-200 bg-green-50' :
                           submitResult.status === 'wrong'    ? 'border-red-200 bg-red-50'    :
                                                                'border-yellow-200 bg-yellow-50'}">
                          <div class="font-bold text-base mb-1
                            {submitResult.status === 'accepted' ? 'text-green-700' :
                             submitResult.status === 'wrong'    ? 'text-red-700'   : 'text-yellow-700'}">
                            {submitResult.status === 'accepted' ? '✓ Accepted' :
                             submitResult.status === 'wrong'    ? '✗ Wrong Answer' : '⚠ Error'}
                          </div>
                          <p class="text-xs text-gray-600">{submitResult.message}</p>
                        </div>
                      {/if}
                    </div>
                  </ScrollArea>
                </Tabs.Content>
              </Tabs.Root>
            </div>
          </Resizable.Pane>

        </Resizable.PaneGroup>
      </Resizable.Pane>

    </Resizable.PaneGroup>
  </div>
</div>