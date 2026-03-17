<script lang="ts">
  import * as Resizable from "$lib/components/ui/resizable";
  import * as Select from "$lib/components/ui/select";
  import { Button } from "$lib/components/ui/button";
  import { ScrollArea } from "$lib/components/ui/scroll-area";
  import { Separator } from "$lib/components/ui/separator";
  import { Badge } from "$lib/components/ui/badge";
  import CodeMirrorEditor from "$lib/components/CodeMirrorEditor.svelte";

  let language = "python";
  let code = `# Write your code here\nprint("Hello, World!")`;
  let output = "";
  let isRunning = false;
  let hasRun = false;

  const languages = [
    { value: "python",     label: "Python" },
    { value: "javascript", label: "JavaScript" },
    { value: "cpp",        label: "C++" },
    { value: "java",       label: "Java" },
  ];

  async function handleRun() {
    isRunning = true;
    hasRun = true;
    // TODO: wire up API
    await new Promise((r) => setTimeout(r, 600));
    output = "// Output will appear here once the API is connected.";
    isRunning = false;
  }

  function handleClear() {
    output = "";
    hasRun = false;
  }
</script>

<svelte:head>
  <title>Playground — MiniLeet</title>
</svelte:head>

<div class="h-screen flex flex-col bg-white overflow-hidden">
  <!-- Top bar -->
  <header class="border-b border-gray-200 px-4 py-2 flex items-center justify-between shrink-0">
    <div class="flex items-center gap-2">
      <a href="/" class="text-sm font-bold tracking-tight text-gray-900 hover:text-orange-500 transition-colors">MiniLeet</a>
      <span class="text-gray-300">/</span>
      <span class="text-sm text-gray-500">Playground</span>
    </div>
    <a href="/problems" class="text-xs text-gray-400 hover:text-gray-600 transition-colors">← Problems</a>
  </header>

  <!-- Main resizable area -->
  <div class="flex-1 overflow-hidden">
    <Resizable.PaneGroup direction="horizontal" class="h-full">

      <!-- Left: Editor -->
      <Resizable.Pane defaultSize={60} minSize={30} class="flex flex-col">
        <!-- Editor Toolbar -->
        <div class="flex items-center gap-2 px-3 py-2 border-b border-gray-200 bg-gray-50 shrink-0">
          <span class="text-xs font-medium text-gray-500 mr-1">Language</span>
          <Select.Root type="single" value={language} onValueChange={(val) => (language = val)}>
            <Select.Trigger class="h-7 w-36 text-xs">

            </Select.Trigger>
            <Select.Content>
              {#each languages as lang}
                <Select.Item value={lang.value}>{lang.label}</Select.Item>
              {/each}
            </Select.Content>
          </Select.Root>

          <div class="flex-1" />
          
          <Button
            size="sm"
            class="h-7 text-xs gap-1 bg-green-600 hover:bg-green-700 text-white"
            onclick={handleRun}
            disabled={isRunning}
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
              Run
            {/if}
          </Button>
        </div>

        <!-- Editor -->
        <div class="flex-1 overflow-hidden">
          <CodeMirrorEditor bind:value={code} {language} onChange={(v) => (code = v)} />
        </div>
      </Resizable.Pane>

      <Resizable.Handle withHandle />

      <!-- Right: Console -->
      <Resizable.Pane defaultSize={40} minSize={20} class="flex flex-col">
        <!-- Console Toolbar -->
        <div class="flex items-center gap-2 px-3 py-2 border-b border-gray-200 bg-gray-50 shrink-0">
          <span class="text-xs font-semibold text-gray-600">Console</span>
          {#if hasRun}
            <Badge variant="secondary" class="text-xs h-4">Output</Badge>
          {/if}
          <div class="flex-1" />
          <Button
            variant="ghost"
            size="sm"
            class="h-7 text-xs text-gray-500 hover:text-red-500"
            onclick={handleClear}
            disabled={!hasRun}
          >
            Clear
          </Button>
        </div>

        <!-- Output -->
        <ScrollArea class="flex-1">
          <div class="p-4 font-mono text-sm">
            {#if !hasRun}
              <p class="text-gray-500 text-xs">Run your code to see output here.</p>
            {:else if output}
              <pre class="text-green-400 whitespace-pre-wrap break-words">{output}</pre>
            {:else}
              <p class="text-gray-500 text-xs">No output.</p>
            {/if}
          </div>
        </ScrollArea>
      </Resizable.Pane>

    </Resizable.PaneGroup>
  </div>
</div>