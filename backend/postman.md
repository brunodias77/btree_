# BTree Backend - Postman Collection (Auth)

Este documento contém a descrição de todos os endpoints presentes no `AuthController.cs`, formatados de maneira fácil para importar ou criar as requisições manualmente no Postman. A base URL assumida localmente é `https://localhost:5001` (ou a porta configurada no seu launchSettings.json).

---

## 1. Register User
Cria uma nova conta de usuário.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/register`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "email": "teste@exemplo.com",
      "password": "Password123!",
      "firstName": "Bruno",
      "lastName": "Dias",
      "cpf": "12345678909",
      "birthDate": "1990-01-01"
  }
  ```
  *(Nota: `cpf` e `birthDate` são opcionais)*

---

## 2. Confirm Email
Confirma o e-mail do usuário recém-registrado, usando o token gerado pelo sistema.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/confirm-email`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "code": "TOKEN_DE_CONFIRMACAO_AQUI"
  }
  ```

---

## 3. Resend Confirmation Email
Reenvia o link/token de confirmação de conta para contas não validadas.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/resend-confirmation-email`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "email": "teste@exemplo.com"
  }
  ```

---

## 4. Login
Realiza o login e devolve os tokens de autenticação (Access e Refresh). Informações adicionais como `IpAddress` e `DeviceName` são lidas através dos Headers `X-Forwarded-For` e `User-Agent`.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/login`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
  - `User-Agent`: `PostmanRuntime/7.x.x` *(Opcional, preenchido automaticamente)*
- **Body (raw JSON)**:
  ```json
  {
      "email": "teste@exemplo.com",
      "password": "Password123!"
  }
  ```

---

## 5. Refresh Token
Gera um novo `AccessToken` e atualiza o `RefreshToken` para estender a sessão sem exigir um novo login.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/refresh`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "refreshToken": "SEU_REFRESH_TOKEN_AQUI"
  }
  ```

---

## 6. Forgot Password
Solicita um link de redefinição de senha para um endereço de e-mail. Funciona de modo silencioso (retorna 204 No Content sempre) prevenindo que invasores descubram se o e-mail existe na base nativa.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/forgot-password`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "email": "teste@exemplo.com"
  }
  ```

---

## 7. Reset Password
Redefine a senha do usuário através do link/código de 6 dígitos recebido por e-mail após a solicitação do Forgot Password.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/reset-password`
- **Auth**: None
- **Headers**:
  - `Content-Type`: `application/json`
- **Body (raw JSON)**:
  ```json
  {
      "code": "123456",
      "newPassword": "NovaPasswordForte123!"
  }
  ```

---

## 8. Change Password
Permite que um usuário autenticado altere sua própria senha informando a senha atual. Todas as outras sessões ativas serão invalidadas.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/change-password`
- **Auth**: `Bearer Token` (necessita estar autenticado)
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body (raw JSON)**:
  ```json
  {
      "currentPassword": "SenhaAntiga123!",
      "newPassword": "NovaPasswordForte456@",
      "confirmNewPassword": "NovaPasswordForte456@"
  }
  ```

---

## 9. Logout
Encerra a sessão, invalidando e revogando o fluxo para aquele Refresh Token específico.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/auth/logout`
- **Auth**: Bearer Token
  - Preencher a aba **Authorization** -> **Type**: Bearer Token -> **Token**: `{{AccessToken}}`
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer SEU_ACCESS_TOKEN_AQUI` (se não usar a aba genérica do Postman)
- **Body (raw JSON)**:
  ```json
  {
      "refreshToken": "SEU_REFRESH_TOKEN_AQUI"
  }
  ```

---

## 10. Create Category
Cria uma nova categoria no catálogo. Requer autenticação com perfil de `Admin`. O `slug` será gerado automaticamente a partir do nome se não for fornecido. A `parentId` permite aninhar categorias em até 5 níveis de profundidade.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/categories`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body (raw JSON)**:
  ```json
  {
      "name": "Eletrônicos",
      "slug": "eletronicos",
      "parentId": null,
      "description": "Categoria principal de eletrônicos",
      "imageUrl": "https://btree.com/images/eletronicos.jpg",
      "metaTitle": "Comprar Eletrônicos | BTree",
      "metaDescription": "As melhores ofertas em eletrônicos.",
      "sortOrder": 1
  }
  ```
  *(Nota: apenas `name` e `sortOrder` são obrigatórios)*

---

## 11. Update Category
Atualiza os dados cadastrais e de SEO de uma categoria existente. Requer autenticação com perfil de `Admin`. Se o `slug` for fornecido ou alterado, a hierarquia (`Path`) será recalculada automaticamente se aplicável.

- **Method**: `PUT`
- **URL**: `{{baseUrl}}/api/categories/{{categoryId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body (raw JSON)**:
  ```json
  {
      "name": "Eletrônicos e Smartphones",
      "slug": "eletronicos-e-smartphones",
      "description": "A melhor categoria de eletrônicos do mercado.",
      "imageUrl": "https://btree.com/images/eletronicos-v2.jpg",
      "metaTitle": "Comprar Eletrônicos e Smartphones | BTree",
      "metaDescription": "As melhores ofertas em eletrônicos e smartphones atualizadas.",
      "sortOrder": 2
  }
  ```
  *(Nota: apenas `name` e `sortOrder` são obrigatórios)*

---

## 12. Delete Category
Exclui logicamente (Soft Delete) uma categoria do catálogo. A operação falhará caso a categoria possua subcategorias atreladas ou produtos contidos nela, garantindo a integridade dos dados. Requer autenticação com perfil de `Admin`.

- **Method**: `DELETE`
- **URL**: `{{baseUrl}}/api/categories/{{categoryId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body**: *Nenhum (empty)*

---

## 13. Get Categories
Retorna uma lista paginada de categorias do catálogo. Focada em otimização (Query), não requer perfis de administrador. Permite filtrar por termo de busca, status ou hierarquia de pais e alterar o tamanho da página.

- **Method**: `GET`
- **URL**: `{{baseUrl}}/api/categories?PageNumber=1&PageSize=10&SearchTerm=&ParentId=&IsActive=`
- **Auth**: None (ou dependendo da sua necessidade, preencher Bearer)
- **Parameters (Query)**:
  - `PageNumber` (int, default: 1)
  - `PageSize` (int, default: 10, max: 100)
  - `SearchTerm` (string, opcional)
  - `ParentId` (guid, opcional)
  - `IsActive` (bool, opcional)
- **Headers**:
  - `Content-Type`: `application/json`
- **Body**: *Nenhum (empty)*

---

## 14. Create Brand
Cria uma nova marca no catálogo. Requer um nome único (unicidade garantida pelo backend). O `Slug` correspondente é gerado automaticamente a partir do nome pelo serviço de domínio. O perfil necessário para a ação é o de `Admin`.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/brands`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body**:
  ```json
  {
      "name": "Apple",
      "description": "Empresa multinacional norte-americana",
      "logoUrl": "https://example.com/apple-logo.png",
      "isActive": true
  }
  ```
  *(Nota: As propriedades `description` e `logoUrl` são opcionais, enquanto o `name` é obrigatório com tamanho entre 2 e 100 limitados pelo FluentValidation)*

---

## 15. Update Brand
Atualiza os dados de uma marca existente no catálogo. O ID é submetido via path e os dados no payload. Se houver alteração de nome validada, um novo slug correspondente será gerado sem quebrar a exclusividade. Exige perfil `Admin`.

- **Method**: `PUT`
- **URL**: `{{baseUrl}}/api/brands/{{brandId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body**:
  ```json
  {
      "name": "Apple Inc.",
      "description": "Empresa Apple atualizada",
      "logoUrl": "https://example.com/apple-logo-new.png",
      "isActive": false
  }
  ```

---

## 16. Delete Brand
Exclui logicamente uma marca do catálogo (Soft Delete). A operação não é permitida caso a marca possua produtos vinculados. Exige perfil `Admin`.

- **Method**: `DELETE`
- **URL**: `{{baseUrl}}/api/brands/{{brandId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Authorization`: `Bearer {{accessToken}}`
- **Body**: *Nenhum (empty)*

---

## 17. Get Brands (Paginado)
Consulta paginada de marcas com filtros opcionais por nome e status de ativação. Endpoint público (sem autenticação obrigatória).

- **Method**: `GET`
- **URL**: `{{baseUrl}}/api/brands?PageNumber=1&PageSize=10`
- **Parâmetros de Query opcionais**:
  - `SearchTerm` (string): Busca parcial no nome da marca.
  - `IsActive` (bool): Filtrar por marcas ativas (`true`) ou inativas (`false`).
- **Exemplo com filtros**: `{{baseUrl}}/api/brands?PageNumber=1&PageSize=10&SearchTerm=apple&IsActive=true`

---

## 18. Upload Image
Faz o upload de uma imagem para o servidor, organizando-a por entidade (brands, categories, products, etc). Retorna a URL relativa da imagem salva.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/images/upload?entity=brands`
- **Auth**: Bearer Token
- **Headers**:
  - `Authorization`: `Bearer {{accessToken}}`
- **Body** (form-data):
  - `file`: (File) - O arquivo de imagem a ser enviado.
- **Resposta (200 OK)**:
  ```json
  {
      "success": true,
      "data": {
          "url": "/uploads/brands/guid_da_imagem.png"
      },
      "timestamp": "2024-03-06T15:15:00Z"
  }
  ```

---

## 19. Delete Image
Remove uma imagem do servidor através da sua URL relativa.

- **Method**: `DELETE`
- **URL**: `{{baseUrl}}/api/images?url=/uploads/brands/guid_da_imagem.png`
- **Auth**: Bearer Token
- **Headers**:
  - `Authorization`: `Bearer {{accessToken}}`
- **Resposta (200 OK)**:
  ```json
  {
      "success": true,
      "message": "Imagem removida com sucesso.",
      "timestamp": "2024-03-06T15:16:00Z"
  }
  ```

---

## 20. Create Product
Cria um novo produto completo no catálogo. Valida deduplicação de SKU, a existência da respectiva Categoria e Marca, e processa dados complementares como Dimensões, Código de Barras e SEO Metadata. O Slug é construído automaticamente baseado no nome. Exige perfil de `Admin`.

- **Method**: `POST`
- **URL**: `{{baseUrl}}/api/products`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body** (raw JSON):
  ```json
  {
      "name": "IPhone 15 Pro Max 256GB",
      "description": "O smartphone mais avançado da Apple com chip A17 Pro e titânio.",
      "brandId": "11111111-1111-1111-1111-111111111111",
      "categoryId": "22222222-2222-2222-2222-222222222222",
      "priceAmount": 7500.00,
      "priceCurrency": "BRL",
      "sku": "IPH15PM-256-BLK",
      "barcode": "0194253768128",
      "initialStock": 50,
      "weightInGrams": 221,
      "lengthInCm": 15.99,
      "widthInCm": 7.67,
      "heightInCm": 0.83,
      "seoTitle": "Comprar IPhone 15 Pro Max 256GB - Melhor Preço",
      "seoDescription": "Aproveite a oferta de IPhone 15 Pro Max 256GB. Original, com garantia Apple."
  }
  ```
  *(Nota: Campos de SEO `seoTitle`, `seoDescription`, e de dimensões junto com o `barcode` e `initialStock` são opcionais. A `brandId` e `categoryId` devem corresponder a Guid's válidos existentes na Base. `priceCurrency` caso omitido será BRL).*

---

## 21. Update Product
Atualiza os dados de um produto existente. O ID deve vir mapeado na Rota (`{{productId}}`). Campos como SKU e Name (que regera o Slug) possuem checagem garantidora de unicidade para evitar colisões no catálogo. Exige perfil de `Admin`. *(Atualização de estoque ocorre por endpoints apartados e independentes)*.

- **Method**: `PUT`
- **URL**: `{{baseUrl}}/api/products/{{productId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body** (raw JSON):
  ```json
  {
      "name": "IPhone 15 Pro Max 512GB",
      "description": "Edição mais avançada do smartphone Apple, alterada e expandida.",
      "brandId": "11111111-1111-1111-1111-111111111111",
      "categoryId": "22222222-2222-2222-2222-222222222222",
      "priceAmount": 8700.00,
      "priceCurrency": "BRL",
      "sku": "IPH15PM-512-BLK",
      "barcode": "0194253768128",
      "weightInGrams": 221,
      "lengthInCm": 15.99,
      "widthInCm": 7.67,
      "heightInCm": 0.83,
      "seoTitle": "Comprar IPhone 15 Pro Max 512GB - Melhor Preço",
      "seoDescription": "Aproveite a oferta do IPhone 15 Pro Max de 512GB atualizada."
  ```
  *(Nota: Todos campos de Dimensões e SEO também são opcionais aqui, assim como o `barcode`. Mas regras rígidas como a formatação sem espaços do SKU continuam implacáveis).*

---

## 22. Delete Product
Realiza a exclusão lógica (Soft Delete) de um produto do catálogo. Ele será ocultado visualmente e desativado. Porém, seu rastro e dependência histórica permanece intacto no Banco de Dados. Lançará um erro 400 Bad Request se tentarem deletar um produto com estoque preso ativamente (`ReservedStock > 0`). Chamadas duplicadas contra um produto já deletado devolvem 204 No Content para garantir Idempotência. Requer perfil de `Admin`.

- **Method**: `DELETE`
- **URL**: `{{baseUrl}}/api/products/{{productId}}`
- **Auth**: Bearer Token
- **Headers**:
  - `Content-Type`: `application/json`
  - `Authorization`: `Bearer {{accessToken}}`
- **Body**: *Nenhum (empty)*

---

## 23. Get Products
Realiza a listagem paginada e projetada contendo múltiplas dinâmicas de filtros (`SearchTerm`, `BrandId`, `CategoryId`, etc.) focado em alta velocidade por carregar objetos estáticos simplificados para GridView, retornando com formato enriquecido usando metadados e estrutura `PagedResult`. Não requer token e é endpoint Público, com exceção da busca de Status `Draft` ou `Inactive` e exibe array vazio caso os parâmetros gerem dead-ends.

- **Method**: `GET`
- **URL**: `{{baseUrl}}/api/products?pageNumber=1&pageSize=10&sortBy=price&sortDirection=desc&searchTerm=IPhone`
- **Auth**: Nenhum (Opcional)
- **QueryParams**:
  - `pageNumber`: int (default: 1)
  - `pageSize`: int (default: 10, max: 100)
  - `sortBy`: string ("Name", "Price", "CreatedAt", "Stock")
  - `sortDirection`: string ("asc", "desc")
  - `searchTerm`: string (Busca no nome, categoria ou código do Sku)
  - `brandId`: guid
  - `categoryId`: guid
  - `status`: string
- **Headers**:
  - `Content-Type`: `application/json`
- **Body**: *Nenhum (empty)*

---
