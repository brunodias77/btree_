---
name: angular-20-tailwind
description: >
  Scaffold and implement Angular v20+ applications using only Angular CLI 20+, standalone components, signals,
  Reactive Forms, functional guards/interceptors/resolvers, provideRouter(), loadComponent() and loadChildren().
  No NgModules, no OnInit, no external UI libraries. All UI built from scratch with TailwindCSS v4.
  Triggers on: creating components, forms, routes, guards, interceptors, resolvers, services, pipes, directives,
  or any Angular architecture task that requires modern, idiomatic Angular 20 code.
---

# Angular 20 + TailwindCSS — Skill

Produce clean, fully typed Angular v20+ code. Every file must compile with Angular CLI 20+ without warnings.

---

## ⚙️ Non-Negotiable Rules

| Rule | Detail |
|---|---|
| **No NgModules** | Never generate `NgModule`. Use standalone APIs everywhere. |
| **No `OnInit`** | Initialisation logic goes in the `constructor()` or via `effect()` / `resource()`. |
| **No external UI libs** | No Angular Material, PrimeNG, Bootstrap, Ng-Zorro, etc. |
| **TailwindCSS only** | All styling via Tailwind utility classes. Use `@apply` sparingly. |
| **`inject()` everywhere** | Never use constructor-parameter injection. |
| **Signal-first** | Prefer `signal()`, `computed()`, `linkedSignal()`, `resource()` over `BehaviorSubject`. |
| **Typed strictly** | No `any`. Enable `strict: true` in `tsconfig.json`. |
| **`input()` / `output()`** | Never use `@Input()` / `@Output()` decorators. |
| **`model()`** | Use `model()` for two-way bindings instead of paired input+output. |
| **Native control flow** | Use `@if`, `@for`, `@switch` — never `*ngIf`, `*ngFor`, `*ngSwitch`. |
| **`host` object** | Use the `host: {}` property — never `@HostBinding` / `@HostListener`. |

---

## 📁 Project Structure

```
src/
├── app/
│   ├── app.config.ts          ← ApplicationConfig (providers)
│   ├── app.routes.ts          ← Route definitions
│   ├── app.component.ts       ← Root shell
│   ├── core/
│   │   ├── guards/            ← Functional CanActivateFn guards
│   │   ├── interceptors/      ← Functional HttpInterceptorFn
│   │   ├── resolvers/         ← Functional ResolveFn
│   │   └── services/          ← Injectable services with signal state
│   ├── features/
│   │   └── <feature>/
│   │       ├── <feature>.routes.ts
│   │       └── components/
│   └── shared/
│       ├── components/        ← Reusable UI components
│       ├── pipes/             ← Standalone pipes
│       └── directives/        ← Standalone directives
├── styles.css                 ← @import "tailwindcss";
└── index.html
```

---

## 🔧 Bootstrap

```typescript
// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app.component';

bootstrapApplication(App, appConfig).catch(console.error);
```

```typescript
// app.config.ts
import { ApplicationConfig } from '@angular/core';
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
} from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      withComponentInputBinding(),   // route params → component inputs
      withInMemoryScrolling({ scrollPositionRestoration: 'enabled' }),
    ),
    provideHttpClient(
      withInterceptors([authInterceptor]),
    ),
  ],
};
```

```css
/* styles.css */
@import "tailwindcss";
```

---

## 🧩 Standalone Component

```typescript
// shared/components/button/button.component.ts
import {
  Component,
  ChangeDetectionStrategy,
  input,
  output,
  computed,
} from '@angular/core';
import { booleanAttribute } from '@angular/core';

type Variant = 'primary' | 'secondary' | 'danger' | 'ghost';
type Size    = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-button',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[class]': 'hostClass()',
    '[attr.aria-disabled]': 'disabled()',
    '(click)': '_onClick($event)',
    '(keydown.enter)': '_onClick($event)',
    '(keydown.space)': '$event.preventDefault(); _onClick($event)',
    'role': 'button',
    '[attr.tabindex]': 'disabled() ? -1 : 0',
  },
  template: `
    @if (loading()) {
      <span class="animate-spin mr-2 h-4 w-4 border-2 border-white border-t-transparent rounded-full inline-block"></span>
    }
    <ng-content />
  `,
})
export class Button {
  variant  = input<Variant>('primary');
  size     = input<Size>('md');
  disabled = input(false, { transform: booleanAttribute });
  loading  = input(false, { transform: booleanAttribute });

  clicked  = output<MouseEvent | KeyboardEvent>();

  private readonly BASE = 'inline-flex items-center justify-center font-semibold rounded-lg transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 cursor-pointer';

  private readonly VARIANTS: Record<Variant, string> = {
    primary:   'bg-indigo-600 text-white hover:bg-indigo-700 focus-visible:ring-indigo-500',
    secondary: 'bg-gray-100 text-gray-900 hover:bg-gray-200 focus-visible:ring-gray-400',
    danger:    'bg-red-600 text-white hover:bg-red-700 focus-visible:ring-red-500',
    ghost:     'bg-transparent text-gray-700 hover:bg-gray-100 focus-visible:ring-gray-400',
  };

  private readonly SIZES: Record<Size, string> = {
    sm: 'text-sm px-3 py-1.5 gap-1.5',
    md: 'text-sm px-4 py-2 gap-2',
    lg: 'text-base px-6 py-3 gap-2',
  };

  hostClass = computed(() => [
    this.BASE,
    this.VARIANTS[this.variant()],
    this.SIZES[this.size()],
    this.disabled() ? 'opacity-50 pointer-events-none' : '',
  ].join(' '));

  _onClick(e: MouseEvent | KeyboardEvent) {
    if (!this.disabled() && !this.loading()) {
      this.clicked.emit(e);
    }
  }
}
```

---

## 📡 Signal Service

```typescript
// core/services/auth.service.ts
import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'admin' | 'user';
}

export interface Credentials {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http   = inject(HttpClient);
  private readonly router = inject(Router);

  // Private writable state
  private readonly _user    = signal<User | null>(null);
  private readonly _loading = signal(false);
  private readonly _error   = signal<string | null>(null);

  // Public read-only API
  readonly user            = this._user.asReadonly();
  readonly loading         = this._loading.asReadonly();
  readonly error           = this._error.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isAdmin         = computed(() => this._user()?.role === 'admin');

  async login(credentials: Credentials): Promise<boolean> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const res = await firstValueFrom(
        this.http.post<{ token: string; user: User }>('/api/auth/login', credentials),
      );
      localStorage.setItem('token', res.token);
      this._user.set(res.user);
      return true;
    } catch {
      this._error.set('Invalid credentials.');
      return false;
    } finally {
      this._loading.set(false);
    }
  }

  async restoreSession(): Promise<boolean> {
    const token = localStorage.getItem('token');
    if (!token) return false;
    try {
      const user = await firstValueFrom(this.http.get<User>('/api/auth/me'));
      this._user.set(user);
      return true;
    } catch {
      localStorage.removeItem('token');
      return false;
    }
  }

  logout(): void {
    this._user.set(null);
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
```

---

## 🔑 Functional Guard

```typescript
// core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = async (route, state) => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) return true;

  const restored = await auth.restoreSession();
  if (restored) return true;

  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: state.url },
  });
};

// Role-based guard factory
export const roleGuard = (roles: string[]): CanActivateFn => () => {
  const auth   = inject(AuthService);
  const router = inject(Router);
  const role   = auth.user()?.role;

  if (role && roles.includes(role)) return true;
  return router.createUrlTree(['/unauthorized']);
};
```

---

## 🌐 Functional HTTP Interceptor

```typescript
// core/interceptors/auth.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  if (!token) return next(req);

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    }),
  );
};
```

```typescript
// core/interceptors/error.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError(err => {
      if (err.status === 401) router.navigate(['/login']);
      if (err.status === 403) router.navigate(['/unauthorized']);
      return throwError(() => err);
    }),
  );
};
```

---

## 🗺️ Routing

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard, roleGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },

  // Public
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent),
  },

  // Protected (lazy feature)
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },

  // Protected feature module (child routes)
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(['admin'])],
    loadChildren: () =>
      import('./features/admin/admin.routes').then(m => m.adminRoutes),
  },

  { path: '**', loadComponent: () => import('./features/not-found/not-found.component').then(m => m.NotFoundComponent) },
];
```

```typescript
// features/admin/admin.routes.ts
import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'users', pathMatch: 'full' },
      {
        path: 'users',
        loadComponent: () => import('./users/users.component').then(m => m.UsersComponent),
      },
    ],
  },
];
```

---

## 🔍 Functional Resolver

```typescript
// core/resolvers/user.resolver.ts
import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { UserService } from '../services/user.service';
import { User } from '../services/auth.service';

export const userResolver: ResolveFn<User> = route => {
  const userService = inject(UserService);
  return userService.getById(route.paramMap.get('id')!);
};

// Route usage:
// { path: 'users/:id', component: UserDetail, resolve: { user: userResolver } }

// Component — access via input():
// user = input.required<User>();
```

---

## 📝 Reactive Forms

```typescript
// features/auth/login/login.component.ts
import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div class="w-full max-w-sm">
        <h1 class="text-2xl font-bold text-gray-900 mb-6 text-center">Sign in</h1>

        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-4">
          <!-- Email -->
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input
              id="email"
              type="email"
              formControlName="email"
              class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm
                     focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
              [class.border-red-400]="emailInvalid()"
            />
            @if (emailInvalid()) {
              <p class="mt-1 text-xs text-red-500">Enter a valid email address.</p>
            }
          </div>

          <!-- Password -->
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-1">Password</label>
            <input
              id="password"
              type="password"
              formControlName="password"
              class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm
                     focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
              [class.border-red-400]="passwordInvalid()"
            />
            @if (passwordInvalid()) {
              <p class="mt-1 text-xs text-red-500">Password must be at least 8 characters.</p>
            }
          </div>

          <!-- Error -->
          @if (auth.error()) {
            <p class="text-sm text-red-600 bg-red-50 border border-red-200 rounded-lg px-3 py-2">
              {{ auth.error() }}
            </p>
          }

          <!-- Submit -->
          <button
            type="submit"
            [disabled]="form.invalid || auth.loading()"
            class="w-full flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm
                   font-semibold text-white hover:bg-indigo-700 transition-colors
                   disabled:opacity-50 disabled:cursor-not-allowed"
          >
            @if (auth.loading()) {
              <span class="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent"></span>
            }
            Sign in
          </button>
        </form>
      </div>
    </div>
  `,
})
export class LoginComponent {
  protected readonly auth  = inject(AuthService);
  private readonly router  = inject(Router);
  private readonly route   = inject(ActivatedRoute);
  private readonly fb      = inject(FormBuilder);

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  emailInvalid    = signal(false);
  passwordInvalid = signal(false);

  async submit() {
    const emailCtrl    = this.form.get('email')!;
    const passwordCtrl = this.form.get('password')!;

    this.emailInvalid.set(emailCtrl.invalid && emailCtrl.touched);
    this.passwordInvalid.set(passwordCtrl.invalid && passwordCtrl.touched);

    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const ok = await this.auth.login(this.form.getRawValue() as { email: string; password: string });
    if (ok) {
      const returnUrl = this.route.snapshot.queryParams['returnUrl'] ?? '/dashboard';
      this.router.navigateByUrl(returnUrl);
    }
  }
}
```

---

## ⚡ Resource API (Async Data)

```typescript
// features/users/user-detail/user-detail.component.ts
import { Component, ChangeDetectionStrategy, input, resource, inject, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

interface User { id: string; name: string; email: string; }

@Component({
  selector: 'app-user-detail',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (userResource.isLoading()) {
      <div class="flex justify-center py-12">
        <span class="h-8 w-8 animate-spin rounded-full border-4 border-indigo-600 border-t-transparent"></span>
      </div>
    } @else if (userResource.error()) {
      <div class="rounded-lg bg-red-50 border border-red-200 p-4 text-sm text-red-700">
        Failed to load user.
      </div>
    } @else if (userResource.hasValue()) {
      <div class="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
        <h2 class="text-xl font-bold text-gray-900">{{ userResource.value()!.name }}</h2>
        <p class="text-sm text-gray-500 mt-1">{{ userResource.value()!.email }}</p>
      </div>
    }
  `,
})
export class UserDetailComponent {
  id = input.required<string>();

  private http = inject(HttpClient);

  userResource = resource({
    params: () => ({ id: this.id() }),
    loader: ({ params }) =>
      firstValueFrom(this.http.get<User>(`/api/users/${params.id}`)),
  });
}
```

---

## 🔄 RxJS ↔ Signals Interop

```typescript
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs';
import { of } from 'rxjs';

// Debounced search
@Component({...})
export class SearchComponent {
  query   = signal('');
  private http = inject(HttpClient);

  results = toSignal(
    toObservable(this.query).pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(q =>
        q.length >= 2
          ? this.http.get<Result[]>(`/api/search?q=${q}`)
          : of([]),
      ),
      catchError(() => of([])),
    ),
    { initialValue: [] as Result[] },
  );
}
```

---

## 📐 Reusable UI Patterns (TailwindCSS)

### Card
```html
<div class="rounded-xl border border-gray-200 bg-white p-6 shadow-sm">
  <h3 class="text-base font-semibold text-gray-900">Title</h3>
  <p class="mt-1 text-sm text-gray-500">Description</p>
</div>
```

### Badge
```html
<!-- Status badges -->
<span class="inline-flex items-center rounded-full bg-green-50 px-2 py-0.5 text-xs font-medium text-green-700 ring-1 ring-green-600/20">
  Active
</span>
<span class="inline-flex items-center rounded-full bg-red-50 px-2 py-0.5 text-xs font-medium text-red-700 ring-1 ring-red-600/20">
  Inactive
</span>
```

### Empty State
```html
<div class="flex flex-col items-center justify-center py-16 text-center">
  <div class="mb-4 rounded-full bg-gray-100 p-4">
    <!-- SVG icon here -->
  </div>
  <h3 class="text-sm font-semibold text-gray-900">No results</h3>
  <p class="mt-1 text-sm text-gray-500">Try adjusting your filters.</p>
</div>
```

### Data Table
```html
<div class="overflow-x-auto rounded-xl border border-gray-200">
  <table class="min-w-full divide-y divide-gray-200 text-sm">
    <thead class="bg-gray-50">
      <tr>
        <th class="px-4 py-3 text-left font-semibold text-gray-600">Name</th>
        <th class="px-4 py-3 text-left font-semibold text-gray-600">Email</th>
        <th class="px-4 py-3 text-right font-semibold text-gray-600">Actions</th>
      </tr>
    </thead>
    <tbody class="divide-y divide-gray-100 bg-white">
      @for (row of rows(); track row.id) {
        <tr class="hover:bg-gray-50 transition-colors">
          <td class="px-4 py-3 font-medium text-gray-900">{{ row.name }}</td>
          <td class="px-4 py-3 text-gray-500">{{ row.email }}</td>
          <td class="px-4 py-3 text-right">
            <!-- actions -->
          </td>
        </tr>
      }
    </tbody>
  </table>
</div>
```

---

## 🔢 Lifecycle Replacement

| Old (avoid) | New (use) |
|---|---|
| `ngOnInit` | `constructor()` + `effect()` or `resource()` |
| `ngOnChanges` | `computed()` reacting to `input()` signals |
| `ngAfterViewInit` | `afterNextRender(() => { ... })` |
| `ngOnDestroy` | `DestroyRef` via `inject(DestroyRef).onDestroy(fn)` |

```typescript
import { inject, DestroyRef, afterNextRender, effect } from '@angular/core';

export class MyComponent {
  private destroyRef = inject(DestroyRef);

  constructor() {
    // Replaces ngOnInit
    effect(() => {
      console.log('userId changed:', this.userId());
    });

    // Replaces ngAfterViewInit — SSR safe
    afterNextRender(() => {
      // DOM access here
    });

    // Replaces ngOnDestroy
    this.destroyRef.onDestroy(() => {
      // cleanup
    });
  }
}
```

---

## ✅ Checklist Before Committing

- [ ] No `NgModule`, `CommonModule`, or `BrowserModule` imported anywhere
- [ ] No `@Input()` / `@Output()` decorators — using `input()` / `output()` / `model()`
- [ ] No `ngOnInit` — constructor / `effect()` / `resource()` used instead
- [ ] No external UI library dependencies in `package.json`
- [ ] All components use `ChangeDetectionStrategy.OnPush`
- [ ] All components import only what they use (no wildcard imports)
- [ ] `provideRouter()` and `provideHttpClient()` present in `app.config.ts`
- [ ] `withComponentInputBinding()` enabled in `provideRouter()`
- [ ] TailwindCSS v4 installed: `@import "tailwindcss"` in `styles.css`
- [ ] Strict TypeScript — no `any`, no implicit types

---

For reference patterns see:
- [references/signal-patterns.md](references/signal-patterns.md)
- [references/routing-patterns.md](references/routing-patterns.md)
- [references/component-patterns.md](references/component-patterns.md)
