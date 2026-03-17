<script lang="ts">
  import * as Dialog from "$lib/components/ui/dialog";
  import { Button } from "$lib/components/ui/button";

  let {
    open = $bindable(false),
    mode,
    itemName,
    onconfirm,
    oncancel,
  }: {
    open: boolean;
    mode: "confirm" | "named";   // confirm = file, named = permanent folder delete
    itemName: string;
    onconfirm: () => void;
    oncancel: () => void;
  } = $props();

  let typedName = $state("");
  const nameMatches = $derived(typedName.trim() === itemName);

  function handleConfirm() {
    if (mode === "named" && !nameMatches) return;
    open = false;
    typedName = "";
    onconfirm();
  }

  function handleCancel() {
    open = false;
    typedName = "";
    oncancel();
  }
</script>

<Dialog.Root bind:open>
  <Dialog.Content class="max-w-md">
    <Dialog.Header>
      <Dialog.Title class="text-gray-900">
        {mode === "named" ? "Permanently delete" : "Delete"} "{itemName}"?
      </Dialog.Title>
      <Dialog.Description class="text-sm text-gray-500">
        {#if mode === "named"}
          This cannot be undone. The folder and all its contents will be
          wiped from disk permanently.
        {:else}
          This file will be deleted immediately.
        {/if}
      </Dialog.Description>
    </Dialog.Header>

    {#if mode === "named"}
      <div class="mt-2 flex flex-col gap-2">
        <p class="text-xs text-gray-500">
          Type <span class="font-mono font-semibold text-gray-800">{itemName}</span> to confirm
        </p>
        <input
          bind:value={typedName}
          autofocus
          placeholder={itemName}
          class="w-full rounded border border-gray-200 px-3 py-1.5 text-sm
            outline-none ring-offset-1 focus:ring-2 focus:ring-red-400
            {nameMatches ? 'border-red-400' : ''}"
          onkeydown={(e) => {
            if (e.key === "Enter" && nameMatches) handleConfirm();
            if (e.key === "Escape") handleCancel();
          }}
        />
      </div>
    {/if}

    <Dialog.Footer class="mt-4 flex justify-end gap-2">
      <Button variant="outline" size="sm" onclick={handleCancel}>
        Cancel
      </Button>
      <Button
        variant="destructive"
        size="sm"
        disabled={mode === "named" && !nameMatches}
        onclick={handleConfirm}
      >
        {mode === "named" ? "Delete forever" : "Delete"}
      </Button>
    </Dialog.Footer>
  </Dialog.Content>
</Dialog.Root>