---

# btree_ // Design System V.04

> **Status:** Ativo / Produção
> **Estética:** Brutalismo Digital, Streetwear, Utilitário
> **Tech Stack:** React, Tailwind CSS, Lucide React

## 1. Introdução

O design system da **btree_** é uma fusão de brutalismo digital e estética streetwear. Priorizamos o alto contraste, tipografia ousada, cantos retos (sharp) e micro-interações táteis. A interface deve parecer uma ferramenta de terminal moderna: funcional, rápida e crua.

---

## 2. Design Tokens

### 2.1 Cores (Palette)

A paleta é dominada por tons de Zinc (Slate escura), com um acento ácido de Lime Green para ações principais e feedback positivo.

| Token            | Nome             | Hex       | Classe Tailwind   | Uso                       |
| ---------------- | ---------------- | --------- | ----------------- | ------------------------- |
| **Brand**        | Acento da Marca  | `#ccf381` | `bg-[#ccf381]`    | CTAs, Seleção, Destaques  |
| **Bg**           | Fundo            | `#09090b` | `bg-zinc-950`     | Background global         |
| **Surface**      | Superfície       | `#18181b` | `bg-zinc-900`     | Cards, Seções secundárias |
| **Text Primary** | Texto Primário   | `#fafafa` | `text-zinc-50`    | Títulos, Texto principal  |
| **Text Sec.**    | Texto Secundário | `#a1a1aa` | `text-zinc-400`   | Legendas, Descrições      |
| **Border**       | Borda            | `#27272a` | `bg-zinc-800`     | Divisores, Inputs         |
| **Alert**        | Destaque / Aviso | `#f97316` | `text-orange-500` | Badges de alerta          |
| **Error**        | Erro / Esgotado  | `#dc2626` | `text-red-600`    | Validação, Status crítico |

### 2.2 Tipografia

Utilizamos duas famílias tipográficas para criar hierarquia visual clara entre conteúdo editorial e dados técnicos.

**Importação:**

```css
@import url("https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800;900&family=JetBrains+Mono:wght@400;700&display=swap");
```

| Uso            | Família          | Peso                        | Características                                            |
| -------------- | ---------------- | --------------------------- | ---------------------------------------------------------- |
| **Primária**   | `Inter`          | Regular (400) a Black (900) | Títulos, Corpo, UI Geral. Geralmente Uppercase em títulos. |
| **Utilitária** | `JetBrains Mono` | Regular (400), Bold (700)   | Preços, Metadados, Badges, Snippets de Código.             |

---

## 3. Componentes

### 3.1 Botões

Os botões evitam arredondamento excessivo (border-radius) e favorecem o uppercase com tracking espaçado.

**Botão Primário (Brand)**

```jsx
<button className="bg-[#ccf381] text-black font-bold uppercase py-4 px-6 hover:bg-white transition-colors tracking-widest flex items-center justify-center gap-2">
  Comprar Agora
</button>
```

**Botão Secundário (Outline)**

```jsx
<button className="bg-transparent border border-zinc-700 text-white font-bold uppercase py-4 px-6 hover:border-white transition-colors tracking-widest">
  Ver Detalhes
</button>
```

**Botão Ícone (Ghost/Solid)**

```jsx
<button className="p-4 bg-zinc-900 border border-zinc-700 text-white hover:border-[#ccf381] hover:text-[#ccf381] transition-colors rounded-full">
  <Icon />
</button>
```

### 3.2 Inputs & Formulários

Campos de entrada focam na legibilidade monospaced e feedback de foco claro.

- **Padrão:** Fundo `zinc-950`, Borda `zinc-700`.
- **Focus:** Borda muda para `#ccf381` (Brand).
- **Fonte:** Sempre `font-mono`, uppercase.

```jsx
<input
  type="text"
  className="w-full bg-zinc-950 border border-zinc-700 p-3 text-white placeholder:text-zinc-600 outline-none focus:border-[#ccf381] font-mono text-sm uppercase transition-colors"
  placeholder="DIGITE SEU EMAIL"
/>
```

### 3.3 Badges & Tags

Pequenos identificadores visuais para status de produtos.

- **Novidade:** `bg-[#ccf381] text-black`
- **Esgotado:** `bg-red-600 text-white`
- **Estrutura:** Texto 10px, Bold, Uppercase, Padding `px-2 py-1`.

---

## 4. Padrões de Interface (Patterns)

### 4.1 Product Card

O card de produto é o elemento central do e-commerce. Ele possui comportamentos complexos de hover.

**Estrutura Lógica:**

1. **Container:** Aspect Ratio 3/4, fundo `zinc-900`.
2. **Imagem:** Cobre todo o container.
3. **Overlay:** Camada preta (`bg-black/20`) que desaparece no hover.
4. **Ação Rápida:** Botão "Adicionar ao Carrinho" desliza de baixo para cima (`translate-y-full` para `translate-y-0`) no hover.
5. **Zoom:** A imagem escala para 110% suavemente.

**Snippet de Estrutura:**

```jsx
<article className="group cursor-pointer relative overflow-hidden">
  {/* Imagem + Overlay */}
  <div className="relative overflow-hidden aspect-[3/4]">
    <img
      src="..."
      className="transition-transform duration-700 group-hover:scale-110"
    />
    <div className="absolute inset-0 bg-black/20 group-hover:bg-transparent transition-all" />

    {/* Botão Slide-up */}
    <button className="absolute bottom-0 w-full bg-white text-black py-4 translate-y-full group-hover:translate-y-0 transition-transform">
      ADICIONAR
    </button>
  </div>

  {/* Meta Info */}
  <div className="flex justify-between mt-4">
    <h3 className="font-medium uppercase group-hover:text-[#ccf381]">
      Nome do Produto
    </h3>
    <span className="font-mono text-zinc-300">R$ 00,00</span>
  </div>
</article>
```

---

## 5. Configuração Tailwind

Para implementar este sistema rapidamente, adicione estas configurações ao seu `tailwind.config.js`:

```javascript
module.exports = {
  theme: {
    extend: {
      colors: {
        brand: "#ccf381",
        dark: "#09090b",
        surface: "#18181b",
      },
      fontFamily: {
        sans: ["Inter", "sans-serif"],
        mono: ["JetBrains Mono", "monospace"],
      },
      animation: {
        "fade-in": "fadeIn 0.2s ease-in-out",
      },
    },
  },
};
```

## 6. Próximos Passos para Implementação

Would you like me to:

1. **Refatorar o código original** para usar componentes reutilizáveis (ex: criar um arquivo `Button.js` e `ProductCard.js` separados baseados nesse design system)?
2. **Criar uma variante "Light Mode"** dessa documentação invertendo os valores de cor lógica?
3. **Gerar os snippets CSS puros** caso não queira usar Tailwind?
