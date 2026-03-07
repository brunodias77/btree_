import { Component, input } from '@angular/core';
import { NgClass } from '@angular/common';

const STATUS_CLASSES: Record<string, string> = {
  PAID: 'text-[#ccf381] border-[#ccf381]',
  SHIPPED: 'text-blue-400 border-blue-400',
  DELIVERED: 'text-zinc-300 border-zinc-500 bg-zinc-800',
  PENDING: 'text-yellow-500 border-yellow-500',
  PAYMENT_PROCESSING: 'text-yellow-500 border-yellow-500',
  PREPARING: 'text-purple-400 border-purple-400',
  CANCELLED: 'text-red-500 border-red-500',
  REFUNDED: 'text-pink-500 border-pink-500',
  ACTIVE: 'text-[#ccf381] border-[#ccf381]',
  LOW_STOCK: 'text-orange-500 border-orange-500',
  OUT_OF_STOCK: 'text-zinc-500 border-zinc-500 line-through',
  INACTIVE: 'text-zinc-500 border-zinc-600',
  BLOCKED: 'text-red-600 border-red-600 bg-red-950/20',
  PAUSED: 'text-yellow-500 border-yellow-500',
  EXPIRED: 'text-zinc-500 border-zinc-600 line-through',
  DEPLETED: 'text-red-500 border-red-500',
  CAPTURED: 'text-[#ccf381] border-[#ccf381]',
  AUTHORIZED: 'text-blue-400 border-blue-400',
  FAILED: 'text-red-600 border-red-600 bg-red-950/10',
  CHARGEBACK: 'text-red-500 border-red-500 bg-red-950/20',
};

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [NgClass],
  template: `
    <span
      [ngClass]="classes()"
      class="px-2 py-1 text-[10px] font-mono font-bold uppercase border"
    >
      {{ formatStatus(status()) }}
    </span>
  `,
})
export class StatusBadgeComponent {
  status = input.required<string>();

  classes() {
    return STATUS_CLASSES[this.status()] ?? 'text-zinc-400 border-zinc-600';
  }

  formatStatus(status: string): string {
    return status.replace(/_/g, ' ');
  }
}
