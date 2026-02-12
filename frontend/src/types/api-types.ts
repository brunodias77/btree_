/**
 * Tipos de resposta da API
 * Baseados em BuildingBlocks.Web.Models
 */

/**
 * Resposta de sucesso da API
 */
export interface ApiResponse<T = void> {
  success: boolean;
  message?: string;
  timestamp: string;
  data?: T;
}

/**
 * Detalhes de erro
 */
export interface ErrorDetails {
  code: string;
  message: string;
  details?: string;
}

/**
 * Erro de validação
 */
export interface ValidationError {
  field: string;
  message: string;
}

/**
 * Resposta de erro da API
 */
export interface ApiErrorResponse {
  success: false;
  error: ErrorDetails;
  validationErrors?: ValidationError[];
  correlationId?: string;
  timestamp: string;
}

/**
 * Metadados de paginação
 */
export interface PaginationMeta {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/**
 * Resposta paginada da API
 */
export interface PaginatedResponse<T> {
  success: boolean;
  items: T[];
  pagination: PaginationMeta;
  timestamp: string;
}

/**
 * Helper type para extrair dados de ApiResponse
 */
export type ExtractData<T> = T extends ApiResponse<infer D> ? D : never;

/**
 * Helper type para resposta com dados obrigatórios
 */
export interface ApiDataResponse<T> extends ApiResponse<T> {
  data: T;
}

/**
 * Resultado genérico (compatível com Result do backend)
 */
export interface Result<T = void, E = ErrorDetails> {
  isSuccess: boolean;
  isFailure: boolean;
  value?: T;
  error?: E;
}
