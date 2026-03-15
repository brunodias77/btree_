import { Component, ChangeDetectionStrategy, input, resource, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
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
  private readonly router = inject(Router);

  categoryResource = resource<CategoryOutput, { id: string }>({
    params: () => ({ id: this.id() }),
    loader: async ({ params }) => await this.categoryService.getCategoryById(params.id)
  });

  isDeleting = signal<boolean>(false);

  async deleteCategory() {
    if (confirm('Tem certeza que deseja excluir esta categoria? Esta ação não pode ser desfeita.')) {
      try {
        this.isDeleting.set(true);
        await this.categoryService.deleteCategory(this.id());
        await this.router.navigate(['/categories']);
      } catch (error) {
        console.error('Erro ao excluir a categoria', error);
        alert('Não foi possível excluir a categoria. Verifique se ela possui categorias filhas ou produtos vinculados.');
      } finally {
        this.isDeleting.set(false);
      }
    }
  }
}
