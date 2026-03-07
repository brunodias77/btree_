# Signal Patterns — Angular 20

## linkedSignal — Dependent Reset

```typescript
const categories = signal(['All', 'Tech', 'Design']);
// Resets to first category whenever the list changes
const activeCategory = linkedSignal(() => categories()[0]);
```

## resource() — Async with Abort

```typescript
listResource = resource({
  params: () => ({ page: this.page(), search: this.search() }),
  loader: async ({ params, abortSignal }) => {
    const url = `/api/items?page=${params.page}&q=${params.search}`;
    const res = await fetch(url, { signal: abortSignal });
    if (!res.ok) throw new Error('Request failed');
    return res.json() as Promise<Item[]>;
  },
  defaultValue: [] as Item[],
});
```

## Signal Store (complex state)

```typescript
@Injectable({ providedIn: 'root' })
export class CartStore {
  private state = signal({ items: [] as CartItem[], open: false });

  readonly items  = computed(() => this.state().items);
  readonly open   = computed(() => this.state().open);
  readonly total  = computed(() =>
    this.items().reduce((s, i) => s + i.price * i.qty, 0),
  );
  readonly count  = computed(() =>
    this.items().reduce((s, i) => s + i.qty, 0),
  );

  add(item: CartItem) {
    this.state.update(s => {
      const existing = s.items.find(i => i.id === item.id);
      const items = existing
        ? s.items.map(i => i.id === item.id ? { ...i, qty: i.qty + 1 } : i)
        : [...s.items, { ...item, qty: 1 }];
      return { ...s, items };
    });
  }

  remove(id: string) {
    this.state.update(s => ({ ...s, items: s.items.filter(i => i.id !== id) }));
  }

  toggle() {
    this.state.update(s => ({ ...s, open: !s.open }));
  }
}
```

## Optimistic Updates

```typescript
async toggleDone(id: string) {
  const prev = this.todos();
  // 1. Optimistic UI update
  this.todos.update(list =>
    list.map(t => t.id === id ? { ...t, done: !t.done } : t),
  );
  try {
    await firstValueFrom(this.http.patch(`/api/todos/${id}/toggle`, {}));
  } catch {
    // 2. Rollback on error
    this.todos.set(prev);
  }
}
```
