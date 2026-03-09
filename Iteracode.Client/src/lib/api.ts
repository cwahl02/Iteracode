import { PUBLIC_API_URL } from '$env/static/public';
import { getAccessToken, setAccessToken, clearAccessToken } from './stores/auth';
import { goto } from '$app/navigation';

const API_URL = PUBLIC_API_URL;

interface FetchOptions extends RequestInit {
    skipAuth?: boolean;
}

async function tryRefresh(): Promise<boolean> {
    try {
        const response = await fetch(`${API_URL}/auth/refresh`, {
            method: 'POST',
            credentials: 'include'
        });

        if(!response.ok) return false;

        const data = await response.json();
        setAccessToken(data.accessToken);
        return true;
    } catch {
        return false;
    }
}

export async function apiFetch<T>(
    path: string,
    options: FetchOptions = {}
): Promise<T> {
    const { skipAuth = false, ...fetchOptions } = options;
    const headers = new Headers(fetchOptions.headers || {});

    if(!skipAuth) {
        const token = getAccessToken();
        if(token) {
            headers.set('Authorization', `Bearer ${token}`);
        }
    }

    // Always include credentials for refresh token handling
    const response = await fetch(`${API_URL}${path}`, {
        ...fetchOptions,
        headers,
        credentials: 'include'
    });

    // Silent refresh on 401
    if(response.status === 401 && !skipAuth) {
        const refreshed = await tryRefresh();

        if(refreshed) {
            const retryHeaders = new Headers(fetchOptions.headers || {});
            const newToken = getAccessToken();
            if(newToken) retryHeaders.set('Authorization', `Bearer ${newToken}`);

            const retryResponse = await fetch(`${API_URL}${path}`, {
                ...fetchOptions,
                headers: retryHeaders,
                credentials: 'include'
            });

            if(!retryResponse.ok) {
                const error = await retryResponse.json().catch(() => ({ errors: ['An unexpected error occurred'] }));
                throw error;
            }

            return retryResponse.json() as Promise<T>;
        }

        // Refresh failed, user needs to login again
        clearAccessToken();
        goto('/login');
        throw { errors: ['Session expired. Please log in again.'] };
    }

    if(!response.ok) {
        const error = await response.json().catch(() => ({ errors: ['An unexpected error occurred'] }));
        throw error;
    }

    // Handle 204 No Content
    if(response.status === 204) return {} as T; // No content

    return response.json() as Promise<T>;
}