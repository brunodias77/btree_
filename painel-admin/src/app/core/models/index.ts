export type OrderStatus =
    | 'PAID' | 'SHIPPED' | 'DELIVERED' | 'PENDING'
    | 'PAYMENT_PROCESSING' | 'PREPARING' | 'CANCELLED'
    | 'REFUNDED';

export type ProductStatus = 'ACTIVE' | 'LOW_STOCK' | 'OUT_OF_STOCK' | 'INACTIVE' | 'DRAFT';
export type CustomerStatus = 'ACTIVE' | 'INACTIVE' | 'BLOCKED';
export type CouponStatus = 'ACTIVE' | 'PAUSED' | 'EXPIRED' | 'DEPLETED';
export type PaymentStatus = 'CAPTURED' | 'AUTHORIZED' | 'PENDING' | 'FAILED' | 'REFUNDED' | 'CHARGEBACK';
export type CategoryStatus = 'ACTIVE' | 'INACTIVE';

export interface Order {
    id: string;
    order_number: string;
    customer: string;
    status: OrderStatus;
    total: string;
    date: string;
    items: number;
}

export interface Product {
    id: string;
    sku: string;
    name: string;
    price: string;
    stock: number;
    status: ProductStatus;
    category: string;
}

export interface Category {
    id: string;
    name: string;
    slug: string;
    parent: string;
    status: CategoryStatus;
    products: number;
}

export interface Customer {
    id: string;
    name: string;
    email: string;
    status: CustomerStatus;
    total_spent: string;
    orders_count: number;
    last_order: string;
    location: string;
}

export interface Coupon {
    id: string;
    code: string;
    type: string;
    value: string;
    status: CouponStatus;
    usage: { current: number; max: number };
    valid_until: string;
    min_purchase: string;
}

export interface Payment {
    id: string;
    order_id: string;
    customer: string;
    amount: string;
    method: string;
    gateway: string;
    status: PaymentStatus;
    date: string;
    brand: string | null;
    last_four: string | null;
}

export interface Notification {
    id: number;
    title: string;
    message: string;
    time: string;
    unread: boolean;
}

export interface Stat {
    label: string;
    value: string;
    change: string;
    icon: string;
    isAlert?: boolean;
}

export interface SystemLog {
    created_at: string;
    module: string;
    action: string;
    user: string;
    ip: string;
}
