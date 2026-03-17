<script lang="ts">
  import { page } from "$app/stores";
  import { auth } from "$lib/stores/auth.svelte";
  import { modal } from "$lib/stores/authModal.svelte";
  import { authApi } from "$lib/api/auth";
  import { goto } from "$app/navigation";
  import { Button } from "$lib/components/ui/button";

  const isActive = (href: string) =>
    $page.url.pathname === href ||
    ($page.url.pathname.startsWith(href) && href !== "/");

  async function handleLogout() {
    await authApi.logout();
    goto("/");
  }

  const publicLinks = [{ href: "/problems", label: "Problems" }];
  const adminLinks  = [{ href: "/admin",    label: "Admin"    }];
</script>

<nav class="sticky top-0 z-50 w-full border-b border-gray-100 bg-white/90 backdrop-blur-sm">
  <div class="mx-auto flex h-14 max-w-6xl items-center justify-between px-4">

    <a href="/" class="flex items-center gap-2 font-bold text-gray-900 hover:opacity-80 transition-opacity">
      <span class="text-orange-500 text-lg">{"<>"}</span>
      <span>Iteracode</span>
    </a>

    <div class="flex items-center gap-1">
      {#each publicLinks as link}
        <a href={link.href}
          class="px-3 py-1.5 rounded text-sm transition-colors
            {isActive(link.href)
              ? 'text-orange-600 font-medium bg-orange-50'
              : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'}">
          {link.label}
        </a>
      {/each}

      {#if auth.isAuthenticated}
        {#each adminLinks as link}
          <a href={link.href}
            class="px-3 py-1.5 rounded text-sm transition-colors
              {isActive(link.href)
                ? 'text-orange-600 font-medium bg-orange-50'
                : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'}">
            {link.label}
          </a>
        {/each}
      {/if}
    </div>

    <div class="flex items-center gap-2">
      {#if auth.isAuthenticated}
        <button
          onclick={handleLogout}
          class="px-3 py-1.5 rounded text-sm text-gray-600
            hover:text-gray-900 hover:bg-gray-100 transition-colors"
        >
          Log out
        </button>
      {:else}
        <Button
          onclick={() => modal.openLogin()}
          class="px-3 py-1.5 rounded text-sm text-gray-600
            hover:text-gray-900 hover:bg-gray-100 transition-colors"
        >
          Log in
        </Button>
        <Button
          onclick={() => modal.openRegister()}
          class="px-3 py-1.5 rounded text-sm font-medium
            bg-orange-500 text-white hover:bg-orange-600 transition-colors"
        >
          Sign up
        </Button>
      {/if}
    </div>

  </div>
</nav>