export interface ProductOutput {
  id: string;
  name: string;
  slug: string;
  sku: string;
  price: number;
  currency: string;
  stockQuantity: number;
  brandName: string;
  categoryName: string;
  status: string;
  mainImageUrl: string | null;
}

export interface ProductImageOutput {
  id: string;
  url: string;
  isPrimary: boolean;
}

export interface ProductDetailOutput {
  id: string;
  name: string;
  description: string;
  sku: string;
  price: number;
  quantityInStock: number;
  status: string;
  brandName: string;
  categoryName: string;
  images: ProductImageOutput[];
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface GetProductsParams {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: string;
  searchTerm?: string;
  categoryId?: string;
  brandId?: string;
  status?: string;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  errors: any[];
}
