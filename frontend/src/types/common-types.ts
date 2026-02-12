/**
 * Tipos comuns utilizados em toda a aplicação
 */

/**
 * Parâmetros de paginação para requisições
 */
export interface PageParams {
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Parâmetros de ordenação
 */
export interface SortParams {
  sortBy?: string;
  sortOrder?: SortOrder;
}

/**
 * Direção de ordenação
 */
export type SortOrder = "asc" | "desc";

/**
 * Parâmetros combinados de paginação e ordenação
 */
export interface PagedSortedParams extends PageParams, SortParams {}

/**
 * Parâmetros de busca com filtros
 */
export interface SearchParams extends PagedSortedParams {
  search?: string;
  filters?: Record<string, unknown>;
}

/**
 * Estado de loading
 */
export type LoadingState = "idle" | "loading" | "success" | "error";

/**
 * Opção para selects/dropdowns
 */
export interface SelectOption<T = string> {
  value: T;
  label: string;
  disabled?: boolean;
}

/**
 * Opção agrupada para selects
 */
export interface GroupedSelectOption<T = string> {
  label: string;
  options: SelectOption<T>[];
}

/**
 * Item de navegação/menu
 */
export interface NavItem {
  label: string;
  href?: string;
  icon?: string;
  badge?: string | number;
  children?: NavItem[];
  disabled?: boolean;
}

/**
 * Item de breadcrumb
 */
export interface BreadcrumbItem {
  label: string;
  href?: string;
}

/**
 * Range de valores (para filtros de preço, etc)
 */
export interface Range<T = number> {
  min: T;
  max: T;
}

/**
 * Range de datas
 */
export interface DateRange {
  start: Date | string | null;
  end: Date | string | null;
}

/**
 * Coordenadas geográficas
 */
export interface Coordinates {
  latitude: number;
  longitude: number;
}

/**
 * Arquivo com preview
 */
export interface FileWithPreview extends File {
  preview: string;
}

/**
 * Resposta de upload
 */
export interface UploadResponse {
  url: string;
  filename: string;
  size: number;
  mimeType: string;
}

/**
 * Dados base com timestamps
 */
export interface Timestamps {
  createdAt: string;
  updatedAt: string;
}

/**
 * Entidade base com ID e timestamps
 */
export interface BaseEntity extends Timestamps {
  id: string;
}

/**
 * Callback genérico
 */
export type Callback<T = void> = () => T;

/**
 * Handler de evento genérico
 */
export type EventHandler<E = unknown> = (event: E) => void;

/**
 * Função assíncrona genérica
 */
export type AsyncFunction<T = void, A extends unknown[] = []> = (
  ...args: A
) => Promise<T>;

/**
 * Torna propriedades específicas opcionais
 */
export type PartialBy<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;

/**
 * Torna propriedades específicas obrigatórias
 */
export type RequiredBy<T, K extends keyof T> = Omit<T, K> &
  Required<Pick<T, K>>;

/**
 * Tipo para valores de formulário (compatível com React Hook Form)
 */
export type FormValues<T> = {
  [K in keyof T]: T[K] extends object
    ? FormValues<T[K]>
    : T[K] | undefined | null;
};
