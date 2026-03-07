import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginRequest, RegisterRequest, ConfirmEmailRequest, ForgotPasswordRequest, ResetPasswordRequest, AdminLoginUserOutput, ApiResponse } from '../models';

import { environment } from '../../../../environments/environment';

export interface User {
    id: string;
    email: string;
    name: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly router = inject(Router);

    // Private writable state
    private readonly _user = signal<User | null>(null);
    private readonly _loading = signal(false);
    private readonly _error = signal<string | null>(null);

    // Public read-only API
    readonly user = this._user.asReadonly();
    readonly loading = this._loading.asReadonly();
    readonly error = this._error.asReadonly();
    readonly isAuthenticated = computed(() => this._user() !== null);

    async login(credentials: LoginRequest): Promise<boolean> {
        this._loading.set(true);
        this._error.set(null);
        try {
            const res = await firstValueFrom(
                this.http.post<ApiResponse<AdminLoginUserOutput>>(`${environment.apiUrl}/api/auth/login`, credentials),
            );

            if (res.success && res.data) {
                localStorage.setItem('token', res.data.accessToken);
                localStorage.setItem('refreshToken', res.data.refreshToken);
                this._user.set({
                    id: res.data.userId,
                    email: res.data.email,
                    name: `${res.data.firstName ?? ''} ${res.data.lastName ?? ''}`.trim() || res.data.email
                });
                return true;
            } else {
                this._error.set(res.message || 'Invalid credentials.');
                return false;
            }
        } catch (err: any) {
            if (err.status === 401) {
                this._error.set('Credenciais inválidas.');
            } else {
                this._error.set('Ocorreu um erro ao fazer login.');
            }
            return false;
        } finally {
            this._loading.set(false);
        }
    }

    async register(data: RegisterRequest): Promise<boolean> {
        this._loading.set(true);
        this._error.set(null);
        try {
            const res = await firstValueFrom(
                this.http.post<ApiResponse<string>>(`${environment.apiUrl}/api/auth/register`, data),
            );

            if (res.success) {
                return true;
            } else {
                this._error.set(res.message || 'Erro ao registrar.');
                return false;
            }
        } catch (err: any) {
            this._error.set(err?.error?.message || 'Ocorreu um erro ao registrar usuário.');
            return false;
        } finally {
            this._loading.set(false);
        }
    }

    async confirmEmail(data: ConfirmEmailRequest): Promise<boolean> {
        this._loading.set(true);
        this._error.set(null);
        try {
            const res = await firstValueFrom(
                this.http.post<ApiResponse<string>>(`${environment.apiUrl}/api/auth/confirm-email`, data),
            );

            if (res.success) {
                return true;
            } else {
                this._error.set(res.message || 'Erro ao confirmar e-mail.');
                return false;
            }
        } catch (err: any) {
            this._error.set(err?.error?.message || 'Código inválido ou expirado.');
            return false;
        } finally {
            this._loading.set(false);
        }
    }

    async forgotPassword(data: ForgotPasswordRequest): Promise<boolean> {
        this._loading.set(true);
        this._error.set(null);
        try {
            // Backend returns StatusCodes.Status204NoContent on success, so we don't expect a body
            await firstValueFrom(
                this.http.post(`${environment.apiUrl}/api/auth/forgot-password`, data),
            );
            return true;
        } catch (err: any) {
            this._error.set(err?.error?.message || 'Ocorreu um erro ao solicitar redefinição de senha.');
            return false;
        } finally {
            this._loading.set(false);
        }
    }

    async resetPassword(data: ResetPasswordRequest): Promise<boolean> {
        this._loading.set(true);
        this._error.set(null);


        try {
            // Backend returns StatusCodes.Status204NoContent on success, so we don't expect a body
            await firstValueFrom(
                this.http.post(`${environment.apiUrl}/api/auth/reset-password`, data),
            );
            return true;
        } catch (err: any) {
            this._error.set(err?.error?.message || 'Ocorreu um erro ao solicitar redefinição de senha.');
            return false;
        } finally {
            this._loading.set(false);
        }
    }

    async restoreSession(): Promise<boolean> {
        const token = localStorage.getItem('token');
        if (!token) return false;

        // We would need a /me endpoint or similar to fully restore, 
        // but for now if token exists we assume logged in until a request fails 401.
        // Let's set a dummy user just to pass guards, and rely on 401 interceptor later.
        // In a real app we'd fetch the user profile here.
        try {
            // Fake restore for now since we don't have a GET /api/auth/me explicitly requested yet
            this._user.set({ id: 'restored', email: 'restored@sessao.com', name: 'Admin' });
            return true;
        } catch {
            localStorage.removeItem('token');
            return false;
        }
    }

    async logout(): Promise<void> {
        this._loading.set(true);
        const refreshToken = localStorage.getItem('refreshToken');

        if (refreshToken) {
            try {
                await firstValueFrom(
                    this.http.post(`${environment.apiUrl}/api/auth/logout`, { refreshToken })
                );
            } catch (err) {
                // Ignore logout errors (e.g., token already invalid on backend)
                console.warn('Logout request failed or token was already invalid.', err);
            }
        }

        this._user.set(null);
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        this._loading.set(false);
        this.router.navigate(['/login']);
    }
}
