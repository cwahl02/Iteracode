import { auth } from "$lib/stores/auth.svelte";
import { toast } from "svelte-sonner";

import { PUBLIC_API_URL } from '$env/static/public';

const MAX_RETRIES = 1;

// silent refresh — uses HttpOnly refresh token cookie
async function tryRefreshAuth(): Promise<boolean> {
  try {
    const res = await fetch(`${PUBLIC_API_URL}/auth/refresh`, {
      method:      "POST",
      credentials: "include",
    });
    if (!res.ok) return false;
    const { accessToken } = await res.json();
    auth.accessToken = accessToken;
    return true;
  } catch {
    return false;
  }
}

export async function apiFetch(
  input: RequestInfo,
  init?: RequestInit,
  retries = MAX_RETRIES
): Promise<Response> {
  const headers = new Headers(init?.headers);

  if (auth.accessToken) {
    headers.set("Authorization", `Bearer ${auth.accessToken}`);
  }

  const res = await fetch(`${PUBLIC_API_URL}${input}`, {
    ...init,
    credentials: "include",
    headers,
  });

  if (res.status === 401 && retries > 0) {
    const refreshed = await tryRefreshAuth();
    if (refreshed) {
      return apiFetch(input, init, retries - 1);
    } else {
      auth.clear();
      toast.error("Session expired — please log in again");
      window.location.href = "/login";
      return res;
    }
  }

  return res;
}