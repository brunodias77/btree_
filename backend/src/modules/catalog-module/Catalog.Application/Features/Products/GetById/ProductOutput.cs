using System;
using System.Collections.Generic;

namespace Catalog.Application.Features.Products.GetById;

/// <summary>
/// DTO que representa a imagem do produto.
/// </summary>
public record ProductImageOutput(
    Guid Id,
    string Url,
    bool IsMain
);

/// <summary>
/// DTO de saída com os detalhes agregados do produto.
/// </summary>
public record ProductOutput(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int QuantityInStock,
    string Status,
    string BrandName,
    string CategoryName,
    List<ProductImageOutput> Images
);
