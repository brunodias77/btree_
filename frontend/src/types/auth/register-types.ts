/**
 * Tipos de autenticação - Registro
 */

/**
 * Requisição de registro
 * Baseado em Users.Application.Features.Auth.Register.RegisterUserCommand
 */
export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  cpf?: string | null;
  birthDate?: string | null; // formato: YYYY-MM-DD
}

/**
 * Resposta de registro (retorna o ID do usuário criado)
 */
export interface RegisterResponse {
  userId: string;
}

/**
 * Dados do formulário de registro (no frontend)
 */
export interface RegisterFormData extends RegisterRequest {
  confirmPassword: string;
  acceptTerms: boolean;
  acceptPrivacy: boolean;
}
