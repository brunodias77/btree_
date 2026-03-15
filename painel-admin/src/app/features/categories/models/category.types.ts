import { PagedRequest } from '../../../shared/models/pagination.model';

export interface CategoryOutput {
    id: string;
    name: string;
    slug: string;
    parentId?: string;
    path: string;
    isActive: boolean;
    createdAt: string;
}

export interface GetCategoriesInput extends PagedRequest {
    searchTerm?: string;
    parentId?: string;
    isActive?: boolean;
}

export interface CreateCategoryInput {
    name: string;
    slug?: string;
    parentId?: string | null;
    description?: string;
    imageUrl?: string;
    metaTitle?: string;
    metaDescription?: string;
    sortOrder?: number;
}

export interface UpdateCategoryRequest {
    name: string;
    slug?: string;
    description?: string;
    imageUrl?: string;
    metaTitle?: string;
    metaDescription?: string;
    sortOrder?: number;
}
