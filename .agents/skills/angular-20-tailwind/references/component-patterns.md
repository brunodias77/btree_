# Component Patterns — Angular 20

## model() — Two-Way Binding

```typescript
// Input field with [(value)]
@Component({
  selector: 'app-input',
  host: { '(input)': 'value.set($any($event.target).value)' },
  template: `<input [value]="value()" class="..." />`,
})
export class AppInput {
  value       = model('');
  placeholder = input('');
  disabled    = input(false, { transform: booleanAttribute });
}

// Usage
<app-input [(value)]="username" placeholder="Username" />
```

## @defer — Lazy UI Sections

```typescript
@Component({
  template: `
    @defer (on viewport; prefetch on idle) {
      <app-heavy-chart [data]="data()" />
    } @placeholder {
      <div class="h-64 rounded-xl bg-gray-100 animate-pulse"></div>
    } @error {
      <p class="text-red-500 text-sm">Chart failed to load.</p>
    }
  `,
})
export class Dashboard { data = input.required<ChartData>(); }
```

## viewChild / viewChildren

```typescript
@Component({
  template: `<div #wrap><app-card *... /></div>`,
})
export class List {
  wrap  = viewChild.required<ElementRef<HTMLDivElement>>('wrap');
  cards = viewChildren(CardComponent);

  constructor() {
    afterNextRender(() => {
      console.log('wrapper width:', this.wrap().nativeElement.offsetWidth);
    });
  }
}
```

## contentChildren — Compound Components

```typescript
@Component({
  selector: 'app-accordion',
  template: `<ng-content />`,
})
export class Accordion {
  panels = contentChildren(AccordionPanel);
  active = signal<AccordionPanel | null>(null);

  open(panel: AccordionPanel) {
    this.active.set(panel);
  }
}

@Component({
  selector: 'app-accordion-panel',
  host: { '[class.open]': 'isOpen()' },
  template: `
    <button (click)="accordion.open(this)" class="w-full text-left px-4 py-3 font-medium">
      {{ label() }}
    </button>
    @if (isOpen()) {
      <div class="px-4 pb-4"><ng-content /></div>
    }
  `,
})
export class AccordionPanel {
  label     = input.required<string>();
  accordion = inject(Accordion);
  isOpen    = computed(() => this.accordion.active() === this);
}
```

## Standalone Pipe

```typescript
@Pipe({ name: 'timeAgo' })
export class TimeAgoPipe implements PipeTransform {
  transform(date: string | Date): string {
    const diff = Date.now() - new Date(date).getTime();
    const mins = Math.floor(diff / 60_000);
    if (mins < 1)  return 'just now';
    if (mins < 60) return `${mins}m ago`;
    const hrs = Math.floor(mins / 60);
    if (hrs < 24)  return `${hrs}h ago`;
    return `${Math.floor(hrs / 24)}d ago`;
  }
}
```

## Standalone Directive

```typescript
@Directive({
  selector: '[appAutoFocus]',
})
export class AutoFocusDirective {
  private el = inject(ElementRef<HTMLElement>);

  constructor() {
    afterNextRender(() => this.el.nativeElement.focus());
  }
}
```
