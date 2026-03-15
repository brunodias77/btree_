import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiResponse, GetProductsParams, PagedResult, ProductOutput, ProductDetailOutput } from './product.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/products`;

  async getProducts(params: GetProductsParams = {}): Promise<PagedResult<ProductOutput>> {
    let httpParams = new HttpParams();
    
    if (params.pageNumber !== undefined) httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize !== undefined) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);
    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.categoryId) httpParams = httpParams.set('categoryId', params.categoryId);
    if (params.brandId) httpParams = httpParams.set('brandId', params.brandId);
    if (params.status) httpParams = httpParams.set('status', params.status);

    const response = await firstValueFrom(
      this.http.get<ApiResponse<PagedResult<ProductOutput>>>(this.apiUrl, { params: httpParams })
    );
    
    const data = response.data;
    if (data && data.items) {
      data.items = data.items.map(item => ({
        ...item,
        mainImageUrl: item.mainImageUrl ? (item.mainImageUrl.startsWith('http') ? item.mainImageUrl : `${environment.apiUrl}${item.mainImageUrl}`) : null
      }));
    }
    return data;
  }

  async getProductById(id: string): Promise<ProductDetailOutput> {
    const response = await firstValueFrom(
      this.http.get<ApiResponse<ProductDetailOutput>>(`${this.apiUrl}/${id}`)
    );
    
    const data = response.data;
    if (data && data.images) {
      data.images = data.images.map(img => ({
        ...img,
        url: img.url.startsWith('http') ? img.url : `${environment.apiUrl}${img.url}`
      }));
    }
    return data;
  }
}
