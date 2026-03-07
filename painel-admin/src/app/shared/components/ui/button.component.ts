import { Component, ChangeDetectionStrategy, Input } from '@angular/core';

@Component({
  selector: 'app-button',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    'class': ' w-full'
  },
  template: `
    <button
      [type]="type"
      [attr.form]="formId"
      [disabled]="disabled || loading"
      class="w-full bg-[#ccf381] text-black font-bold uppercase py-2 px-4 hover:bg-white transition-colors tracking-widest flex items-center justify-center gap-2 disabled:opacity-50 border border-[#ccf381] hover:border-white"
    >
      @if (loading) {
        <!-- Brutalist loader -->
        <span class="inline-block w-4 h-4 border-2 border-black border-t-transparent animate-spin"></span>
      } @else {
        <ng-content></ng-content>
      }
    </button>
  `,
})
export class ButtonComponent {
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() loading = false;
  @Input() disabled = false;
  @Input() formId?: string;
}
