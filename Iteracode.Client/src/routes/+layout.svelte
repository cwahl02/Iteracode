<script lang="ts">
  import { onMount } from "svelte";
  import Navbar from "$lib/components/Navbar.svelte";
  import { Toaster } from "svelte-sonner";
  import favicon from "$lib/assets/favicon.svg";
  import "./layout.css";

  let { children } = $props();
</script>

<svelte:head>
  <link rel="icon" href={favicon} />
</svelte:head>

<Toaster richColors position="bottom-right" />
<Navbar />

<main>
  {@render children()}
</main>