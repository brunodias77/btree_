import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/services/auth.service';

export const authGuard: CanActivateFn = async (route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.isAuthenticated()) return true;

    const restored = await auth.restoreSession();
    if (restored) return true;

    return router.createUrlTree(['/login'], {
        queryParams: { returnUrl: state.url },
    });
};
