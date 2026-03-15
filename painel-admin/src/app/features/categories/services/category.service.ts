import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { PagedResult } from '../../../shared/models/pagination.model';
import { ApiResponse } from '../../../core/auth/models';
import { CategoryOutput, GetCategoriesInput, CreateCategoryInput, UpdateCategoryRequest } from '../models/category.types';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/categories`;

  async getCategories(input: GetCategoriesInput = {}): Promise<PagedResult<CategoryOutput>> {
    let params = new HttpParams();

    if (input.pageNumber) {
      params = params.set('pageNumber', input.pageNumber.toString());
    }
    if (input.pageSize) {
      params = params.set('pageSize', input.pageSize.toString());
    }
    if (input.searchTerm) {
      params = params.set('searchTerm', input.searchTerm);
    }
    if (input.parentId) {
      params = params.set('parentId', input.parentId);
    }
    if (input.isActive !== undefined) {
      params = params.set('isActive', input.isActive.toString());
    }

    const res = await firstValueFrom(
      this.http.get<ApiResponse<PagedResult<CategoryOutput>>>(this.apiUrl, { params })
    );

    return res.data;
  }

  async createCategory(input: CreateCategoryInput): Promise<string> {
    const res = await firstValueFrom(
      this.http.post<ApiResponse<string>>(this.apiUrl, input)
    );
    return res.data; // Retrieves the Guid
  }

  async getCategoryById(id: string): Promise<CategoryOutput> {
    const res = await firstValueFrom(
      this.http.get<ApiResponse<CategoryOutput>>(`${this.apiUrl}/${id}`)
    );
    return res.data;
  }

  async updateCategory(id: string, input: UpdateCategoryRequest): Promise<string> {
    const res = await firstValueFrom(
      this.http.put<ApiResponse<string>>(`${this.apiUrl}/${id}`, input)
    );
    return res.data;
  }

  async deleteCategory(id: string): Promise<string> {
    const res = await firstValueFrom(
      this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${id}`)
    );
    return res.data;
  }
}
