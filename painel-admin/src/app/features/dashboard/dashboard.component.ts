import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { AuthService } from '../../core/auth/services/auth.service';

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],

  template: `
    <div class="p-8">
      <h1 class="text-3xl font-black text-brand tracking-tighter uppercase">Dashboard</h1>
      <p class="mt-4 text-zinc-400 font-mono text-sm">Bem vindo, {{ auth.user()?.name }}</p>

      <button
        (click)="logout()"
        class="mt-8 bg-zinc-900 border border-zinc-700 text-white font-bold uppercase py-3 px-6 hover:border-brand hover:text-brand transition-colors tracking-widest text-xs"
      >
        Sair do Sistema
      </button>
    </div>
  `,
})
export class DashboardComponent {
  readonly auth = inject(AuthService);

  async logout() {
    await this.auth.logout();
  }
}
