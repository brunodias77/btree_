import { Component, ChangeDetectionStrategy, input, resource, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ProductService } from '../product.service';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { ProductDetailOutput } from '../product.models';

@Component({
  selector: 'app-product-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule, StatusBadgeComponent],
  templateUrl: './product-detail-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductDetailPage {
  // Recebe o ID via route params (requires withComponentInputBinding in app.routes.ts)
  id = input.required<string>();

  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);

  productResource = resource<ProductDetailOutput, { id: string }>({
    params: () => ({ id: this.id() }),
    loader: async ({ params }) => await this.productService.getProductById(params.id)
  });

  isDeleting = signal<boolean>(false);

  async deleteProduct() {
      // Temporary placeholder if needed. Implementation omitted for this task.
  }
}
