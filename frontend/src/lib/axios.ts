import axios, {
  type AxiosError,
  type InternalAxiosRequestConfig,
  type AxiosResponse,
} from "axios";
import { API_BASE_URL, API_TIMEOUT, API_HEADERS } from "../config";
import { STORAGE_KEYS } from "../config";
import { storage } from "./storage";

// Criar instância do axios
export const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
  headers: API_HEADERS,
});

// Request interceptor - adiciona token de autenticação
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = storage.get<string>(STORAGE_KEYS.accessToken);

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  },
);

// Response interceptor - trata erros e refresh token
api.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    // Se receber 401 e não for uma tentativa de retry
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = storage.get<string>(STORAGE_KEYS.refreshToken);

      // Se não há refresh token, fazer logout diretamente
      if (!refreshToken) {
        handleLogout();
        return Promise.reject(error);
      }

      // TODO: Implementar refresh token quando endpoint estiver disponível
      // Por enquanto, apenas faz logout
      handleLogout();
    }

    return Promise.reject(error);
  },
);

/**
 * Limpa tokens e redireciona para login
 */
function handleLogout() {
  // Limpar todos os dados de autenticação do storage
  storage.remove(STORAGE_KEYS.accessToken);
  storage.remove(STORAGE_KEYS.refreshToken);
  storage.remove(STORAGE_KEYS.user);

  // Redirecionar para login se não estiver na página de auth
  if (typeof window !== "undefined") {
    const isAuthPage = window.location.pathname.includes("/auth/");
    if (!isAuthPage) {
      window.location.href = "/auth/login?expired=true";
    }
  }
}

export default api;
