<script lang="ts">

  import LoginForm from "./LoginForm.svelte";
  import RegisterForm from "./RegisterForm.svelte";
  import * as Dialog from "$lib/components/ui/dialog";
</script>

<Dialog.Root
  open={modal.open}
  onOpenChange={(o) => { if (!o) modal.close(); }}
>
  <Dialog.Content class="sm:max-w-sm p-0 overflow-hidden gap-0">
    {#if modal.view === "login"}
      <LoginForm />
    {:else if modal.view === "register"}
      <RegisterForm />
    {/if}
  </Dialog.Content>
</Dialog.Root>