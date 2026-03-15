using Catalog.Application.Features.Products.GetById;
using Catalog.Domain.Aggregates.Product;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using FluentValidation;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shared.Application.Abstractions;
using Shared.Domain.ValueObjects;

namespace Catalog.UnitTests.Application.Features.Products.GetById;

public class GetProductByIdUseCaseTests
{
    private readonly IProductReadRepository _readRepositoryMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly IValidator<GetProductByIdInput> _validatorMock;
    private readonly GetProductByIdUseCase _useCase;

    public GetProductByIdUseCaseTests()
    {
        _readRepositoryMock = Substitute.For<IProductReadRepository>();
        _cacheServiceMock = Substitute.For<ICacheService>();
        _validatorMock = Substitute.For<IValidator<GetProductByIdInput>>();

        _useCase = new GetProductByIdUseCase(
            _readRepositoryMock,
            _cacheServiceMock,
            _validatorMock);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var input = new GetProductByIdInput(Guid.Empty);
        var validationFailure = new FluentValidation.Results.ValidationResult(
            new[] { new FluentValidation.Results.ValidationFailure("Id", "Error") });
        
        _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>())
            .Returns(validationFailure);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Error");
        
        await _cacheServiceMock.DidNotReceiveWithAnyArgs().GetAsync<ProductOutput>(default!);
        await _readRepositoryMock.DidNotReceiveWithAnyArgs().GetByIdAsync(default);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnCachedProduct_WhenCacheIsAvailable()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var input = new GetProductByIdInput(productId);
        
        _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        var cachedProduct = new ProductOutput(
            Id: productId,
            Name: "Cached Product",
            Description: "Cached Desc",
            Sku: "CACHED-01",
            Price: 10m,
            QuantityInStock: 5,
            Status: "Active",
            BrandName: Guid.NewGuid().ToString(),
            CategoryName: Guid.NewGuid().ToString(),
            Images: new List<ProductImageOutput>()
        );

        _cacheServiceMock.GetAsync<ProductOutput>($"catalog:products:{productId}", Arg.Any<CancellationToken>())
            .Returns(cachedProduct);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(cachedProduct);

        await _readRepositoryMock.DidNotReceiveWithAnyArgs().GetByIdAsync(default);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotFound_WhenProductDoesNotExistInDb()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var input = new GetProductByIdInput(productId);
        
        _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _cacheServiceMock.GetAsync<ProductOutput>(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        _readRepositoryMock.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound(productId));
        
        await _cacheServiceMock.DidNotReceiveWithAnyArgs().SetAsync<ProductOutput>(default!, default!, default, default);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnProductFromDb_AndSaveToCache_WhenProductExists()
    {
         // Arrange
        var productId = Guid.NewGuid();
        var input = new GetProductByIdInput(productId);
        
        _validatorMock.ValidateAsync(input, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _cacheServiceMock.GetAsync<ProductOutput>(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var brandId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var productMock = Product.Create(
            "Test Product",
            "Description",
            brandId,
            categoryId,
            Money.FromDecimal(100.50m, "BRL"),
            Sku.Create("SKU-123").Value,
            null, // barcode
            10, // stock
            null, null, null, null, null, null
        ).Value;

        // Note: Using Product aggregate creation logic directly. In real tests, 
        // reflection might be needed to set the internal ID for asserting the ID match.

        _readRepositoryMock.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(productMock);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Product");
        result.Value.Price.Should().Be(100.50m);
        result.Value.QuantityInStock.Should().Be(10);
        result.Value.BrandName.Should().Be(brandId.ToString());

        await _cacheServiceMock.Received(1).SetAsync(
            $"catalog:products:{productId}",
            result.Value,
            TimeSpan.FromMinutes(15),
            Arg.Any<CancellationToken>());
    }
}
