<script lang="ts">
  import { modal } from "$lib/stores/authModal.svelte";
  import { authApi } from "$lib/api/auth";

  let email           = $state("");
  let username        = $state("");
  let password        = $state("");
  let confirmPassword = $state("");
  let loading         = $state(false);
  let errors          = $state<Record<string, string>>({});

  function validate() {
    const e: Record<string, string> = {};
    if (!email.trim())           e.email           = "Email is required";
    else if (!/\S+@\S+\.\S+/.test(email)) e.email  = "Enter a valid email";
    if (!username.trim())        e.username         = "Username is required";
    if (!password)               e.password         = "Password is required";
    else if (password.length < 8) e.password        = "Password must be at least 8 characters";
    if (!confirmPassword)        e.confirmPassword  = "Please confirm your password";
    else if (password !== confirmPassword) e.confirmPassword = "Passwords do not match";
    return e;
  }

  async function handleSubmit(e: SubmitEvent) {
    e.preventDefault();
    errors = validate();
    if (Object.keys(errors).length) return;

    loading = true;
    try {
      await authApi.register(email, username, password, confirmPassword);
      modal.close();
    } catch (err: any) {
      const messages: string[] = err?.errors ?? ["Something went wrong"];
      // try to map backend errors to fields
      const mapped: Record<string, string> = {};
      for (const msg of messages) {
        const lower = msg.toLowerCase();
        if (lower.includes("email"))    mapped.email    = msg;
        else if (lower.includes("username") || lower.includes("user name")) mapped.username = msg;
        else if (lower.includes("password")) mapped.password = msg;
        else mapped.form = msg;
      }
      errors = mapped;
    } finally {
      loading = false;
    }
  }
</script>

<div class="px-6 py-8 flex flex-col gap-6">
  <!-- Header -->
  <div>
    <h2 class="text-xl font-bold text-gray-900">Create an account</h2>
    <p class="text-sm text-gray-400 mt-0.5">Join Iteracode today</p>
  </div>

  <!-- Form -->
  <form onsubmit={handleSubmit} class="flex flex-col gap-4">

    {#if errors.form}
      <p class="text-xs text-red-500 bg-red-50 px-3 py-2 rounded">{errors.form}</p>
    {/if}

    <!-- Email -->
    <div class="flex flex-col gap-1">
      <label for="email" class="text-xs font-medium text-gray-600">Email</label>
      <input
        id="email"
        bind:value={email}
        type="email"
        autocomplete="email"
        placeholder="you@example.com"
        class="w-full rounded-md border px-3 py-2 text-sm outline-none
          transition ring-offset-1 focus:ring-2
          {errors.email
            ? 'border-red-300 focus:ring-red-300'
            : 'border-gray-200 focus:ring-orange-400'}"
      />
      {#if errors.email}
        <p class="text-xs text-red-500">{errors.email}</p>
      {/if}
    </div>

    <!-- Username -->
    <div class="flex flex-col gap-1">
      <label for="username" class="text-xs font-medium text-gray-600">Username</label>
      <input
        id="username"
        bind:value={username}
        type="text"
        autocomplete="username"
        placeholder="coolcoder42"
        class="w-full rounded-md border px-3 py-2 text-sm outline-none
          transition ring-offset-1 focus:ring-2
          {errors.username
            ? 'border-red-300 focus:ring-red-300'
            : 'border-gray-200 focus:ring-orange-400'}"
      />
      {#if errors.username}
        <p class="text-xs text-red-500">{errors.username}</p>
      {/if}
    </div>

    <!-- Password -->
    <div class="flex flex-col gap-1">
      <label for="reg-pw" class="text-xs font-medium text-gray-600">Password</label>
      <input
        id="reg-pw"
        bind:value={password}
        type="password"
        autocomplete="new-password"
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

    <!-- Confirm password -->
    <div class="flex flex-col gap-1">
      <label for="confirm-pw" class="text-xs font-medium text-gray-600">
        Confirm password
      </label>
      <input
        id="confirm-pw"
        bind:value={confirmPassword}
        type="password"
        autocomplete="new-password"
        placeholder="••••••••"
        class="w-full rounded-md border px-3 py-2 text-sm outline-none
          transition ring-offset-1 focus:ring-2
          {errors.confirmPassword
            ? 'border-red-300 focus:ring-red-300'
            : 'border-gray-200 focus:ring-orange-400'}"
      />
      {#if errors.confirmPassword}
        <p class="text-xs text-red-500">{errors.confirmPassword}</p>
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
      {loading ? "Creating account…" : "Create account"}
    </button>
  </form>

  <!-- Switch to login -->
  <p class="text-center text-xs text-gray-400">
    Already have an account?
    <button
      onclick={() => modal.openLogin()}
      class="text-orange-500 font-medium hover:underline"
    >
      Log in
    </button>
  </p>
</div>