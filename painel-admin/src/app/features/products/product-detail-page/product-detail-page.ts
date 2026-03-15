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

  /** IDs de imagens que falharam ao carregar (404, etc.) */
  private brokenImageIds = signal<Set<string>>(new Set());

  /** Imagem principal: a marcada como isMain ou a primeira da lista, excluindo imagens quebradas */
  mainImage = computed<ProductImageOutput | null>(() => {
    const product = this.productResource.value();
    if (!product?.images?.length) return null;
    const broken = this.brokenImageIds();
    const validImages = product.images.filter(i => !broken.has(i.id));
    if (!validImages.length) return null;
    return validImages.find(i => i.isMain) ?? validImages[0];
  });

  /** Miniaturas: todas exceto a imagem principal, excluindo imagens quebradas */
  thumbnailImages = computed<ProductImageOutput[]>(() => {
    const product = this.productResource.value();
    const main = this.mainImage();
    const broken = this.brokenImageIds();
    if (!product?.images?.length || !main) return [];
    return product.images.filter(i => i.id !== main.id && !broken.has(i.id));
  });

  /** Marca uma imagem como quebrada e força recálculo do mainImage */
  onImageError(imageId: string): void {
    this.brokenImageIds.update(set => {
      const next = new Set(set);
      next.add(imageId);
      return next;
    });
  }

  async deleteProduct() {
      // Temporary placeholder if needed. Implementation omitted for this task.
  }
}
