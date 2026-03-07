import { Component, inject, signal } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { AsyncPipe, NgClass } from '@angular/common';
import { StoreService } from '../../../core/services/store.service';


@Component({
  selector: 'app-header',
  imports: [NgClass],
  templateUrl: './header-component.html',
})
export class HeaderComponent {
  protected store = inject(StoreService);
  private router = inject(Router);
  notifOpen = signal(false);


  get pageTitle(): string {
    const url = this.router.url.split('/')[1]?.split('?')[0]?.toUpperCase() ?? 'DASHBOARD';
    const labels: Record<string, string> = {
      DASHBOARD: 'DASHBOARD',
      PRODUCTS: 'PRODUTOS',
      CATEGORIES: 'CATEGORIAS',
      ORDERS: 'PEDIDOS',
      CUSTOMERS: 'CLIENTES',
      COUPONS: 'CUPONS',
      PAYMENTS: 'FINANCEIRO',
      SETTINGS: 'CONFIGURAÇÕES',
    };
    return labels[url] ?? url;
  }

  toggleNotifications(): void {
    this.notifOpen.update(v => !v);
  }

}
