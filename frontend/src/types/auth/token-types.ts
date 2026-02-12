/**
 * Tipos de autenticação - Tokens
 */

/**
 * Par de tokens de autenticação
 */
export interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

/**
 * Payload decodificado do JWT
 */
export interface JwtPayload {
  sub: string; // userId
  email: string;
  firstName?: string;
  lastName?: string;
  roles?: string[];
  exp: number; // expiration timestamp
  iat: number; // issued at timestamp
  iss: string; // issuer
  aud: string; // audience
}

/**
 * Requisição de refresh token
 */
export interface RefreshTokenRequest {
  refreshToken: string;
}

/**
 * Resposta de refresh token
 */
export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

/**
 * Requisição de confirmação de e-mail
 */
export interface ConfirmEmailRequest {
  userId: string;
  token: string;
}

/**
 * Requisição para reenviar e-mail de confirmação
 */
export interface ResendConfirmationEmailRequest {
  email: string;
}

/**
 * Requisição de esqueci minha senha
 * Baseado em Users.Application.Features.Auth.ForgotPassword.ForgotPasswordCommand
 */
export interface ForgotPasswordRequest {
  email: string;
}

/**
 * Requisição de redefinição de senha
 * Baseado em Users.Application.Features.Auth.ResetPassword.ResetPasswordCommand
 */
export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}

/**
 * Requisição de alteração de senha
 * Baseado em Users.Application.Features.Auth.ChangePassword.ChangePasswordCommand
 */
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}
