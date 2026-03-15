import { Component, ChangeDetectionStrategy, input, resource, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CategoryService } from '../services/category.service';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { CategoryOutput } from '../models/category.types';

@Component({
  selector: 'app-category-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule, StatusBadgeComponent, DatePipe],
  templateUrl: './category-detail-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryDetailPage {
  // Recebe o ID via route params (requires withComponentInputBinding in app.routes.ts)
  id = input.required<string>();

  private readonly categoryService = inject(CategoryService);

  categoryResource = resource<CategoryOutput, { id: string }>({
    params: () => ({ id: this.id() }),
    loader: async ({ params }) => await this.categoryService.getCategoryById(params.id)
  });
}
