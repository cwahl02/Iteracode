<script lang="ts">
  import { onMount } from "svelte";
  import { EditorState } from "@codemirror/state";
  import { EditorView, basicSetup } from "codemirror";
  import { javascript } from "@codemirror/lang-javascript";
  import { python } from "@codemirror/lang-python";
  import { cpp } from "@codemirror/lang-cpp";
  import { java } from "@codemirror/lang-java";
  import { json } from "@codemirror/lang-json";

  let {
    value = $bindable(""),
    language = "python",
    onChange = null,
  }: {
    value?: string;
    language?: string;
    onChange?: ((val: string) => void) | null;
  } = $props();

  let editorDiv: HTMLDivElement;
  let view: EditorView;

  function getLanguageExtension(lang: string) {
    switch (lang) {
      case "javascript": return javascript();
      case "cpp":        return cpp();
      case "java":       return java();
      case "python":
      default:           return python();
    }
  }

  function buildState(doc: string, lang: string) {
    return EditorState.create({
      doc,
      extensions: [
        basicSetup,
        getLanguageExtension(lang),
        EditorView.theme({
          "&": { height: "100%", fontSize: "14px" },
          ".cm-scroller": { overflow: "auto", fontFamily: "'JetBrains Mono', 'Fira Mono', monospace" },
          ".cm-content": { padding: "12px 0" },
        }),
        EditorView.updateListener.of((update) => {
          if (update.docChanged) {
            const newVal = view.state.doc.toString();
            value = newVal;
            onChange?.(newVal);
          }
        }),
      ],
    });
  }

  onMount(() => {
    view = new EditorView({
      state: buildState(value, language),
      parent: editorDiv,
    });

    return () => view?.destroy();
  });

  // Rebuild editor when language changes
  $effect(() => {
    if (view && language) {
      const currentDoc = view.state.doc.toString();
      view.setState(buildState(currentDoc, language));
    }
  });

  // Sync external value changes into the editor
  $effect(() => {
    if (view && value !== view.state.doc.toString()) {
      view.dispatch({
        changes: { from: 0, to: view.state.doc.length, insert: value },
      });
    }
  });
</script>

<div bind:this={editorDiv} class="h-full w-full overflow-hidden" />