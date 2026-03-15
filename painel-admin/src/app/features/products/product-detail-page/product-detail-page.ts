import { Component, ChangeDetectionStrategy, input, resource, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ProductService } from '../product.service';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { ProductDetailOutput, ProductImageOutput } from '../product.models';

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

  /** Imagem principal: a marcada como isPrimary ou a primeira da lista */
  mainImage = computed<ProductImageOutput | null>(() => {
    const product = this.productResource.value();
    if (!product?.images?.length) return null;
    return product.images.find(i => i.isPrimary) ?? product.images[0];
  });

  /** Miniaturas: todas exceto a imagem principal */
  thumbnailImages = computed<ProductImageOutput[]>(() => {
    const product = this.productResource.value();
    const main = this.mainImage();
    if (!product?.images?.length || !main) return [];
    return product.images.filter(i => i.id !== main.id);
  });

  async deleteProduct() {
      // Temporary placeholder if needed. Implementation omitted for this task.
  }
}
