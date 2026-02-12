/**
 * Wrapper para localStorage/sessionStorage com tipagem e serialização
 */

type StorageType = "local" | "session";

interface StorageOptions {
  type?: StorageType;
  expireIn?: number; // tempo em milissegundos
}

interface StorageItem<T> {
  value: T;
  expireAt?: number;
}

/**
 * Verifica se o storage está disponível
 */
const isStorageAvailable = (type: StorageType): boolean => {
  if (typeof window === "undefined") return false;

  try {
    const storage =
      type === "local" ? window.localStorage : window.sessionStorage;
    const testKey = "__storage_test__";
    storage.setItem(testKey, testKey);
    storage.removeItem(testKey);
    return true;
  } catch {
    return false;
  }
};

/**
 * Obtém a instância do storage
 */
const getStorage = (type: StorageType): Storage | null => {
  if (!isStorageAvailable(type)) return null;
  return type === "local" ? window.localStorage : window.sessionStorage;
};

/**
 * Wrapper para operações de storage
 */
export const storage = {
  /**
   * Armazena um valor
   */
  set<T>(key: string, value: T, options: StorageOptions = {}): boolean {
    const { type = "local", expireIn } = options;
    const storageInstance = getStorage(type);

    if (!storageInstance) return false;

    try {
      const item: StorageItem<T> = {
        value,
        expireAt: expireIn ? Date.now() + expireIn : undefined,
      };

      storageInstance.setItem(key, JSON.stringify(item));
      return true;
    } catch {
      console.warn(`Failed to set item "${key}" in ${type}Storage`);
      return false;
    }
  },

  /**
   * Recupera um valor
   */
  get<T>(key: string, options: StorageOptions = {}): T | null {
    const { type = "local" } = options;
    const storageInstance = getStorage(type);

    if (!storageInstance) return null;

    try {
      const data = storageInstance.getItem(key);

      if (!data) return null;

      const item: StorageItem<T> = JSON.parse(data);

      // Verificar expiração
      if (item.expireAt && Date.now() > item.expireAt) {
        storageInstance.removeItem(key);
        return null;
      }

      return item.value;
    } catch {
      // Se falhar o parse, tentar retornar o valor diretamente (compatibilidade)
      try {
        const data = storageInstance.getItem(key);
        return data as unknown as T;
      } catch {
        console.warn(`Failed to get item "${key}" from ${type}Storage`);
        return null;
      }
    }
  },

  /**
   * Remove um valor
   */
  remove(key: string, options: StorageOptions = {}): boolean {
    const { type = "local" } = options;
    const storageInstance = getStorage(type);

    if (!storageInstance) return false;

    try {
      storageInstance.removeItem(key);
      return true;
    } catch {
      console.warn(`Failed to remove item "${key}" from ${type}Storage`);
      return false;
    }
  },

  /**
   * Limpa todo o storage
   */
  clear(options: StorageOptions = {}): boolean {
    const { type = "local" } = options;
    const storageInstance = getStorage(type);

    if (!storageInstance) return false;

    try {
      storageInstance.clear();
      return true;
    } catch {
      console.warn(`Failed to clear ${type}Storage`);
      return false;
    }
  },

  /**
   * Verifica se uma chave existe
   */
  has(key: string, options: StorageOptions = {}): boolean {
    return this.get(key, options) !== null;
  },

  /**
   * Obtém todas as chaves
   */
  keys(options: StorageOptions = {}): string[] {
    const { type = "local" } = options;
    const storageInstance = getStorage(type);

    if (!storageInstance) return [];

    try {
      return Object.keys(storageInstance);
    } catch {
      return [];
    }
  },
};

/**
 * Atalhos para sessionStorage
 */
export const sessionStorage = {
  set: <T>(key: string, value: T, expireIn?: number) =>
    storage.set(key, value, { type: "session", expireIn }),
  get: <T>(key: string) => storage.get<T>(key, { type: "session" }),
  remove: (key: string) => storage.remove(key, { type: "session" }),
  clear: () => storage.clear({ type: "session" }),
  has: (key: string) => storage.has(key, { type: "session" }),
  keys: () => storage.keys({ type: "session" }),
};

export default storage;
