import { Component, ChangeDetectionStrategy, signal, inject, resource, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { CategoryService } from '../services/category.service';

@Component({
  selector: 'app-categories-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, FormsModule, DatePipe, StatusBadgeComponent],
  templateUrl: './categories-page.html',
})
export class CategoriesPage {
  private readonly categoryService = inject(CategoryService);

  search = signal<string>('');
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);

  // Instead of using RxJS debounce explicitly, we could just let resource do its thing
  // or use `toSignal`/`toObservable` if strict debouncing is required. But resource is standard now.
  // Often it's good to type in the template to update the signal.
  
  categoriesResource = resource({
    params: () => ({ 
      pageNumber: this.pageNumber(), 
      pageSize: this.pageSize(), 
      searchTerm: this.search() 
    }),
    loader: ({ params, abortSignal }) => this.categoryService.getCategories(params)
  });

  totalItems = computed(() => this.categoriesResource.value()?.totalCount ?? 0);
  items = computed(() => this.categoriesResource.value()?.items ?? []);

  nextPage() {
    if (this.categoriesResource.value()?.hasNextPage) {
      this.pageNumber.update(p => p + 1);
    }
  }

  previousPage() {
    if (this.categoriesResource.value()?.hasPreviousPage) {
      this.pageNumber.update(p => p - 1);
    }
  }

  onSearchChange(val: string) {
    this.search.set(val);
    this.pageNumber.set(1); // reset to first page on search
  }
}
