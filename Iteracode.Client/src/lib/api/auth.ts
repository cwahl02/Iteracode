import { auth } from "$lib/stores/auth.svelte";
import { apiFetch } from "./client";

export type LoginRequest = {
  emailOrUsername: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
};

export const authApi = {
  async login(body: LoginRequest): Promise<void> {
    const res = await fetch("/api/auth/login", {
      method:      "POST",
      credentials: "include", // receive refresh token cookie
      headers:     { "Content-Type": "application/json" },
      body:        JSON.stringify(body),
    });
    if (!res.ok) {
      const { message } = await res.json().catch(() => ({ message: "Login failed" }));
      throw new Error(message);
    }
    const { accessToken }: LoginResponse = await res.json();
    auth.accessToken = accessToken;
  },

  async logout(): Promise<void> {
    await apiFetch("/api/auth/logout", { method: "POST" }).catch(() => {});
    auth.clear();
  },

  // Call on app mount to silently reacquire access token from refresh cookie
  async hydrate(): Promise<void> {
    try {
      const res = await fetch("/api/auth/refresh", {
        method:      "POST",
        credentials: "include",
      });
      if (!res.ok) return;
      const { accessToken }: LoginResponse = await res.json();
      auth.accessToken = accessToken;
    } catch {
      // no valid session, stay logged out
    }
  },
};