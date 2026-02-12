import { useEffect, type ReactNode } from 'react';
import { useAuthStore } from '../stores/auth-store';
import { storage } from '../lib/storage';
import { STORAGE_KEYS } from '../config';

interface AuthProviderProps {
    children: ReactNode;
}


/**
 * Provider que inicializa e sincroniza o estado de autenticação
 */
export function AuthProvider({ children }: AuthProviderProps) {
    const setLoading = useAuthStore((state) => state.setLoading);
    const accessToken = useAuthStore((state) => state.accessToken);
    // Sincroniza o token com o storage separado (para o axios interceptor)
    useEffect(() => {
        if (accessToken) {
            storage.set(STORAGE_KEYS.accessToken, accessToken);
        } else {
            storage.remove(STORAGE_KEYS.accessToken);
        }
    }, [accessToken]);

    // Marca como não loading após hidratação
    useEffect(() => {
        // Pequeno delay para garantir hidratação do Zustand
        const timeout = setTimeout(() => {
            setLoading(false);
        }, 100);

        return () => clearTimeout(timeout);
    }, [setLoading]);

    return <>{children}</>;
}
export default AuthProvider;