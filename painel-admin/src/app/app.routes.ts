import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
    {
        path: 'login',
        loadComponent: () =>
            import('./features/auth/login/login.component').then(m => m.LoginComponent),
    },
    {
        path: 'register',
        loadComponent: () =>
            import('./features/auth/register/register.component').then(m => m.RegisterComponent),
    },
    {
        path: 'forgot-password',
        loadComponent: () =>
            import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
    },
    {
        path: 'reset-password',
        loadComponent: () =>
            import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
    },
    {
        path: 'confirm-email',
        loadComponent: () =>
            import('./features/auth/confirm-email/confirm-email.component').then(m => m.ConfirmEmailComponent),
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./shared/components/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
        children: [
            {
                path: 'dashboard',
                loadComponent: () =>
                    import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
            },
            {
                path: 'products',
                loadComponent: () =>
                    import('./features/products/products-page/products-page').then(m => m.ProductsPage),
            },
            {
                path: 'products/new',
                loadComponent: () =>
                    import('./features/products/products-page/products-create-page/products-create-page').then(m => m.ProductsCreatePage),
            },
            {
                path: 'categories',
                loadComponent: () =>
                    import('./features/categories/categories-page/categories-page').then(m => m.CategoriesPage),
            },
            {
                path: 'categories/new',
                loadComponent: () =>
                    import('./features/categories/create-category-page/create-category-page').then(m => m.CreateCategoryPage),
            },
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    // Default fallback
    { path: '**', redirectTo: '/dashboard' }
];
