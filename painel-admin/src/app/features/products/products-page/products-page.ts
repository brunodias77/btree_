import { Component, computed, inject, signal } from '@angular/core';
import { StoreService } from '../../../core/services/store.service';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { InputComponent } from '../../../shared/components/ui/input.component';

@Component({
  selector: 'app-products-page',
  imports: [RouterLink, FormsModule, StatusBadgeComponent, InputComponent],
  templateUrl: './products-page.html',
})
export class ProductsPage {
  protected store = inject(StoreService);
  search = signal('');

  filtered = computed(() =>
    this.store.products().filter(p =>
      p.name.toLowerCase().includes(this.search().toLowerCase()) ||
      p.sku.toLowerCase().includes(this.search().toLowerCase())
    )
  );
}
