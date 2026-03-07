import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { InputComponent } from '../../../shared/components/ui/input.component';
import { ButtonComponent } from '../../../shared/components/ui/button.component';

@Component({
  selector: 'app-login',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, InputComponent, ButtonComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-dark px-4 font-sans animate-fade-in">
      <div class="w-full max-w-sm bg-surface border border-zinc-800 p-8">
        
        <!-- Header -->
        <div class="mb-8 text-center">
          <h1 class="text-3xl font-black text-zinc-50 tracking-tighter uppercase">btree_</h1>
          <p class="text-zinc-400 font-mono text-xs mt-2 uppercase tracking-widest">Painel Administrativo v.04</p>
        </div>

        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">
          
          <app-input
            label="E-mail"
            type="email"
            formControlName="email"
            placeholder="DIGITE SEU EMAIL"
            [control]="form.get('email')"
            errorMessage="E-mail inválido."
          ></app-input>

          <app-input
            label="Senha"
            type="password"
            formControlName="password"
            placeholder="DIGITE SUA SENHA"
            [control]="form.get('password')"
            errorMessage="Insira a senha."
          ></app-input>

          <!-- Error Alert -->
          @if (auth.error()) {
            <div class="bg-red-600/10 border border-red-600 p-3">
              <p class="text-xs text-red-600 font-mono uppercase text-center">
                {{ auth.error() }}
              </p>
            </div>
          }

          <!-- Submit Button -->
          <div class="flex flex-col items-center gap-4 mt-8">
            <app-button
              type="submit"
              [disabled]="form.invalid || auth.loading()"
              [loading]="auth.loading()"
            >
              Entrar
            </app-button>
            
            <a routerLink="/forgot-password" class="text-xs font-mono text-zinc-400 hover:text-white uppercase tracking-wider underline decoration-zinc-700 underline-offset-4">
               Esqueci minha senha
            </a>
            
            <a routerLink="/register" class="text-xs font-mono text-zinc-400 hover:text-white uppercase tracking-wider underline decoration-zinc-700 underline-offset-4">
               Não possui conta? Cadastre-se
            </a>
          </div>
          
        </form>
      </div>
    </div>
  `,
})
export class LoginComponent {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  async submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const credentials = this.form.getRawValue() as { email: string; password: string };
    const ok = await this.auth.login(credentials);

    if (ok) {
      const returnUrl = this.route.snapshot.queryParams['returnUrl'] ?? '/';
      this.router.navigateByUrl(returnUrl);
    }
  }
}
