import { QueryClient, type DefaultOptions } from "@tanstack/react-query";

const defaultQueryOptions: DefaultOptions = {
  queries: {
    // Tempo que os dados ficam "frescos" (não refetch automático)
    staleTime: 5 * 60 * 1000, // 5 minutos

    // Tempo que os dados ficam em cache após não serem mais usados
    gcTime: 10 * 60 * 1000, // 10 minutos (anteriormente cacheTime)

    // Número de tentativas em caso de erro
    retry: (failureCount, error) => {
      // Não tentar novamente para erros 4xx
      if (error instanceof Error && "status" in error) {
        const status = (error as { status: number }).status;
        if (status >= 400 && status < 500) {
          return false;
        }
      }
      return failureCount < 3;
    },

    // Intervalo entre tentativas (exponential backoff)
    retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000),

    // Refetch quando a janela ganha foco
    refetchOnWindowFocus: false,

    // Refetch quando reconecta à internet
    refetchOnReconnect: true,

    // Não refetch automaticamente em mount
    refetchOnMount: true,
  },

  mutations: {
    // Número de tentativas para mutations
    retry: (failureCount, error) => {
      // Não tentar novamente para erros 4xx
      if (error instanceof Error && "status" in error) {
        const status = (error as { status: number }).status;
        if (status >= 400 && status < 500) {
          return false;
        }
      }
      return failureCount < 2;
    },
  },
};

export const queryClient = new QueryClient({
  defaultOptions: defaultQueryOptions,
});

export const clearQueryCache = () => {
  queryClient.clear();
};

export default queryClient;

// export const invalidateQueries = {
//   profile: () => queryClient.invalidateQueries({ queryKey: ["profile"] }),
//   user: () => queryClient.invalidateQueries({ queryKey: ["currentUser"] }),
//   cart: () => queryClient.invalidateQueries({ queryKey: ["cart"] }),
//   orders: () => queryClient.invalidateQueries({ queryKey: ["orders"] }),
//   favorites: () => queryClient.invalidateQueries({ queryKey: ["favorites"] }),
//   notifications: () =>
//     queryClient.invalidateQueries({ queryKey: ["notifications"] }),
//   addresses: () => queryClient.invalidateQueries({ queryKey: ["addresses"] }),
//   all: () => queryClient.invalidateQueries(),
// };
