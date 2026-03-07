import { HttpInterceptorFn, HttpBackend, HttpRequest, HttpEvent } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError, BehaviorSubject, filter, take, switchMap, finalize, Observable } from 'rxjs';
import { RefreshTokenResponse, ApiResponse } from '../auth/models';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const token = localStorage.getItem('token');

    // Clone the request and add the authorization header if token exists
    const authReq = token
        ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
        : req;

    return next(authReq);
};

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

function handle401Error(req: HttpRequest<any>, next: any, router: Router, httpBackend: HttpBackend): Observable<HttpEvent<any>> {
    if (req.url.includes('/api/auth/refresh') || req.url.includes('/api/auth/login')) {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        router.navigate(['/login']);
        return throwError(() => new Error('Authentication failed'));
    }

    if (!isRefreshing) {
        isRefreshing = true;
        refreshTokenSubject.next(null);

        const refreshToken = localStorage.getItem('refreshToken');

        if (!refreshToken) {
            isRefreshing = false;
            router.navigate(['/login']);
            return throwError(() => new Error('No refresh token available'));
        }

        // We use HttpBackend to create an isolated request that bypasses ALL interceptors
        // to prevent infinite loops on 401.
        return new Observable<HttpEvent<any>>(observer => {
            const refreshReq = new HttpRequest<any>(
                'POST',
                `${environment.apiUrl}/api/auth/refresh`,
                { refreshToken },
                { responseType: 'json' }
            );

            httpBackend.handle(refreshReq).subscribe({
                next: (event: any) => {
                    // Check if the event is a HttpResponse and has body
                    if (event.type === 4 && event.body) {
                        const res = event.body as ApiResponse<RefreshTokenResponse>;
                        if (res.success && res.data) {
                            localStorage.setItem('token', res.data.accessToken);
                            localStorage.setItem('refreshToken', res.data.refreshToken);
                            refreshTokenSubject.next(res.data.accessToken);

                            // Retry original request with new token
                            const newReq = req.clone({ setHeaders: { Authorization: `Bearer ${res.data.accessToken}` } });
                            next(newReq).subscribe({
                                next: (e: any) => observer.next(e),
                                error: (err: any) => observer.error(err),
                                complete: () => observer.complete()
                            });
                        } else {
                            localStorage.removeItem('token');
                            localStorage.removeItem('refreshToken');
                            router.navigate(['/login']);
                            observer.error(new Error('Refresh token invalid'));
                        }
                    }
                },
                error: (err) => {
                    localStorage.removeItem('token');
                    localStorage.removeItem('refreshToken');
                    router.navigate(['/login']);
                    observer.error(err);
                },
                complete: () => {
                    isRefreshing = false;
                }
            });
        });

    } else {
        // Queue parallel requests until refresh completes
        return refreshTokenSubject.pipe(
            filter(token => token !== null),
            take(1),
            switchMap(token => {
                const newReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
                return next(newReq) as Observable<HttpEvent<any>>;
            })
        );
    }
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);
    const httpBackend = inject(HttpBackend);

    return next(req).pipe(
        catchError(err => {
            if (err.status === 401) {
                return handle401Error(req, next, router, httpBackend);
            }
            if (err.status === 403) {
                router.navigate(['/unauthorized']);
            }
            return throwError(() => err);
        }),
    );
};
