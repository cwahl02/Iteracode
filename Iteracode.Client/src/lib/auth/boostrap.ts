import { refreshSession } from '$lib/api/auth';

export async function bootstrapAuth() {
    try {
        await refreshSession();
        return true;
    } catch {
        return false;
    }
}