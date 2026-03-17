<script lang="ts">
  import { modal } from "$lib/stores/authModal.svelte";
  import { authApi, type LoginRequest } from "$lib/api/auth";

  let emailOrUsername = $state("");
  let password        = $state("");
  let loading         = $state(false);
  let errors          = $state<Record<string, string>>({});

  function validate() {
    const e: Record<string, string> = {};
    if (!emailOrUsername.trim()) e.emailOrUsername = "Email or username is required";
    if (!password)               e.password        = "Password is required";
    return e;
  }

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    errors = validate();
    if (Object.keys(errors).length) return;

    loading = true;
    try {
      const body: LoginRequest = { emailOrUsername, password };
      await authApi.login(body);
      modal.close();
    } catch (err: any) {
      // API returns { errors: string[] }
      const messages: string[] = err?.errors ?? ["Something went wrong"];
      errors = { form: messages[0] };
    } finally {
      loading = false;
    }
  }
</script>

<div class="px-6 py-8 flex flex-col gap-6">
  <!-- Header -->
  <div>
    <h2 class="text-xl font-bold text-gray-900">Welcome back</h2>
    <p class="text-sm text-gray-400 mt-0.5">Good to see you again</p>
  </div>

  <!-- Form -->
  <form onsubmit={handleSubmit} class="flex flex-col gap-4">

    <!-- Form-level error -->
    {#if errors.form}
      <p class="text-xs text-red-500 bg-red-50 px-3 py-2 rounded">{errors.form}</p>
    {/if}

    <!-- Email or username -->
    <div class="flex flex-col gap-1">
      <label for="eou" class="text-xs font-medium text-gray-600">
        Email or username
      </label>
      <input
        id="eou"
        bind:value={emailOrUsername}
        type="text"
        autocomplete="username"
        placeholder="you@example.com"
        class="w-full rounded-md border px-3 py-2 text-sm outline-none
          transition ring-offset-1 focus:ring-2
          {errors.emailOrUsername
            ? 'border-red-300 focus:ring-red-300'
            : 'border-gray-200 focus:ring-orange-400'}"
      />
      {#if errors.emailOrUsername}
        <p class="text-xs text-red-500">{errors.emailOrUsername}</p>
      {/if}
    </div>

    <!-- Password -->
    <div class="flex flex-col gap-1">
      <label for="pw" class="text-xs font-medium text-gray-600">Password</label>
      <input
        id="pw"
        bind:value={password}
        type="password"
        autocomplete="current-password"
        placeholder="••••••••"
        class="w-full rounded-md border px-3 py-2 text-sm outline-none
          transition ring-offset-1 focus:ring-2
          {errors.password
            ? 'border-red-300 focus:ring-red-300'
            : 'border-gray-200 focus:ring-orange-400'}"
      />
      {#if errors.password}
        <p class="text-xs text-red-500">{errors.password}</p>
      {/if}
    </div>

    <!-- Submit -->
    <button
      type="submit"
      disabled={loading}
      class="w-full rounded-md bg-orange-500 py-2 text-sm font-medium
        text-white transition hover:bg-orange-600 disabled:opacity-50
        disabled:cursor-not-allowed mt-1"
    >
      {loading ? "Logging in…" : "Log in"}
    </button>
  </form>

  <!-- Switch to register -->
  <p class="text-center text-xs text-gray-400">
    Don't have an account?
    <button
      onclick={() => modal.openRegister()}
      class="text-orange-500 font-medium hover:underline"
    >
      Sign up
    </button>
  </p>
</div>