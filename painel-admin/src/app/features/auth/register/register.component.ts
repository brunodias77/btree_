import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/services/auth.service';
import { RegisterRequest } from '../../../core/auth/models';
import { InputComponent } from '../../../shared/components/ui/input.component';
import { ButtonComponent } from '../../../shared/components/ui/button.component';

@Component({
  selector: 'app-register',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, InputComponent, ButtonComponent],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-dark px-4 py-8 font-sans animate-fade-in">
      <div class="w-full max-w-xl bg-surface border border-zinc-800 p-8">
        
        <!-- Header -->
        <div class="mb-8 text-center">
          <h1 class="text-3xl font-black text-zinc-50 tracking-tighter uppercase">btree_</h1>
          <p class="text-zinc-400 font-mono text-xs mt-2 uppercase tracking-widest">Painel Administrativo v.04</p>
          <h2 class="text-xl font-bold text-brand mt-6 uppercase">Novo Cadastro</h2>
        </div>

        <form [formGroup]="form" (ngSubmit)="submit()" class="space-y-6">
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <app-input
              label="Nome"
              type="text"
              formControlName="firstName"
              placeholder="SEU NOME"
              errorMessage="Nome obrigatório."
              [control]="form.get('firstName')"
            ></app-input>

            <app-input
              label="Sobrenome"
              type="text"
              formControlName="lastName"
              placeholder="SEU SOBRENOME"
              errorMessage="Sobrenome obrigatório."
              [control]="form.get('lastName')"
            ></app-input>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <app-input
              label="E-mail"
              type="email"
              formControlName="email"
              placeholder="SEU EMAIL"
              errorMessage="E-mail inválido."
              [control]="form.get('email')"
            ></app-input>

            <app-input
              label="Senha"
              type="password"
              formControlName="password"
              placeholder="MÍN. 8 CARACTERES"
              errorMessage="Insira a senha (Mín. 8)."
              [control]="form.get('password')"
            ></app-input>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
             <app-input
              label="CPF"
              type="text"
              formControlName="cpf"
              placeholder="SOMENTE NÚMEROS"
              [optional]="true"
            ></app-input>

            <app-input
              label="Data Nasc."
              type="date"
              formControlName="birthDate"
              [optional]="true"
            ></app-input>
          </div>

          <!-- Error Alert -->
          @if (auth.error()) {
            <div class="bg-red-600/10 border border-red-600 p-3">
              <p class="text-xs text-red-600 font-mono uppercase text-center">
                {{ auth.error() }}
              </p>
            </div>
          }

          <!-- Submit Button -->
          <div class="pt-4 flex flex-col items-center gap-4">
            <app-button
              type="submit"
              [disabled]="form.invalid || auth.loading()"
              [loading]="auth.loading()"
            >
              CRIAR CONTA
            </app-button>
            
            <a routerLink="/login" class="text-xs font-mono text-zinc-400 hover:text-white uppercase tracking-wider underline decoration-zinc-700 underline-offset-4">
               Já possui conta? Entrar
            </a>
          </div>
          
        </form>
      </div>
    </div>
  `,
})
export class RegisterComponent {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  form = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    cpf: [''],
    birthDate: ['']
  });

  invalidFields = signal<Record<string, boolean>>({});

  isInvalid(controlName: string): boolean {
    const ctrl = this.form.get(controlName);
    return !!(ctrl && ctrl.invalid && ctrl.touched);
  }

  async submit() {
    this.form.markAllAsTouched();

    // Update standalone signal mapping for template tracking
    const invalidMap: Record<string, boolean> = {};
    Object.keys(this.form.controls).forEach(key => {
      invalidMap[key] = this.isInvalid(key);
    });
    this.invalidFields.set(invalidMap);

    if (this.form.invalid) {
      return;
    }

    const payload = this.form.getRawValue();
    // Clean up empty optional fields
    const data: RegisterRequest = {
      email: payload.email!,
      password: payload.password!,
      firstName: payload.firstName!,
      lastName: payload.lastName!,
      ...(payload.cpf ? { cpf: payload.cpf } : {}),
      ...(payload.birthDate ? { birthDate: payload.birthDate } : {})
    };

    const ok = await this.auth.register(data);

    if (ok) {
      // Registration successful, send to confirm-email
      this.router.navigate(['/confirm-email']);
    }
  }
}
