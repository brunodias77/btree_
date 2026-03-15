using System;

namespace Catalog.Application.Features.Products.GetById;

/// <summary>
/// Entrada para o caso de uso de consulta de produto por ID.
/// </summary>
public record GetProductByIdInput(Guid Id);
