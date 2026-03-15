import { Component, ChangeDetectionStrategy, inject, computed, signal, resource, effect, input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CategoryService } from '../services/category.service';
import { UpdateCategoryRequest, CategoryOutput } from '../models/category.types';

@Component({
  selector: 'app-edit-category-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './edit-category-page.html',
})
export class EditCategoryPage {
  // Input route param mapped safely via withComponentInputBinding in app.config.ts
  id = input.required<string>();

  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  submitLoading = signal(false);
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

  // Fetch the current category being edited based on the ID
  categoryResource = resource<CategoryOutput, { id: string }>({
    params: () => ({ id: this.id() }),
    loader: ({ params }) => this.categoryService.getCategoryById(params.id)
  });

  rootCategories = computed(() => {
    // Show only root categories (no parentId), exclude itself to prevent self-parenting
    return this.rootCategoriesResource.value()?.items.filter(c => !c.parentId && c.id !== this.id()) ?? [];
  });

  constructor() {
    effect(() => {
      const data = this.categoryResource.value();
      if (data) {
        this.form.patchValue({
          name: data.name,
          slug: data.slug,
          description: '', // description might not be fully fetched unless exposed by the backend
          status: data.isActive ? 'ACTIVE' : 'INACTIVE',
          parent: data.parentId ?? ''
        });
      }
    });
  }

  async submit() {
    if (this.form.invalid) { 
      this.form.markAllAsTouched(); 
      return; 
    }

    this.submitLoading.set(true);
    this.error.set(null);

    const formValues = this.form.getRawValue();
    
    const request: UpdateCategoryRequest = {
      name: formValues.name!,
      slug: formValues.slug || undefined,
      description: formValues.description || undefined,
      sortOrder: 0
    };

    try {
      await this.categoryService.updateCategory(this.id(), request);
      this.router.navigate(['/categories']);
    } catch (err: any) {
      this.error.set(err?.error?.message || 'Ocorreu um erro ao atualizar a categoria.');
    } finally {
      this.submitLoading.set(false);
    }
  }
}
