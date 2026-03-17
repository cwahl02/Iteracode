<script lang="ts">
  import { page } from "$app/stores";
  import * as Resizable from "$lib/components/ui/resizable";
  import * as Tabs from "$lib/components/ui/tabs";
  import { Button } from "$lib/components/ui/button";
  import { Badge } from "$lib/components/ui/badge";
  import { ScrollArea } from "$lib/components/ui/scroll-area";
  import CodeMirrorEditor from "$lib/components/CodeMirrorEditor.svelte";

  const slug = $derived($page.params.slug);
  const isNew = $derived(slug === "new");

  // TODO: load real data from API / JSON file based on slug
  const defaultJson = JSON.stringify(
    {
      slug: "two-sum",
      title: "Two Sum",
      difficulty: "Easy",
      tags: ["Array", "Hash Table"],
      description: "Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.",
      examples: [
        { input: "nums = [2,7,11,15], target = 9", output: "[0,1]", explanation: "nums[0] + nums[1] == 9" },
        { input: "nums = [3,2,4], target = 6",      output: "[1,2]", explanation: "" },
      ],
      constraints: [
        "2 <= nums.length <= 10^4",
        "-10^9 <= nums[i] <= 10^9",
        "Only one valid answer exists."
      ],
      testCases: [
        { input: { nums: [2, 7, 11, 15], target: 9 }, expected: [0, 1] },
        { input: { nums: [3, 2, 4],      target: 6 }, expected: [1, 2] },
        { input: { nums: [3, 3],          target: 6 }, expected: [0, 1] },
      ],
    },
    null,
    2
  );

  const defaultPython = `def solution(nums, target):
    seen = {}
    for i, n in enumerate(nums):
        diff = target - n
        if diff in seen:
            return [seen[diff], i]
        seen[n] = i
    return []
`;

  let jsonCode  = $state(isNew ? '{\n  \n}' : defaultJson);
  let pyCode    = $state(isNew ? 'def solution():\n    pass\n' : defaultPython);
  let activeTab = $state<"json" | "python">("json");

  type RunResult = {
    status: "pass" | "fail" | "error";
    case: number;
    input: string;
    expected: string;
    got: string;
    stdout?: string;
  };

  let isRunning    = $state(false);
  let isSaving     = $state(false);
  let saveStatus   = $state<"idle" | "saved" | "error">("idle");
  let runResults   = $state<RunResult[]>([]);
  let runError     = $state<string | null>(null);
  let hasRun       = $state(false);

  let jsonValid    = $state(true);
  let jsonError    = $state<string | null>(null);

  // Validate JSON as user types
  $effect(() => {
    try {
      JSON.parse(jsonCode);
      jsonValid = true;
      jsonError = null;
    } catch (e: any) {
      jsonValid = false;
      jsonError = e.message;
    }
  });

  async function handleRun() {
    if (!jsonValid) {
      activeTab = "json";
      return;
    }
    isRunning = true;
    hasRun = true;
    runError = null;
    runResults = [];
    // TODO: wire up API — send jsonCode + pyCode, receive test results
    await new Promise((r) => setTimeout(r, 800));
    // Stub response
    runResults = [
      { status: "pass",  case: 1, input: "nums=[2,7,11,15], target=9", expected: "[0, 1]", got: "[0, 1]" },
      { status: "pass",  case: 2, input: "nums=[3,2,4], target=6",     expected: "[1, 2]", got: "[1, 2]" },
      { status: "fail",  case: 3, input: "nums=[3,3], target=6",       expected: "[0, 1]", got: "[1, 0]" },
    ];
    isRunning = false;
  }

  async function handleSave() {
    if (!jsonValid) {
      activeTab = "json";
      return;
    }
    isSaving = true;
    // TODO: wire up API — save JSON + Python file
    await new Promise((r) => setTimeout(r, 600));
    saveStatus = "saved";
    isSaving = false;
    setTimeout(() => (saveStatus = "idle"), 2000);
  }

  const passCount  = $derived(runResults.filter((r) => r.status === "pass").length);
  const failCount  = $derived(runResults.filter((r) => r.status === "fail").length);
  const allPassed  = $derived(hasRun && runResults.length > 0 && failCount === 0);
</script>

<svelte:head>
  <title>{isNew ? "New Problem" : `Edit: ${slug}`} — Admin</title>
</svelte:head>

<div class="h-screen flex flex-col bg-white overflow-hidden">
  <!-- Top bar -->
  <header class="border-b border-gray-200 px-4 py-2 flex items-center justify-between shrink-0">
    <div class="flex items-center gap-2 text-sm">
      <a href="/" class="font-bold text-gray-900 hover:text-orange-500 transition-colors">MiniLeet</a>
      <span class="text-gray-300">/</span>
      <a href="/admin" class="text-gray-500 hover:text-gray-700 transition-colors">Admin</a>
      <span class="text-gray-300">/</span>
      <span class="text-gray-700 font-medium">{isNew ? "New Problem" : slug}</span>
      {#if !jsonValid}
        <Badge variant="destructive" class="text-xs ml-1">JSON Error</Badge>
      {/if}
    </div>

    <div class="flex items-center gap-2">
      <!-- Run status summary -->
      {#if hasRun && !isRunning}
        <span class="text-xs {allPassed ? 'text-green-600' : 'text-red-500'} font-medium">
          {allPassed ? `✓ All ${passCount} passed` : `${passCount} passed · ${failCount} failed`}
        </span>
      {/if}

      <Button
        variant="outline"
        size="sm"
        class="h-7 text-xs gap-1"
        onclick={handleRun}
        disabled={isRunning || isSaving}
      >
        {#if isRunning}
          <svg class="animate-spin h-3 w-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
          </svg>
          Running...
        {:else}
          <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd" />
          </svg>
          Run Tests
        {/if}
      </Button>

      <Button
        size="sm"
        class="h-7 text-xs gap-1 bg-orange-500 hover:bg-orange-600 text-white min-w-[80px]"
        onclick={handleSave}
        disabled={isRunning || isSaving || !jsonValid}
      >
        {#if isSaving}
          Saving...
        {:else if saveStatus === "saved"}
          ✓ Saved
        {:else}
          Save
        {/if}
      </Button>
    </div>
  </header>

  <!-- Main -->
  <div class="flex-1 overflow-hidden">
    <Resizable.PaneGroup direction="horizontal" class="h-full">

      <!-- LEFT: Tabbed editor -->
      <Resizable.Pane defaultSize={55} minSize={30} class="flex flex-col">

        <!-- Tab bar -->
        <Tabs.Root bind:value={activeTab} class="flex flex-col h-full">
          <div class="border-b border-gray-200 bg-gray-50 px-3 shrink-0">
            <Tabs.List class="h-9 bg-transparent gap-0 p-0 border-0 rounded-none">
              <Tabs.Trigger
                value="json"
                class="text-xs h-9 rounded-none border-b-2 border-transparent data-[state=active]:border-orange-500 data-[state=active]:shadow-none bg-transparent gap-1.5"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                problem.json
                {#if !jsonValid}
                  <span class="w-1.5 h-1.5 rounded-full bg-red-500 inline-block"></span>
                {/if}
              </Tabs.Trigger>
              <Tabs.Trigger
                value="python"
                class="text-xs h-9 rounded-none border-b-2 border-transparent data-[state=active]:border-orange-500 data-[state=active]:shadow-none bg-transparent gap-1.5"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
                </svg>
                solution.py
              </Tabs.Trigger>
            </Tabs.List>
          </div>

          <!-- JSON editor -->
          <Tabs.Content value="json" class="flex-1 overflow-hidden m-0 data-[state=active]:flex flex-col">
            {#if jsonError}
              <div class="shrink-0 px-3 py-1.5 bg-red-50 border-b border-red-200 text-xs text-red-600 font-mono">
                ⚠ {jsonError}
              </div>
            {/if}
            <div class="flex-1 overflow-hidden">
              <CodeMirrorEditor bind:value={jsonCode} language="json" onChange={(v) => (jsonCode = v)} />
            </div>
          </Tabs.Content>

          <!-- Python editor -->
          <Tabs.Content value="python" class="flex-1 overflow-hidden m-0 data-[state=active]:flex flex-col">
            <div class="flex-1 overflow-hidden">
              <CodeMirrorEditor bind:value={pyCode} language="python" onChange={(v) => (pyCode = v)} />
            </div>
          </Tabs.Content>
        </Tabs.Root>
      </Resizable.Pane>

      <Resizable.Handle withHandle />

      <!-- RIGHT: Output -->
      <Resizable.Pane defaultSize={45} minSize={25} class="flex flex-col">
        <!-- Output toolbar -->
        <div class="flex items-center gap-2 px-3 py-2 border-b border-gray-200 bg-gray-50 shrink-0">
          <span class="text-xs font-semibold text-gray-600">Test Output</span>
          {#if hasRun && !isRunning}
            <span class="text-xs px-2 py-0.5 rounded-full font-medium {allPassed ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}">
              {passCount}/{runResults.length} passed
            </span>
          {/if}
        </div>

        <ScrollArea class="flex-1 bg-gray-950">
          <div class="p-4 space-y-3 font-mono text-xs">
            {#if !hasRun && !isRunning}
              <p class="text-gray-500">Hit <span class="text-gray-300 font-semibold">Run Tests</span> to execute the reference solution against test cases defined in the JSON.</p>
              <div class="mt-4 space-y-1 text-gray-600">
                <p class="font-sans text-xs font-semibold text-gray-400 mb-2">Workflow</p>
                <p>1. Define problem + test cases in <span class="text-orange-400">problem.json</span></p>
                <p>2. Write reference solution in <span class="text-blue-400">solution.py</span></p>
                <p>3. Run Tests → verify all cases pass</p>
                <p>4. Save when ready</p>
              </div>
            {:else if isRunning}
              <div class="flex items-center gap-2 text-gray-400">
                <svg class="animate-spin h-3 w-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
                </svg>
                Running test cases...
              </div>
            {:else if runError}
              <div class="rounded-lg border border-red-800 bg-red-950 p-3">
                <p class="text-red-400 font-sans font-semibold text-xs mb-1">⚠ Runtime Error</p>
                <pre class="text-red-300 whitespace-pre-wrap">{runError}</pre>
              </div>
            {:else}
              {#each runResults as result}
                <div class="rounded-lg border p-3 space-y-1.5
                  {result.status === 'pass'
                    ? 'border-green-800 bg-green-950'
                    : result.status === 'fail'
                    ? 'border-red-800 bg-red-950'
                    : 'border-yellow-800 bg-yellow-950'}">

                  <div class="flex items-center gap-2 font-sans mb-1">
                    <span class="font-semibold
                      {result.status === 'pass'  ? 'text-green-400'  :
                       result.status === 'fail'  ? 'text-red-400'    : 'text-yellow-400'}">
                      {result.status === 'pass' ? '✓' : result.status === 'fail' ? '✗' : '⚠'}
                      Case {result.case}
                    </span>
                    <span class="text-gray-500 text-xs capitalize">{result.status}</span>
                  </div>

                  <div class="text-gray-300"><span class="text-gray-500">input:    </span>{result.input}</div>
                  <div class="text-gray-300"><span class="text-gray-500">expected: </span>{result.expected}</div>
                  <div class="{result.status === 'pass' ? 'text-green-300' : 'text-red-300'}">
                    <span class="text-gray-500">got:      </span>{result.got}
                  </div>
                  {#if result.stdout}
                    <div class="mt-1 pt-1.5 border-t border-gray-700 text-gray-400">
                      <span class="text-gray-600">stdout: </span>{result.stdout}
                    </div>
                  {/if}
                </div>
              {/each}

              <!-- Summary footer -->
              <div class="pt-2 border-t border-gray-800 font-sans">
                {#if allPassed}
                  <p class="text-green-400 text-xs font-semibold">✓ All test cases passed — ready to save.</p>
                {:else}
                  <p class="text-red-400 text-xs font-semibold">{failCount} test case{failCount !== 1 ? 's' : ''} failing — fix before saving.</p>
                {/if}
              </div>
            {/if}
          </div>
        </ScrollArea>
      </Resizable.Pane>

    </Resizable.PaneGroup>
  </div>
</div>