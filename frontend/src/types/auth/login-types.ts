/**
 * Tipos de autenticação - Login
 */

/**
 * Requisição de login
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Resposta de login
 * Baseado em Users.Application.Features.Auth.Login.LoginResponse
 */
export interface LoginResponse {
  userId: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

/**
 * Dados do usuário autenticado para o contexto/store
 */
export interface AuthUser {
  userId: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  fullName: string;
}
