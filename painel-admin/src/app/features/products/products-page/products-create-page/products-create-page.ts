import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { StoreService } from '../../../../core/services/store.service';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Product } from '../../../../core/models';
import { ButtonComponent } from '../../../../shared/components/ui/button.component';
import { InputComponent } from '../../../../shared/components/ui/input.component';

@Component({
  selector: 'app-products-create-page',
  imports: [RouterLink, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './products-create-page.html',
})
export class ProductsCreatePage {
  private store = inject(StoreService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  form = this.fb.group({
    name: ['', Validators.required],
    description: [''],
    status: ['ACTIVE', Validators.required],
    category: ['', Validators.required],
    sku: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    stock: [0, Validators.min(0)],
  });

  activeCategories = computed(() =>
    this.store.categories().filter(c => c.status === 'ACTIVE')
  );

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const priceNum = parseFloat(String(v.price ?? 0));
    const stockNum = parseInt(String(v.stock ?? 0), 10);
    const stockStatus =
      stockNum === 0
        ? "OUT_OF_STOCK"
        : stockNum < 15
          ? "LOW_STOCK"
          : (v.status as any);

    const product: Product = {
      id: `prod-${Date.now()}`,
      sku: (v.sku ?? "").toUpperCase(),
      name: v.name ?? "",
      category: v.category ?? "",
      price: `R$ ${priceNum.toFixed(2).replace(".", ",")}`,
      stock: stockNum,
      status: stockStatus,
    };
    this.store.addProduct(product);
    this.router.navigate(["/products"]);
  }
}
