using DGII.Api.Validators;
using Xunit;

namespace DGII.Api.Tests.Unit;

public class RncCedulaValidatorTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Normalize_TrimsWhitespaceAndPreservesLeadingZeros()
    {
        var result = RncCedulaValidator.Normalize(" 00101564321 ");

        Assert.Equal("00101564321", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123456789012")]
    [InlineData("123-456")]
    [InlineData("ABC123")]
    [Trait("Category", "Unit")]
    public void Normalize_RejectsInvalidIdentifiers(string? value)
    {
        Assert.Throws<ArgumentException>(() => RncCedulaValidator.Normalize(value));
    }
}
