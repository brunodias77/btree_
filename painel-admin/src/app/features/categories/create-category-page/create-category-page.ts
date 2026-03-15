import { Component, ChangeDetectionStrategy, inject, computed, signal, resource } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CategoryService } from '../services/category.service';
import { CreateCategoryInput } from '../models/category.types';

@Component({
  selector: 'app-create-category-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-category-page.html',
})
export class CreateCategoryPage {
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    name: ['', Validators.required],
    slug: ['', Validators.required],
    description: [''],
    status: ['ACTIVE', Validators.required],
    parent: [''], // Will hold Guid or empty
  });

  // Fetch only active root categories to allow parent nesting limits
  rootCategoriesResource = resource({
    params: () => ({ /* no dependencies to track */ }),
    loader: () => this.categoryService.getCategories({ pageSize: 50, isActive: true })
  });

  rootCategories = computed(() => {
    // Show only root categories (no parentId)
    return this.rootCategoriesResource.value()?.items.filter(c => !c.parentId) ?? [];
  });

  async submit() {
    if (this.form.invalid) { 
      this.form.markAllAsTouched(); 
      return; 
    }

    this.loading.set(true);
    this.error.set(null);

    const formValues = this.form.getRawValue();
    
    const input: CreateCategoryInput = {
      name: formValues.name!,
      slug: formValues.slug || undefined,
      description: formValues.description || undefined,
      parentId: formValues.parent ? formValues.parent : null,
      sortOrder: 0
    };

    try {
      await this.categoryService.createCategory(input);
      this.router.navigate(['/categories']);
    } catch (err: any) {
      this.error.set(err?.error?.message || 'Ocorreu um erro ao criar a categoria.');
    } finally {
      this.loading.set(false);
    }
  }
}
