/**
 * Constantes globais da aplicação
 */

// Storage keys
export const STORAGE_KEYS = {
  accessToken: "bcommerce_access_token",
  refreshToken: "bcommerce_refresh_token",
  user: "bcommerce_user",
  theme: "bcommerce_theme",
  cart: "bcommerce_cart",
  recentSearches: "bcommerce_recent_searches",
  cookieConsent: "bcommerce_cookie_consent",
} as const;

// Query keys para React Query
export const QUERY_KEYS = {
  // Auth
  currentUser: ["currentUser"],

  // Profile
  profile: ["profile"],
  profileById: (userId: string) => ["profile", userId],

  // User
  addresses: ["addresses"],
  sessions: ["sessions"],
  notifications: ["notifications"],
  notificationPreferences: ["notificationPreferences"],
  unreadNotificationsCount: ["unreadNotificationsCount"],

  // Catalog
  categories: ["categories"],
  categoryTree: ["categoryTree"],
  brands: ["brands"],
  products: (params?: Record<string, unknown>) => ["products", params],
  product: (id: string) => ["product", id],
  productBySlug: (slug: string) => ["productBySlug", slug],
  productReviews: (productId: string) => ["productReviews", productId],
  favorites: ["favorites"],
  isFavorite: (productId: string) => ["isFavorite", productId],

  // Cart
  cart: ["cart"],
  cartSummary: ["cartSummary"],
  cartItemCount: ["cartItemCount"],
  savedCarts: ["savedCarts"],

  // Order
  orders: (params?: Record<string, unknown>) => ["orders", params],
  order: (id: string) => ["order", id],
  orderByNumber: (number: string) => ["orderByNumber", number],
  orderTracking: (orderId: string) => ["orderTracking", orderId],

  // Payment
  payment: (id: string) => ["payment", id],
  paymentMethods: ["paymentMethods"],

  // Coupon
  publicCoupons: ["publicCoupons"],
} as const;

// Paginação padrão
export const DEFAULT_PAGINATION = {
  pageNumber: 1,
  pageSize: 12,
  maxPageSize: 100,
} as const;

// Limites
export const LIMITS = {
  maxCartItems: 50,
  maxAddresses: 10,
  maxSavedCarts: 5,
  maxRecentSearches: 10,
  maxProductImages: 10,
  maxReviewLength: 1000,
  minPasswordLength: 8,
  maxPasswordLength: 128,
} as const;

// Intervalos de tempo (em ms)
export const TIME_INTERVALS = {
  tokenRefreshInterval: 5 * 60 * 1000, // 5 minutos
  notificationPollInterval: 30 * 1000, // 30 segundos
  cartSyncInterval: 60 * 1000, // 1 minuto
  searchDebounce: 300, // 300ms
  toastDuration: 5000, // 5 segundos
  pixExpirationWarning: 5 * 60 * 1000, // 5 minutos antes de expirar
} as const;

// Formatos
export const FORMATS = {
  date: "DD/MM/YYYY",
  dateTime: "DD/MM/YYYY HH:mm",
  time: "HH:mm",
  currency: "pt-BR",
  locale: "pt-BR",
} as const;

// Moeda
export const CURRENCY = {
  code: "BRL",
  symbol: "R$",
  locale: "pt-BR",
} as const;

// Status de pedido
export const ORDER_STATUS = {
  pending: "Pendente",
  confirmed: "Confirmado",
  processing: "Processando",
  shipped: "Enviado",
  delivered: "Entregue",
  cancelled: "Cancelado",
  refunded: "Reembolsado",
} as const;

// Status de pagamento
export const PAYMENT_STATUS = {
  pending: "Pendente",
  processing: "Processando",
  approved: "Aprovado",
  rejected: "Rejeitado",
  refunded: "Reembolsado",
  cancelled: "Cancelado",
} as const;

// Métodos de pagamento
export const PAYMENT_METHODS = {
  creditCard: "Cartão de Crédito",
  debitCard: "Cartão de Débito",
  pix: "PIX",
  boleto: "Boleto Bancário",
} as const;

// Gêneros
export const GENDERS = {
  male: "Masculino",
  female: "Feminino",
  other: "Outro",
  preferNotToSay: "Prefiro não informar",
} as const;
