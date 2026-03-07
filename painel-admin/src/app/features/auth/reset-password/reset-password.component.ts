import { Component, ChangeDetectionStrategy, inject, signal, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { ButtonComponent } from '../../../shared/components/ui/button.component';
import { InputComponent } from '../../../shared/components/ui/input.component';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
        return { passwordsMismatch: true };
    }
    return null;
}

@Component({
    selector: 'app-reset-password',
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [ReactiveFormsModule, RouterLink, ButtonComponent, InputComponent],
    template: `
    <div class="min-h-screen flex items-center justify-center bg-zinc-950 px-4">
      <div class="w-full max-w-sm bg-zinc-900 border border-zinc-800 p-8">
        <h1 class="text-3xl font-black text-zinc-50 mb-2 uppercase tracking-wide">Nova Senha</h1>
        <p class="text-sm text-zinc-400 mb-8 font-mono">Digite sua nova senha abaixo.</p>

        @if (success()) {
          <div class="mb-6 p-4 border border-[#ccf381] bg-[#ccf381]/10 text-[#ccf381] font-mono text-sm uppercase">
            Senha redefinida com sucesso.
          </div>
          
          <div class="mt-4">
            <app-button routerLink="/login" type="button">Fazer Login</app-button>
          </div>
        } @else {
          <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">

            <app-input
              formControlName="code"
              type="text"
              label="Código de Verificação"
              placeholder="000000"
              errorMessage="O código de 6 dígitos é obrigatório."
              [control]="form.get('code')"
            ></app-input>
            
            <app-input
              formControlName="newPassword"
              type="password"
              label="Nova Senha"
              placeholder="DIGITE A NOVA SENHA"
              errorMessage="A senha deve ter no mínimo 8 caracteres."
              [control]="form.get('newPassword')"
            ></app-input>

            <app-input
              formControlName="confirmPassword"
              type="password"
              label="Confirmar Senha"
              placeholder="CONFIRME A SENHA"
              errorMessage="As senhas não coincidem."
              [showErrorSignal]="passwordsMismatch"
            ></app-input>

            <!-- API Error -->
            @if (auth.error()) {
              <div class="p-3 border border-red-600/30 bg-red-600/10 text-red-600 font-mono text-xs uppercase text-center tracking-widest">
                {{ auth.error() }}
              </div>
            }

            <app-button
              type="submit"
              [disabled]="form.invalid || form.hasError('passwordsMismatch')"
              [loading]="auth.loading()"
            >
              Redefinir Senha
            </app-button>
            
            <div class="pt-4 text-center">
              <a routerLink="/login" class="text-xs font-mono text-zinc-400 hover:text-white transition-colors uppercase cursor-pointer">
                Cancelar
              </a>
            </div>
          </form>
        }
      </div>
    </div>
  `,
})
export class ResetPasswordComponent {
    protected readonly auth = inject(AuthService);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly fb = inject(FormBuilder);

    form = this.fb.group({
        code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
        newPassword: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]]
    }, { validators: passwordMatchValidator });

    success = signal(false);

    // Derived signal to check if passwords mismatch (reacts to form changes efficiently)
    passwordsMismatch = computed(() => {
        // Only show error if the user has touched or dirtied the confirm parameter
        const cpCtrl = this.form.get('confirmPassword');
        if (!cpCtrl?.touched && !cpCtrl?.dirty) {
            return false;
        }
        return this.form.hasError('passwordsMismatch');
    });

    constructor() {
        // Capture the code dynamically from query params in the constructor instead of ngOnInit
        const codeStr = this.route.snapshot.queryParamMap.get('code');
        if (codeStr) {
            this.form.patchValue({ code: codeStr });
        }
    }

    async submit() {
        this.form.markAllAsTouched();

        if (this.form.invalid || this.form.hasError('passwordsMismatch')) {
            return;
        }

        const { code, newPassword } = this.form.getRawValue();

        const ok = await this.auth.resetPassword({
            code: code!,
            newPassword: newPassword!
        });

        if (ok) {
            this.success.set(true);
        }
    }
}
