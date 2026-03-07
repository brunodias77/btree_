import { Injectable, signal, computed } from '@angular/core';
import { Order, Product, Category, Coupon, Notification } from '../models';
import {
    ALL_ORDERS, PRODUCTS, CATEGORIES_DATA,
    COUPONS_DATA, NOTIFICATIONS_DATA
} from '../data/mock-data';

@Injectable({ providedIn: 'root' })
export class StoreService {
    // --- State Signals ---
    readonly orders = signal<Order[]>(ALL_ORDERS);
    readonly products = signal<Product[]>(PRODUCTS);
    readonly categories = signal<Category[]>(CATEGORIES_DATA);
    readonly coupons = signal<Coupon[]>(COUPONS_DATA);
    readonly notifications = signal<Notification[]>(NOTIFICATIONS_DATA);

    // --- Computed ---
    readonly hasUnread = computed(() => this.notifications().some(n => n.unread));

    // --- Mutations ---
    addProduct(product: Product): void {
        this.products.update(list => [product, ...list]);
    }

    addCategory(category: Category): void {
        this.categories.update(list => [category, ...list]);
    }

    addOrder(order: Order): void {
        this.orders.update(list => [order, ...list]);
    }

    addCoupon(coupon: Coupon): void {
        this.coupons.update(list => [coupon, ...list]);
    }

    markAllRead(): void {
        this.notifications.update(list => list.map(n => ({ ...n, unread: false })));
    }
}
