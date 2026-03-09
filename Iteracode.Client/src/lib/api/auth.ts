import { apiFetch } from '../api';
import { getAccessToken, setAccessToken, clearAccessToken } from '$lib/stores/auth';
import type { ApiError } from '$lib/types/auth';

export interface LoginResponse {
    accessToken: string;
}

export interface RegisterResponse {
    email: string;
    username: string;
    password: string;
    confirmPassword: string;
}

export interface RefreshResponse {
    accessToken: string;
}

export async function login(emailOrUsername: string, password: string) {
    try {
        const data = await apiFetch<LoginResponse>('/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ emailOrUsername: emailOrUsername, password: password }),
            skipAuth: true
        });

        setAccessToken(data.accessToken);
    } catch (err) {
        const apiError = err as ApiError;
    }
}

export async function logout() {
    try {
        await apiFetch('/auth/logout',{
            method: 'POST'
        });
    } finally {
        clearAccessToken();
    }
}

export async function register(email: string, username: string, password: string, confirmPassword: string) {
    try {
        await apiFetch<RegisterResponse>('/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, username, password, confirmPassword }),
            skipAuth: true
        });
    } catch (err) {
        const apiError = err as ApiError;
    }
}

export async function refreshSession() {
    const data = await apiFetch<RefreshResponse>('/auth/refresh', {
        method: 'POST'
    });
    
    setAccessToken(data.accessToken);
}