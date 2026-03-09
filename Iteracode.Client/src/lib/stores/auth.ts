import { writable, derived, get } from 'svelte/store';

const accessToken = writable<string | null>(null);

export const isAuthenticated = derived(
    accessToken,
    ($token) => !!$token
);

export const setAccessToken = (token: string | null) => accessToken.set(token);
export const clearAccessToken = () => accessToken.set(null);
export const getAccessToken = () => get(accessToken);