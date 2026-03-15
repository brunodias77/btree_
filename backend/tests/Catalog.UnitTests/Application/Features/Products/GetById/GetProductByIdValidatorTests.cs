using Catalog.Application.Features.Products.GetById;
using FluentAssertions;

namespace Catalog.UnitTests.Application.Features.Products.GetById;

public class GetProductByIdValidatorTests
{
    private readonly GetProductByIdValidator _validator;

    public GetProductByIdValidatorTests()
    {
        _validator = new GetProductByIdValidator();
    }

    [Fact]
    public void Validator_ShouldHaveError_WhenIdIsEmpty()
    {
        // Arrange
        var input = new GetProductByIdInput(Guid.Empty);

        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "O ID do produto é obrigatório e deve ser válido.");
    }

    [Fact]
    public void Validator_ShouldNotHaveError_WhenIdIsValid()
    {
        // Arrange
        var input = new GetProductByIdInput(Guid.NewGuid());

        // Act
        var result = _validator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
