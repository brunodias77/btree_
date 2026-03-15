import { Component, ChangeDetectionStrategy, Input, forwardRef, signal, computed } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, FormsModule, ReactiveFormsModule, AbstractControl } from '@angular/forms';

let nextUniqueId = 0;

@Component({
  selector: 'app-input',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    }
  ],
  template: `
    <div class="space-y-2">
      <label [for]="inputId" class="block font-mono text-xs font-bold text-zinc-400 uppercase tracking-widest">
        {{ label }}
        @if (optional) {
            <span class="text-zinc-600 text-[10px]">(OPCIONAL)</span>
        }
      </label>
      <div class="relative">
        @if (hasIcon) {
          <div class="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500 flex items-center justify-center pointer-events-none">
            <ng-content select="[icon]"></ng-content>
          </div>
        }
        <input
          [id]="inputId"
          [type]="currentType"
          [value]="value()"
          (input)="onInput($event)"
          (blur)="onTouchedAction()"
          [placeholder]="placeholder"
          [disabled]="disabled()"
          class="w-full bg-zinc-950 border border-zinc-700 p-3 text-white placeholder:text-zinc-600 outline-none focus:border-brand font-mono text-sm uppercase transition-colors"
          [class.pr-12]="type === 'password'"
          [class.pl-10]="hasIcon"
          [class.border-red-600]="showError"
          [class.text-center]="centered"
          [class.tracking-widest]="centered || currentType === 'password'"
          style="color-scheme: dark"
        />
        
        @if (type === 'password') {
          <button 
            type="button" 
            (click)="togglePassword()" 
            class="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-500 hover:text-zinc-300 focus:outline-none"
            tabindex="-1"
          >
            @if (showPassword()) {
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></svg>
            } @else {
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>
            }
          </button>
        }
      </div>
      <!-- Error Feedback -->
      @if (showError && errorMessage) {
        <p class="text-xs text-red-600 font-mono uppercase mt-1">{{ errorMessage }}</p>
      }
    </div>
  `,
})
export class InputComponent implements ControlValueAccessor {
  @Input({ required: true }) label!: string;
  @Input() type: 'text' | 'password' | 'email' | 'date' = 'text';
  @Input() placeholder = '';
  @Input() errorMessage = 'Campo inválido';
  @Input() showErrorSignal?: () => boolean; // Advanced: allow parent to dictate error state manually
  @Input() control?: AbstractControl | null; // Pass control if we want automatic error parsing (alternative to showErrorSignal)
  @Input() optional = false;
  @Input() centered = false;
  @Input() hasIcon = false;

  readonly inputId = `app-input-${nextUniqueId++}`;

  protected readonly value = signal<string>('');
  protected readonly disabled = signal<boolean>(false);
  private touched = signal<boolean>(false);
  showPassword = signal<boolean>(false);

  get currentType(): string {
    if (this.type === 'password') {
      return this.showPassword() ? 'text' : 'password';
    }
    return this.type;
  }

  togglePassword() {
    this.showPassword.set(!this.showPassword());
  }

  // Dynamic accessor instead of computed, because AbstractControl lacks signals
  get showError(): boolean {
    // 1. If explicit signal is provided
    if (this.showErrorSignal) return this.showErrorSignal();

    // 2. If control is provided (common in ReactiveForms)
    if (this.control) {
      return !!(this.control.invalid && (this.control.touched || this.touched()));
    }

    return false;
  }

  // Interface implementations
  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(val: any): void {
    this.value.set(val || '');
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  // Internal Logic
  onInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.onChange(target.value);
  }

  onTouchedAction() {
    this.touched.set(true);
    this.onTouched();
  }
}
