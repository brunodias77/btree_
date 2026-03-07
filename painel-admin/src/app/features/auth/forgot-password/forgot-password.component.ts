import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { ButtonComponent } from '../../../shared/components/ui/button.component';
import { InputComponent } from '../../../shared/components/ui/input.component';

@Component({
  selector: 'app-forgot-password',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, ButtonComponent, InputComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-zinc-950 px-4">
      <div class="w-full max-w-sm bg-zinc-900 border border-zinc-800 p-8">
        <h1 class="text-3xl font-black text-zinc-50 mb-2 uppercase tracking-wide">Recuperar</h1>
        <p class="text-sm text-zinc-400 mb-8 font-mono">Digite seu email para receber as instruções.</p>

        @if (success()) {
          <div class="mb-6 p-4 border border-[#ccf381] bg-[#ccf381]/10 text-[#ccf381] font-mono text-sm uppercase">
            Código enviado com sucesso. Verifique sua caixa de entrada.
          </div>
          
          <div class="mt-4">
            <app-button routerLink="/reset-password" type="button">Inserir Código</app-button>
          </div>
        } @else {
          <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">
            <!-- Email -->
            <app-input
              formControlName="email"
              type="email"
              label="Email"
              placeholder="DIGITE SEU EMAIL"
              errorMessage="Insira um email válido."
              [showErrorSignal]="emailInvalid"
            ></app-input>

            <!-- Error -->
            @if (auth.error()) {
              <div class="p-3 border border-red-600/30 bg-red-600/10 text-red-600 font-mono text-xs uppercase">
                {{ auth.error() }}
              </div>
            }

            <!-- Submit -->
            <app-button
              type="submit"
              [disabled]="form.invalid"
              [loading]="auth.loading()"
            >
              Recuperar Senha
            </app-button>
            
            <div class="pt-4 text-center">
              <a routerLink="/login" class="text-xs font-mono text-zinc-400 hover:text-white transition-colors uppercase cursor-pointer">
                Lembrei minha senha
              </a>
            </div>
          </form>
        }
      </div>
    </div>
  `,
})
export class ForgotPasswordComponent {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  emailInvalid = signal(false);
  success = signal(false);

  async submit() {
    const emailCtrl = this.form.get('email')!;

    this.emailInvalid.set(emailCtrl.invalid && emailCtrl.touched);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.emailInvalid.set(emailCtrl.invalid);
      return;
    }

    const email = this.form.getRawValue().email!;
    const ok = await this.auth.forgotPassword({ email });
    if (ok) {
      this.success.set(true);
    }
  }
}
