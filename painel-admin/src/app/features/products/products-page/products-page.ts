import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { StatusBadgeComponent } from '../../../shared/components/ui/status-badge.components';
import { InputComponent } from '../../../shared/components/ui/input.component';
import { ProductService } from '../product.service';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, switchMap, catchError, of, combineLatest } from 'rxjs';
import { PagedResult, ProductOutput } from '../product.models';

@Component({
  selector: 'app-products-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, FormsModule, StatusBadgeComponent, InputComponent],
  templateUrl: './products-page.html',
})
export class ProductsPage {
  private productService = inject(ProductService);

  searchQuery = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);

  pagedProducts = toSignal(
    combineLatest([
      toObservable(this.searchQuery).pipe(
        debounceTime(300),
        distinctUntilChanged()
      ),
      toObservable(this.pageNumber),
      toObservable(this.pageSize)
    ]).pipe(
      switchMap(([searchTerm, page, size]) => this.productService.getProducts({
        searchTerm,
        pageNumber: page,
        pageSize: size
      })),
      catchError(() => of({
        items: [],
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false
      } as PagedResult<ProductOutput>))
    ),
    { initialValue: {
        items: [],
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false
      } as PagedResult<ProductOutput>
    }
  );

  nextPage() {
    if (this.pagedProducts().hasNextPage) {
      this.pageNumber.update(p => p + 1);
    }
  }

  previousPage() {
    if (this.pagedProducts().hasPreviousPage) {
      this.pageNumber.update(p => p - 1);
    }
  }
}
