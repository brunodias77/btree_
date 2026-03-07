import { Order, Product, Category, Customer, Coupon, Payment, Notification, Stat, SystemLog } from '../models';

export const STATS: Stat[] = [
    { label: 'RECEITA TOTAL', value: 'R$ 124.500,00', change: '+12%', icon: 'dollar-sign' },
    { label: 'PEDIDOS ATIVOS', value: '1.204', change: '+5%', icon: 'shopping-cart' },
    { label: 'NOVOS CLIENTES', value: '340', change: '+18%', icon: 'users' },
    { label: 'ALERTA ESTOQUE', value: '23', change: '-2%', icon: 'alert-triangle', isAlert: true },
];

export const RECENT_ORDERS: Order[] = [
    { id: 'a1b2c3d4-e5f6', order_number: '25-001234', customer: 'ALEXANDRE MAGNO', status: 'PAID', total: 'R$ 450,00', date: '2025-12-12', items: 3 },
    { id: 'b2c3d4e5-f6a1', order_number: '25-001235', customer: 'JULIA ROBERTS', status: 'SHIPPED', total: 'R$ 1.290,00', date: '2025-12-12', items: 1 },
    { id: 'c3d4e5f6-a1b2', order_number: '25-001236', customer: 'CARLOS DRUMMOND', status: 'PENDING', total: 'R$ 89,90', date: '2025-12-13', items: 2 },
    { id: 'd4e5f6a1-b2c3', order_number: '25-001237', customer: 'MARIA QUITÉRIA', status: 'CANCELLED', total: 'R$ 2.400,00', date: '2025-12-13', items: 5 },
];

export const ALL_ORDERS: Order[] = [
    ...RECENT_ORDERS,
    { id: 'e5f6a1b2-c3d4', order_number: '25-001230', customer: 'ROBERTO CARLOS', status: 'DELIVERED', total: 'R$ 1.200,00', date: '2025-12-10', items: 4 },
    { id: 'f6a1b2c3-d4e5', order_number: '25-001229', customer: 'ANA TERRA', status: 'PAYMENT_PROCESSING', total: 'R$ 350,50', date: '2025-12-10', items: 2 },
    { id: 'a1b2c3d4-0001', order_number: '25-001228', customer: 'JOÃO GRILO', status: 'REFUNDED', total: 'R$ 90,00', date: '2025-12-09', items: 1 },
    { id: 'a1b2c3d4-0002', order_number: '25-001227', customer: 'CHICÓ DA SILVA', status: 'PREPARING', total: 'R$ 1.890,00', date: '2025-12-09', items: 6 },
];

export const PRODUCTS: Product[] = [
    { id: 'prod-001', sku: 'TSH-BLK-001', name: 'OVERSIZED TEE / BLACK', price: 'R$ 180,00', stock: 145, status: 'ACTIVE', category: 'APPAREL' },
    { id: 'prod-002', sku: 'HOOD-GRY-004', name: 'TECH FLEECE HOODIE', price: 'R$ 450,00', stock: 12, status: 'LOW_STOCK', category: 'OUTERWEAR' },
    { id: 'prod-003', sku: 'ACC-CAP-002', name: 'TACTICAL CAP 5-PANEL', price: 'R$ 120,00', stock: 0, status: 'OUT_OF_STOCK', category: 'ACCESSORIES' },
];

export const CATEGORIES_DATA: Category[] = [
    { id: 'cat-001', name: 'Vestuário', slug: 'vestuario', parent: '-', status: 'ACTIVE', products: 156 },
    { id: 'cat-002', name: 'T-Shirts', slug: 't-shirts', parent: 'Vestuário', status: 'ACTIVE', products: 45 },
    { id: 'cat-003', name: 'Acessórios', slug: 'acessorios', parent: '-', status: 'ACTIVE', products: 89 },
    { id: 'cat-004', name: 'Chapéus', slug: 'chapeus', parent: 'Acessórios', status: 'ACTIVE', products: 12 },
    { id: 'cat-005', name: 'Calçado', slug: 'calcado', parent: '-', status: 'INACTIVE', products: 0 },
];

export const CUSTOMERS_DATA: Customer[] = [
    { id: 'u-001', name: 'ALEXANDRE MAGNO', email: 'alex.magno@example.com', status: 'ACTIVE', total_spent: 'R$ 4.500,00', orders_count: 12, last_order: '2025-12-12', location: 'SÃO PAULO, SP' },
    { id: 'u-002', name: 'JULIA ROBERTS', email: 'j.roberts@hollywood.com', status: 'ACTIVE', total_spent: 'R$ 12.350,00', orders_count: 45, last_order: '2025-12-11', location: 'RIO DE JANEIRO, RJ' },
    { id: 'u-003', name: 'MARIA QUITÉRIA', email: 'm.quiteria@history.br', status: 'BLOCKED', total_spent: 'R$ 0,00', orders_count: 0, last_order: '-', location: 'SALVADOR, BA' },
    { id: 'u-004', name: 'CARLOS DRUMMOND', email: 'c.drummond@poetry.com', status: 'INACTIVE', total_spent: 'R$ 890,00', orders_count: 3, last_order: '2024-11-05', location: 'BELO HORIZONTE, MG' },
];

export const COUPONS_DATA: Coupon[] = [
    { id: 'c-001', code: 'SUMMER10', type: 'PERCENTAGE', value: '10%', status: 'ACTIVE', usage: { current: 45, max: 100 }, valid_until: '2026-01-30', min_purchase: 'R$ 0,00' },
    { id: 'c-002', code: 'WELCOME50', type: 'FIXED_AMOUNT', value: 'R$ 50,00', status: 'ACTIVE', usage: { current: 12, max: 1000 }, valid_until: '2026-12-31', min_purchase: 'R$ 200,00' },
    { id: 'c-003', code: 'FREESHIP', type: 'FREE_SHIPPING', value: 'FRETE GRÁTIS', status: 'PAUSED', usage: { current: 890, max: 1000 }, valid_until: '2025-12-25', min_purchase: 'R$ 150,00' },
    { id: 'c-004', code: 'BLACKFRIDAY', type: 'PERCENTAGE', value: '50%', status: 'EXPIRED', usage: { current: 500, max: 500 }, valid_until: '2025-11-30', min_purchase: 'R$ 0,00' },
    { id: 'c-005', code: 'VIP_MEMBER', type: 'PERCENTAGE', value: '20%', status: 'DEPLETED', usage: { current: 50, max: 50 }, valid_until: '2026-06-01', min_purchase: 'R$ 500,00' },
];

export const PAYMENTS_DATA: Payment[] = [
    { id: 'pay-001', order_id: 'a1b2c3d4-e5f6', customer: 'ALEXANDRE MAGNO', amount: 'R$ 450,00', method: 'CREDIT_CARD', gateway: 'STRIPE', status: 'CAPTURED', date: '2025-12-12 14:30', brand: 'MASTERCARD', last_four: '4242' },
    { id: 'pay-002', order_id: 'c3d4e5f6-a1b2', customer: 'CARLOS DRUMMOND', amount: 'R$ 89,90', method: 'PIX', gateway: 'PAGSEGURO', status: 'PENDING', date: '2025-12-13 09:15', brand: null, last_four: null },
    { id: 'pay-003', order_id: 'd4e5f6a1-b2c3', customer: 'MARIA QUITÉRIA', amount: 'R$ 2.400,00', method: 'CREDIT_CARD', gateway: 'STRIPE', status: 'FAILED', date: '2025-12-13 10:00', brand: 'VISA', last_four: '1234' },
    { id: 'pay-004', order_id: 'a1b2c3d4-0001', customer: 'JOÃO GRILO', amount: 'R$ 90,00', method: 'BOLETO', gateway: 'PAGAR.ME', status: 'REFUNDED', date: '2025-12-09 11:20', brand: null, last_four: null },
    { id: 'pay-005', order_id: 'b2c3d4e5-f6a1', customer: 'JULIA ROBERTS', amount: 'R$ 1.290,00', method: 'CREDIT_CARD', gateway: 'STRIPE', status: 'AUTHORIZED', date: '2025-12-12 18:45', brand: 'AMEX', last_four: '0005' },
];

export const SYSTEM_LOGS: SystemLog[] = [
    { created_at: '2025-12-13 14:32:10', module: 'PAYMENTS', action: 'GATEWAY_CONFIG_UPDATE', user: 'Admin User', ip: '192.168.1.1' },
    { created_at: '2025-12-13 10:15:00', module: 'CATALOG', action: 'PRODUCT_PRICE_BULK_UPDATE', user: 'Julia Roberts', ip: '172.16.0.5' },
    { created_at: '2025-12-12 18:20:45', module: 'AUTH', action: 'LOGIN_FAILURE_LIMIT', user: 'System', ip: '203.0.113.42' },
    { created_at: '2025-12-12 09:00:00', module: 'SYSTEM', action: 'DAILY_BACKUP_COMPLETED', user: 'System', ip: 'localhost' },
];

export const NOTIFICATIONS_DATA: Notification[] = [
    { id: 1, title: 'NOVO PEDIDO', message: 'Pedido #25-001238 recebido de Maria Silva.', time: 'Agora mesmo', unread: true },
    { id: 2, title: 'ALERTA ESTOQUE', message: 'O produto TECH FLEECE HOODIE atingiu o estoque mínimo (12 un).', time: '1 hora atrás', unread: true },
    { id: 3, title: 'SISTEMA', message: 'O backup diário da base de dados foi concluído com sucesso.', time: '4 horas atrás', unread: false },
    { id: 4, title: 'AVALIAÇÃO', message: 'Nova avaliação de 5 estrelas recebida em OVERSIZED TEE / BLACK.', time: 'Ontem', unread: false },
];
