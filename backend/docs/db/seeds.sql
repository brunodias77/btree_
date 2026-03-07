-- =============================================
-- BCommerce - Seeds de Dados Iniciais
-- =============================================
-- Executar após as migrations do EF Core
-- Senha dos usuários: Teste@123*
-- =============================================

-- =============================================
-- 1. ROLES (Papéis)
-- =============================================
INSERT INTO users.asp_net_roles ("Id", "Name", "NormalizedName", "Description", "CreatedAt", "ConcurrencyStamp")
VALUES 
    ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Customer', 'CUSTOMER', 'Cliente padrão da plataforma', NOW(), gen_random_uuid()::text),
    ('b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a22', 'Admin', 'ADMIN', 'Administrador do sistema', NOW(), gen_random_uuid()::text)
ON CONFLICT ("Id") DO NOTHING;

-- =============================================
-- 2. USERS (Usuários do Identity)
-- =============================================
-- Hash da senha "Teste@123*" gerado pelo ASP.NET Identity (PBKDF2 com HMAC-SHA256)
-- IMPORTANTE: Em produção, use o UserManager do Identity para criar usuários

INSERT INTO users.asp_net_users (
    "Id", 
    "UserName", 
    "NormalizedUserName", 
    "Email", 
    "NormalizedEmail", 
    "EmailConfirmed", 
    "PasswordHash", 
    "SecurityStamp", 
    "ConcurrencyStamp", 
    "PhoneNumber",
    "PhoneNumberConfirmed", 
    "TwoFactorEnabled", 
    "LockoutEnabled", 
    "AccessFailedCount",
    "CreatedAt",
    "UpdatedAt"
)
VALUES 
    -- Usuário Customer: bruno@teste.com
    (
        'c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33',
        'bruno@teste.com',
        'BRUNO@TESTE.COM',
        'bruno@teste.com',
        'BRUNO@TESTE.COM',
        true,
        'AQAAAAIAAYagAAAAEKI0dbp9uwU8OGF+UBW6PAxl9VNqoz/6bpeAqbjHJv4MrDWDtckIFEvr5UP+R9XrZA==',
        'RVOFKP3WFZGQ5DQVVGVHKYJXFRBMGHZD',
        gen_random_uuid()::text,
        '11999999999',
        false,
        false,
        true,
        0,
        NOW(),
        NOW()
    ),
    -- Usuário Admin: bruno@admin.com
    (
        'd0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44',
        'bruno@admin.com',
        'BRUNO@ADMIN.COM',
        'bruno@admin.com',
        'BRUNO@ADMIN.COM',
        true,
        'AQAAAAIAAYagAAAAEKI0dbp9uwU8OGF+UBW6PAxl9VNqoz/6bpeAqbjHJv4MrDWDtckIFEvr5UP+R9XrZA==',
        'XWOFKP3WFZGQ5DQVVGVHKYJXFRBMGHAD',
        gen_random_uuid()::text,
        '11988888888',
        false,
        false,
        true,
        0,
        NOW(),
        NOW()
    )
ON CONFLICT ("Id") DO NOTHING;

-- =============================================
-- 3. USER ROLES (Associação Usuário-Role)
-- =============================================
INSERT INTO users.asp_net_user_roles ("UserId", "RoleId")
VALUES 
    -- bruno@teste.com -> Customer
    ('c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33', 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11'),
    -- bruno@admin.com -> Admin
    ('d0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44', 'b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a22'),
    -- bruno@admin.com -> Customer (admin também é customer)
    ('d0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44', 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11')
ON CONFLICT ("UserId", "RoleId") DO NOTHING;

-- =============================================
-- 4. PROFILES (Perfis dos Usuários)
-- =============================================
INSERT INTO users.profiles (
    id,
    user_id,
    first_name,
    last_name,
    display_name,
    avatar_url,
    birth_date,
    gender,
    cpf,
    preferred_language,
    preferred_currency,
    newsletter_subscribed,
    accepted_terms_at,
    accepted_privacy_at,
    version,
    created_at,
    updated_at,
    deleted_at
)
VALUES 
    -- Perfil do bruno@teste.com
    (
        'e0eebc99-9c0b-4ef8-bb6d-6bb9bd380a55',
        'c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33',
        'Bruno',
        'Customer',
        'Bruno Customer',
        NULL,
        '1990-05-15',
        'Male',
        '123.456.789-00',
        'pt-BR',
        'BRL',
        true,
        NOW(),
        NOW(),
        1,
        NOW(),
        NOW(),
        NULL
    ),
    -- Perfil do bruno@admin.com
    (
        'f0eebc99-9c0b-4ef8-bb6d-6bb9bd380a66',
        'd0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44',
        'Bruno',
        'Admin',
        'Bruno Admin',
        NULL,
        '1985-10-20',
        'Male',
        '987.654.321-00',
        'pt-BR',
        'BRL',
        false,
        NOW(),
        NOW(),
        1,
        NOW(),
        NOW(),
        NULL
    )
ON CONFLICT (id) DO NOTHING;

-- =============================================
-- 5. NOTIFICATION PREFERENCES (Preferências de Notificação)
-- =============================================
INSERT INTO users.notification_preferences (
    id,
    user_id,
    email_enabled,
    push_enabled,
    sms_enabled,
    order_updates,
    promotions,
    price_drops,
    back_in_stock,
    newsletter,
    created_at,
    updated_at,
    "Version"
)
VALUES 
    -- Preferências do bruno@teste.com
    (
        'a1eebc99-9c0b-4ef8-bb6d-6bb9bd380a77',
        'c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33',
        true,
        true,
        false,
        true,
        true,
        true,
        true,
        true,
        NOW(),
        NOW(),
        1
    ),
    -- Preferências do bruno@admin.com
    (
        'b1eebc99-9c0b-4ef8-bb6d-6bb9bd380a88',
        'd0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44',
        true,
        true,
        false,
        true,
        false,
        false,
        false,
        false,
        NOW(),
        NOW(),
        1
    )
ON CONFLICT (id) DO NOTHING;

-- =============================================
-- Resumo dos dados criados:
-- =============================================
-- ROLES:
--   - Customer (ID: a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11)
--   - Admin (ID: b0eebc99-9c0b-4ef8-bb6d-6bb9bd380a22)
--
-- USUÁRIOS:
--   - bruno@teste.com (ID: c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33) -> Role: Customer
--   - bruno@admin.com (ID: d0eebc99-9c0b-4ef8-bb6d-6bb9bd380a44) -> Roles: Admin, Customer
--
-- SENHA PADRÃO: Teste@123*
-- =============================================

-- =============================================
-- 6. CATALOG CATEGORIES (Streetwear/Minimalism Structure)
-- =============================================
-- Estrutura:
-- 1. Parte de Cima (Tops)
--    - T-Shirts
--      - Oversized, Boxy Fit, Basics
--    - Moletons & Tricôs
--      - Hoodies, Crewnecks, Sweaters
--    - Camisas
--      - Overshirts, Flanelas
-- 2. Parte de Baixo (Bottoms)
--    - Calças
--      - Cargo, Parachute/Nylon, Alfaiataria Relaxed, Jeans
--    - Shorts
--      - Sweatshorts, Cargo Shorts
-- 3. Outerwear
--    - Jaquetas
--      - Puffer, Bomber, Windbreakers, Coletes
-- 4. Acessórios
--    - Headwear
--      - Beanies, Bonés
--    - Bolsas
--      - Shoulder Bags, Tote Bags
--    - Meias

-- ========================================
-- SEEDS: CATALOG.CATEGORIES (CORRIGIDO FINAL)
-- ========================================
INSERT INTO catalog.categories (
    id, parent_id, name, slug, description, depth, is_active, sort_order, 
    created_at, updated_at, version -- Adicionado explicitamente
)
VALUES
    -- 1. Parte de Cima (ROOT)
    ('5a8c9e0a-1111-4444-8888-000000000001', NULL, 'Parte de Cima', 'parte-de-cima', 'Tops, camisetas e agasalhos', 0, true, 1, NOW(), NOW(), 1),
        -- 1.1 T-Shirts
        ('5a8c9e0a-1111-4444-8888-100000000001', '5a8c9e0a-1111-4444-8888-000000000001', 'T-Shirts', 't-shirts', 'Camisetas essenciais', 1, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-110000000001', '5a8c9e0a-1111-4444-8888-100000000001', 'Oversized', 'oversized', 'Modelagem ampla e moderna', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-110000000002', '5a8c9e0a-1111-4444-8888-100000000001', 'Boxy Fit', 'boxy-fit', 'Corte quadrado e mais curto', 2, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-110000000003', '5a8c9e0a-1111-4444-8888-100000000001', 'Basics', 'basics', 'Cores sólidas e essenciais', 2, true, 3, NOW(), NOW(), 1),
        -- 1.2 Moletons & Tricôs
        ('5a8c9e0a-1111-4444-8888-100000000002', '5a8c9e0a-1111-4444-8888-000000000001', 'Moletons & Tricôs', 'moletons-tricos', 'Conforto e estilo', 1, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-120000000001', '5a8c9e0a-1111-4444-8888-100000000002', 'Hoodies', 'hoodies', 'Com capuz, lisos e pesados', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-120000000002', '5a8c9e0a-1111-4444-8888-100000000002', 'Crewnecks', 'crewnecks', 'Gola careca clássica', 2, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-120000000003', '5a8c9e0a-1111-4444-8888-100000000002', 'Sweaters', 'sweaters', 'Tricô minimalista', 2, true, 3, NOW(), NOW(), 1),
        -- 1.3 Camisas
        ('5a8c9e0a-1111-4444-8888-100000000003', '5a8c9e0a-1111-4444-8888-000000000001', 'Camisas', 'camisas', 'Camisas casuais e sobreposições', 1, true, 3, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-130000000001', '5a8c9e0a-1111-4444-8888-100000000003', 'Overshirts', 'overshirts', 'Para usar como jaqueta leve', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-1111-4444-8888-130000000002', '5a8c9e0a-1111-4444-8888-100000000003', 'Flanelas', 'flanelas', 'Xadrez e texturas', 2, true, 2, NOW(), NOW(), 1),

    -- 2. Parte de Baixo (ROOT)
    ('5a8c9e0a-2222-4444-8888-000000000001', NULL, 'Parte de Baixo', 'parte-de-baixo', 'Bottoms, calças e shorts', 0, true, 2, NOW(), NOW(), 1),
        -- 2.1 Calças
        ('5a8c9e0a-2222-4444-8888-200000000001', '5a8c9e0a-2222-4444-8888-000000000001', 'Calças', 'calcas', 'Do cargo à alfaiataria', 1, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-210000000001', '5a8c9e0a-2222-4444-8888-200000000001', 'Cargo', 'cargo', 'Design techwear utilitário', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-210000000002', '5a8c9e0a-2222-4444-8888-200000000001', 'Parachute / Nylon', 'parachute-nylon', 'Tecidos sintéticos modernos', 2, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-210000000003', '5a8c9e0a-2222-4444-8888-200000000001', 'Alfaiataria Relaxed', 'alfaiataria-relaxed', 'Corte social solto', 2, true, 3, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-210000000004', '5a8c9e0a-2222-4444-8888-200000000001', 'Jeans', 'jeans', 'Wide leg e straight', 2, true, 4, NOW(), NOW(), 1),
        -- 2.2 Shorts
        ('5a8c9e0a-2222-4444-8888-200000000002', '5a8c9e0a-2222-4444-8888-000000000001', 'Shorts', 'shorts', 'Conforto para dias quentes', 1, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-220000000001', '5a8c9e0a-2222-4444-8888-200000000002', 'Sweatshorts', 'sweatshorts', 'Shorts de moletom', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-2222-4444-8888-220000000002', '5a8c9e0a-2222-4444-8888-200000000002', 'Cargo Shorts', 'cargo-shorts', 'Utilitários curtos', 2, true, 2, NOW(), NOW(), 1),

    -- 3. Outerwear (ROOT)
    ('5a8c9e0a-3333-4444-8888-000000000001', NULL, 'Outerwear', 'outerwear', 'Terceira peça e layering', 0, true, 3, NOW(), NOW(), 1),
        -- 3.1 Jaquetas
        ('5a8c9e0a-3333-4444-8888-300000000001', '5a8c9e0a-3333-4444-8888-000000000001', 'Jaquetas', 'jaquetas', 'Proteção com estilo', 1, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-3333-4444-8888-310000000001', '5a8c9e0a-3333-4444-8888-300000000001', 'Puffer', 'puffer', 'Acolchoadas para inverno', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-3333-4444-8888-310000000002', '5a8c9e0a-3333-4444-8888-300000000001', 'Bomber', 'bomber', 'Clássica e versátil', 2, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-3333-4444-8888-310000000003', '5a8c9e0a-3333-4444-8888-300000000001', 'Windbreakers', 'windbreakers', 'Corta-vento esportivo', 2, true, 3, NOW(), NOW(), 1),
            ('5a8c9e0a-3333-4444-8888-310000000004', '5a8c9e0a-3333-4444-8888-300000000001', 'Coletes', 'coletes', 'Puffers ou táticos', 2, true, 4, NOW(), NOW(), 1),

    -- 4. Acessórios (ROOT)
    ('5a8c9e0a-4444-4444-8888-000000000001', NULL, 'Acessórios', 'acessorios', 'Essentials minimalistas', 0, true, 4, NOW(), NOW(), 1),
        -- 4.1 Headwear
        ('5a8c9e0a-4444-4444-8888-400000000001', '5a8c9e0a-4444-4444-8888-000000000001', 'Headwear', 'headwear', 'Para a cabeça', 1, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-4444-4444-8888-410000000001', '5a8c9e0a-4444-4444-8888-400000000001', 'Beanies', 'beanies', 'Toucas curtas/pescador', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-4444-4444-8888-410000000002', '5a8c9e0a-4444-4444-8888-400000000001', 'Bonés', 'bones', 'Dad hats e 5-panel', 2, true, 2, NOW(), NOW(), 1),
        -- 4.2 Bolsas
        ('5a8c9e0a-4444-4444-8888-400000000002', '5a8c9e0a-4444-4444-8888-000000000001', 'Bolsas', 'bolsas', 'Bags funcionais', 1, true, 2, NOW(), NOW(), 1),
            ('5a8c9e0a-4444-4444-8888-420000000001', '5a8c9e0a-4444-4444-8888-400000000002', 'Shoulder Bags', 'shoulder-bags', 'Crossbody compactas', 2, true, 1, NOW(), NOW(), 1),
            ('5a8c9e0a-4444-4444-8888-420000000002', '5a8c9e0a-4444-4444-8888-400000000002', 'Tote Bags', 'tote-bags', 'Ecobags reforçadas', 2, true, 2, NOW(), NOW(), 1),
        -- 4.3 Meias
        ('5a8c9e0a-4444-4444-8888-400000000003', '5a8c9e0a-4444-4444-8888-000000000001', 'Meias', 'meias', 'Cano alto e lisas', 1, true, 3, NOW(), NOW(), 1)
ON CONFLICT (id) DO NOTHING;

-- ========================================
-- ATUALIZAÇÃO RECURSIVA DE PATHS
-- (Este bloco permanece igual)
-- ========================================
WITH RECURSIVE CategoryTree AS (
    SELECT 
        id, 
        id::text AS built_path 
    FROM catalog.categories 
    WHERE parent_id IS NULL

    UNION ALL

    SELECT 
        c.id, 
        ct.built_path || '/' || c.id::text
    FROM catalog.categories c
    INNER JOIN CategoryTree ct ON c.parent_id = ct.id
)
UPDATE catalog.categories c
SET path = ct.built_path
FROM CategoryTree ct
WHERE c.id = ct.id
AND (c.path IS NULL OR c.path != ct.built_path);





-- =============================================
-- BCommerce - Seeds Complementares (Catalog, Coupons, Orders)
-- Execute APÓS o seeds.sql original
-- =============================================

-- =============================================
-- 6. CATALOG: BRANDS (Marcas Streetwear)
-- =============================================
INSERT INTO catalog.brands (id, name, slug, description, is_active, version, created_at, updated_at)
VALUES
    ('b1eebc99-1111-4444-8888-000000000001', 'Urban Void', 'urban-void', 'Techwear e funcionalidade futurista.', true, 1, NOW(), NOW()),
    ('b1eebc99-1111-4444-8888-000000000002', 'Concrete Soul', 'concrete-soul', 'Básicos essenciais de alta gramatura.', true, 1, NOW(), NOW()),
    ('b1eebc99-1111-4444-8888-000000000003', 'Night Shift', 'night-shift', 'Estética underground e oversized.', true, 1, NOW(), NOW())
ON CONFLICT DO NOTHING;

-- =============================================
-- 2. CATALOG: PRODUCTS (Produtos)
-- IDs fixos para relacionar com imagens e estoque
-- =============================================
INSERT INTO catalog.products (
    id, category_id, brand_id, sku, slug, name, 
    short_description, full_description, price, cost_price, 
    stock, weight_grams, height_cm, width_cm, length_cm,
    status, is_featured, attributes, tags, version, created_at, updated_at
)
VALUES
    -- 1. T-Shirt Oversized (Concrete Soul)
    (
        '11eebc99-0000-0000-0000-000000000001', -- Corrigido: 'p' trocado por '1'
        '5a8c9e0a-1111-4444-8888-110000000001', 
        'b1eebc99-1111-4444-8888-000000000002', 
        'TSH-HVY-BLK-L', 'heavyweight-oversized-tee-black', 'Heavyweight Oversized Tee Black',
        'Camiseta 260gsm com modelagem quadrada.',
        'Feita para durar. Algodão penteado fio 30.1, gramatura alta, gola de 3cm.',
        129.90, 45.00,
        50, 300, 2.0, 25.0, 35.0,
        'ACTIVE', true,
        '{"Color": "Black", "Size": "L", "Fabric": "100% Cotton"}',
        ARRAY['basics', 'oversized', 'black'],
        1, NOW(), NOW()
    ),
    -- 2. Hoodie Boxy (Night Shift)
    (
        '11eebc99-0000-0000-0000-000000000002', -- Corrigido: 'p' trocado por '1'
        '5a8c9e0a-1111-4444-8888-120000000001', 
        'b1eebc99-1111-4444-8888-000000000003', 
        'HDD-BOX-ASH-M', 'boxy-hoodie-ash-grey', 'Boxy Hoodie Ash Grey',
        'Moletom pesado sem cordão.',
        'Modelagem curta e larga (boxy). Interior felpudo, capuz duplo estruturado.',
        289.90, 110.00,
        30, 800, 10.0, 40.0, 50.0,
        'ACTIVE', true,
        '{"Color": "Grey", "Size": "M", "Fit": "Boxy"}',
        ARRAY['winter', 'hoodie', 'streetwear'],
        1, NOW(), NOW()
    ),
    -- 3. Cargo Pants (Urban Void)
    (
        '11eebc99-0000-0000-0000-000000000003', -- Corrigido: 'p' trocado por '1'
        '5a8c9e0a-2222-4444-8888-210000000001', 
        'b1eebc99-1111-4444-8888-000000000001', 
        'PNT-CRG-OLV-32', 'tech-cargo-v2-olive', 'Tech Cargo V2 Olive',
        'Calça utilitária com 6 bolsos e ajuste no tornozelo.',
        'Tecido Ripstop resistente à água. Zíperes YKK e fitas ajustáveis.',
        349.90, 140.00,
        25, 500, 5.0, 30.0, 40.0,
        'ACTIVE', false,
        '{"Color": "Olive", "Size": "32", "Material": "Ripstop"}',
        ARRAY['techwear', 'cargo', 'utility'],
        1, NOW(), NOW()
    ),
    -- 4. Beanie (Concrete Soul)
    (
        '11eebc99-0000-0000-0000-000000000004', -- Corrigido: 'p' trocado por '1'
        '5a8c9e0a-4444-4444-8888-410000000001', 
        'b1eebc99-1111-4444-8888-000000000002', 
        'ACC-BNI-ORG', 'fisherman-beanie-orange', 'Fisherman Beanie Orange',
        'Touca estilo pescador curta.',
        'Tricô canelado, etiqueta minimalista na lateral.',
        79.90, 20.00,
        100, 100, 2.0, 15.0, 15.0,
        'ACTIVE', false,
        '{"Color": "Orange", "Size": "One Size"}',
        ARRAY['accessories', 'headwear'],
        1, NOW(), NOW()
    );

-- =============================================
-- 7. CATALOG: IMAGES (Imagens Placeholder)
-- =============================================
INSERT INTO catalog.product_images (id, product_id, url, alt_text, is_primary, sort_order, created_at)
VALUES
    -- 1. T-Shirt Front
    ('22eebc99-0000-0000-0000-000000000001', '11eebc99-0000-0000-0000-000000000001', 'https://placehold.co/600x800/1a1a1a/ffffff?text=T-Shirt+Front', 'T-Shirt Black Front', true, 1, NOW()),
    
    -- 2. T-Shirt Back
    ('22eebc99-0000-0000-0000-000000000002', '11eebc99-0000-0000-0000-000000000001', 'https://placehold.co/600x800/1a1a1a/ffffff?text=T-Shirt+Back', 'T-Shirt Black Back', false, 2, NOW()),
    
    -- 3. Hoodie
    ('22eebc99-0000-0000-0000-000000000003', '11eebc99-0000-0000-0000-000000000002', 'https://placehold.co/600x800/e0e0e0/333333?text=Hoodie+Grey', 'Hoodie Grey', true, 1, NOW()),
    
    -- 4. Cargo
    ('22eebc99-0000-0000-0000-000000000004', '11eebc99-0000-0000-0000-000000000003', 'https://placehold.co/600x800/3e4a38/ffffff?text=Cargo+Olive', 'Cargo Pants Olive', true, 1, NOW()),
    
    -- 5. Beanie
    ('22eebc99-0000-0000-0000-000000000005', '11eebc99-0000-0000-0000-000000000004', 'https://placehold.co/600x800/ff6b00/ffffff?text=Beanie', 'Beanie Orange', true, 1, NOW());

-- =============================================
-- 8. CATALOG: STOCK MOVEMENTS (Estoque Inicial)
-- =============================================
INSERT INTO catalog.stock_movements (
    product_id, movement_type, quantity, stock_before, stock_after, reason, created_at
)
VALUES
    ('p1eebc99-0000-0000-0000-000000000001', 'IN', 50, 0, 50, 'Estoque Inicial', NOW()),
    ('p1eebc99-0000-0000-0000-000000000002', 'IN', 30, 0, 30, 'Estoque Inicial', NOW()),
    ('p1eebc99-0000-0000-0000-000000000003', 'IN', 25, 0, 25, 'Estoque Inicial', NOW()),
    ('p1eebc99-0000-0000-0000-000000000004', 'IN', 100, 0, 100, 'Estoque Inicial', NOW());

-- =============================================
-- 9. COUPONS (Cupons de Desconto)
-- =============================================
INSERT INTO coupons.coupons (
    code, name, description, discount_type, discount_value, 
    min_purchase_amount, valid_from, valid_until, max_uses, 
    status, created_at, updated_at
)
VALUES
    ('BEMVINDO10', 'Primeira Compra', '10% de desconto na primeira compra', 'PERCENTAGE', 10.00, 
     100.00, NOW(), NOW() + INTERVAL '1 year', 1000, 
     'ACTIVE', NOW(), NOW()),
     
    ('FRETEGRATIS', 'Frete Grátis', 'Frete grátis acima de R$ 400', 'FREE_SHIPPING', 0, 
     400.00, NOW(), NOW() + INTERVAL '6 months', NULL, 
     'ACTIVE', NOW(), NOW());

-- =============================================
-- 10. ORDERS (Histórico para o Cliente Bruno)
-- Cria um pedido já entregue para popular o dashboard
-- =============================================

-- 10.1 Criar o Pedido
INSERT INTO orders.orders (
    id, order_number, user_id, 
    subtotal, discount_amount, shipping_amount, total,
    status, payment_method, shipping_method,
    shipping_address, billing_address,
    paid_at, shipped_at, delivered_at,
    created_at, updated_at
)
VALUES (
    'o1eebc99-9999-0000-0000-000000000001', '26-000001', 
    'c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33', -- ID do bruno@teste.com (do seeds.sql)
    369.80, 0, 25.00, 394.80,
    'DELIVERED', 'CREDIT_CARD', 'STANDARD',
    '{"street": "Rua das Flores", "number": "123", "city": "São Paulo", "state": "SP", "zip": "01000-000"}',
    '{"street": "Rua das Flores", "number": "123", "city": "São Paulo", "state": "SP", "zip": "01000-000"}',
    NOW() - INTERVAL '10 days', NOW() - INTERVAL '8 days', NOW() - INTERVAL '5 days',
    NOW() - INTERVAL '10 days', NOW() - INTERVAL '5 days'
);

-- 10.2 Itens do Pedido (1 Hoodie + 1 Beanie)
INSERT INTO orders.items (order_id, product_id, product_snapshot, unit_price, quantity, subtotal, created_at)
VALUES
    (
        'o1eebc99-9999-0000-0000-000000000001',
        'p1eebc99-0000-0000-0000-000000000002', -- Hoodie
        '{"name": "Boxy Hoodie Ash Grey", "sku": "HDD-BOX-ASH-M"}',
        289.90, 1, 289.90, NOW() - INTERVAL '10 days'
    ),
    (
        'o1eebc99-9999-0000-0000-000000000001',
        'p1eebc99-0000-0000-0000-000000000004', -- Beanie
        '{"name": "Fisherman Beanie Orange", "sku": "ACC-BNI-ORG"}',
        79.90, 1, 79.90, NOW() - INTERVAL '10 days'
    );

-- 10.3 Pagamento do Pedido
INSERT INTO payments.payments (
    id, order_id, user_id, idempotency_key,
    amount, currency, payment_method_type,
    gateway_name, gateway_transaction_id, status,
    authorized_at, captured_at, created_at, updated_at
)
VALUES (
    uuid_generate_v4(),
    'o1eebc99-9999-0000-0000-000000000001',
    'c0eebc99-9c0b-4ef8-bb6d-6bb9bd380a33',
    'idemp-key-0001',
    394.80, 'BRL', 'CREDIT_CARD',
    'STRIPE', 'ch_123456789', 'CAPTURED',
    NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days',
    NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days'
);

-- 10.4 Histórico de Status do Pedido
INSERT INTO orders.status_history (order_id, from_status, to_status, created_at)
VALUES
    ('o1eebc99-9999-0000-0000-000000000001', NULL, 'PENDING', NOW() - INTERVAL '10 days'),
    ('o1eebc99-9999-0000-0000-000000000001', 'PENDING', 'PAID', NOW() - INTERVAL '10 days'),
    ('o1eebc99-9999-0000-0000-000000000001', 'PAID', 'SHIPPED', NOW() - INTERVAL '8 days'),
    ('o1eebc99-9999-0000-0000-000000000001', 'SHIPPED', 'DELIVERED', NOW() - INTERVAL '5 days');
