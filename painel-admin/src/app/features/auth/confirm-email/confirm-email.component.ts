import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { InputComponent } from '../../../shared/components/ui/input.component';
import { ButtonComponent } from '../../../shared/components/ui/button.component';

@Component({
  selector: 'app-confirm-email',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, InputComponent, ButtonComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-dark px-4 font-sans animate-fade-in">
      <div class="w-full max-w-sm bg-surface border border-zinc-800 p-8">
        
        <!-- Header -->
        <div class="mb-8 text-center text-zinc-50">
          <h1 class="text-3xl font-black tracking-tighter uppercase">btree_</h1>
          <p class="text-zinc-400 font-mono text-xs mt-2 uppercase tracking-widest">Painel Administrativo v.04</p>
          <h2 class="text-xl font-bold mt-6 uppercase text-brand">Confirmar E-mail</h2>
          <p class="text-zinc-500 font-mono text-[10px] mt-2 uppercase tracking-wide">
             Insira o código enviado para você
          </p>
        </div>

        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">
          
          <app-input
            label="Código"
            type="text"
            formControlName="code"
            placeholder="CÓDIGO DE 6 DÍGITOS"
            errorMessage="Insira um código válido."
            [control]="form.get('code')"
            [centered]="true"
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
              Confirmar
            </app-button>
            
            <a routerLink="/login" class="text-xs font-mono text-zinc-400 hover:text-white uppercase tracking-wider underline decoration-zinc-700 underline-offset-4 mt-2">
               Voltar ao Login
            </a>
          </div>
          
        </form>
      </div>
    </div>
  `,
})
export class ConfirmEmailComponent {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    code: ['', [Validators.required, Validators.minLength(4)]] // Some codes might be 6 chars, let's keep min 4 just in case
  });

  isInvalid(controlName: string): boolean {
    const ctrl = this.form.get(controlName);
    return !!(ctrl && ctrl.invalid && ctrl.touched);
  }

  async submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const payload = this.form.getRawValue();
    const ok = await this.auth.confirmEmail({ code: payload.code! });

    if (ok) {
      // Confirmed, user can login now
      this.router.navigate(['/login']);
    }
  }
}
