/**
 * Configuração das rotas da aplicação
 * Todas as rotas centralizadas para evitar strings hardcoded
 */

export const ROUTES = {
  // Home
  home: "/",

  // Auth
  auth: {
    login: "/login",
    register: "/register",
    forgotPassword: "/forgot-password",
    resetPassword: "/reset-password",
    confirmEmail: "/confirm-email",
  },

  // Error pages
  error: {
    notFound: "/404",
    serverError: "/500",
  },
};
