// Base URL da API - usar variável de ambiente
export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5055";

// Timeout padrão para requisições (em ms)
export const API_TIMEOUT = 30000;

// Headers padrão
export const API_HEADERS = {
  "Content-Type": "application/json",
  Accept: "application/json",
} as const;

/**
 * Endpoints da API organizados por módulo
 */
export const API_ENDPOINTS = {
  // Auth
  auth: {
    login: "/api/auth/login",
    register: "/api/auth/register",
    confirmEmail: "/api/auth/confirm-email",
    resendConfirmationEmail: "/api/auth/resend-confirmation-email",
    forgotPassword: "/api/auth/forgot-password",
    resetPassword: "/api/auth/reset-password",
    changePassword: "/api/auth/change-password",
  },

  // // Profile
  // profile: {
  //   me: "/api/profile",
  //   byUserId: (userId: string) => `/api/profile/${userId}`,
  // },

  // // User Addresses (authenticated)
  // addresses: {
  //   base: "/api/addresses",
  //   byId: (id: string) => `/api/addresses/${id}`,
  //   setDefault: (id: string) => `/api/addresses/${id}/default`,
  //   setBilling: (id: string) => `/api/addresses/${id}/billing`,
  //   delete: (id: string) => `/api/addresses/${id}`,
  // },

  // // User (futuros endpoints)
  // user: {
  //   sessions: "/api/user/sessions",
  //   notifications: "/api/user/notifications",
  //   preferences: "/api/user/preferences",
  // },

  // // Catalog (futuros endpoints)
  // catalog: {
  //   categories: "/api/catalog/categories",
  //   brands: "/api/catalog/brands",
  //   products: "/api/catalog/products",
  //   product: (id: string) => `/api/catalog/products/${id}`,
  //   productBySlug: (slug: string) => `/api/catalog/products/slug/${slug}`,
  //   reviews: (productId: string) =>
  //     `/api/catalog/products/${productId}/reviews`,
  //   favorites: "/api/catalog/favorites",
  // },

  // // Cart (futuros endpoints)
  // cart: {
  //   current: "/api/cart",
  //   items: "/api/cart/items",
  //   item: (itemId: string) => `/api/cart/items/${itemId}`,
  //   applyCoupon: "/api/cart/coupon",
  //   removeCoupon: "/api/cart/coupon",
  // },

  // // Order (futuros endpoints)
  // order: {
  //   list: "/api/orders",
  //   byId: (id: string) => `/api/orders/${id}`,
  //   byNumber: (number: string) => `/api/orders/number/${number}`,
  //   cancel: (id: string) => `/api/orders/${id}/cancel`,
  //   tracking: (id: string) => `/api/orders/${id}/tracking`,
  // },

  // // Payment (futuros endpoints)
  // payment: {
  //   process: "/api/payments",
  //   byId: (id: string) => `/api/payments/${id}`,
  //   methods: "/api/payments/methods",
  // },

  // // Coupon (futuros endpoints)
  // coupon: {
  //   validate: "/api/coupons/validate",
  //   public: "/api/coupons/public",
  // },
} as const;
