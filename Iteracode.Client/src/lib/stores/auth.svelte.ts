function createAuthStore() {
  let accessToken = $state<string | null>(null);

  return {
    get accessToken() { return accessToken; },
    set accessToken(v: string | null) { accessToken = v; },
    clear() { accessToken = null; },
    get isAuthenticated() { return accessToken !== null; },
  };
}

export const auth = createAuthStore();