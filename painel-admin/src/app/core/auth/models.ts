export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    cpf?: string;
    birthDate?: string;
}

export interface ConfirmEmailRequest {
    code: string;
}

export interface ForgotPasswordRequest {
    email: string;
}

export interface ResetPasswordRequest {
    code: string;
    newPassword: string;
}

export interface AdminLoginUserOutput {
    userId: string;
    email: string;
    firstName?: string;
    lastName?: string;
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
}

export interface RefreshTokenResponse {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
}

export interface ApiResponse<T> {
    success: boolean;
    message?: string;
    timestamp: string;
    data: T;
}
