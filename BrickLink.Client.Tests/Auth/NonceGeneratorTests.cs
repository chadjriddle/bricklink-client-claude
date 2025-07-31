using System.Text.RegularExpressions;
using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="NonceGenerator"/> class.
/// </summary>
public class NonceGeneratorTests
{
    [Fact]
    public void Generate_ReturnsNonEmptyString()
    {
        // Act
        var nonce = NonceGenerator.Generate();

        // Assert
        Assert.False(string.IsNullOrEmpty(nonce));
    }

    [Fact]
    public void Generate_ReturnsUrlSafeBase64String()
    {
        // Act
        var nonce = NonceGenerator.Generate();

        // Assert
        // URL-safe base64 should only contain letters, numbers, hyphens, and underscores
        var urlSafeBase64Pattern = @"^[A-Za-z0-9\-_]+$";
        Assert.Matches(urlSafeBase64Pattern, nonce);
    }

    [Fact]
    public void Generate_DoesNotContainPaddingCharacters()
    {
        // Act
        var nonce = NonceGenerator.Generate();

        // Assert
        Assert.DoesNotContain('=', nonce);
    }

    [Fact]
    public void Generate_ReturnsUniqueValues()
    {
        // Arrange
        var nonces = new HashSet<string>();
        const int iterations = 1000;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var nonce = NonceGenerator.Generate();
            nonces.Add(nonce);
        }

        // Assert
        // With cryptographically secure random generation, all nonces should be unique
        Assert.Equal(iterations, nonces.Count);
    }

    [Fact]
    public void Generate_WithValidLength_ReturnsCorrectlyEncodedString()
    {
        // Arrange
        const int length = 16;

        // Act
        var nonce = NonceGenerator.Generate(length);

        // Assert
        Assert.False(string.IsNullOrEmpty(nonce));

        // URL-safe base64 should only contain letters, numbers, hyphens, and underscores
        var urlSafeBase64Pattern = @"^[A-Za-z0-9\-_]+$";
        Assert.Matches(urlSafeBase64Pattern, nonce);

        // Should not contain padding
        Assert.DoesNotContain('=', nonce);
    }

    [Fact]
    public void Generate_WithMinimumLength_Succeeds()
    {
        // Arrange
        const int minLength = 8;

        // Act
        var nonce = NonceGenerator.Generate(minLength);

        // Assert
        Assert.False(string.IsNullOrEmpty(nonce));
    }

    [Fact]
    public void Generate_WithMaximumLength_Succeeds()
    {
        // Arrange
        const int maxLength = 64;

        // Act
        var nonce = NonceGenerator.Generate(maxLength);

        // Assert
        Assert.False(string.IsNullOrEmpty(nonce));
    }

    [Theory]
    [InlineData(7)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Generate_WithLengthTooSmall_ThrowsArgumentOutOfRangeException(int length)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NonceGenerator.Generate(length));

        Assert.Equal("length", exception.ParamName);
        Assert.Contains("must be between 8 and 64 bytes", exception.Message);
    }

    [Theory]
    [InlineData(65)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Generate_WithLengthTooLarge_ThrowsArgumentOutOfRangeException(int length)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            NonceGenerator.Generate(length));

        Assert.Equal("length", exception.ParamName);
        Assert.Contains("must be between 8 and 64 bytes", exception.Message);
    }

    [Fact]
    public void Generate_WithVariousValidLengths_ReturnsUniqueValues()
    {
        // Arrange
        var nonces = new HashSet<string>();
        var validLengths = new[] { 8, 12, 16, 20, 24, 32, 40, 48, 56, 64 };

        // Act
        foreach (var length in validLengths)
        {
            for (int i = 0; i < 10; i++) // Generate 10 nonces for each length
            {
                var nonce = NonceGenerator.Generate(length);
                nonces.Add(nonce);
            }
        }

        // Assert
        // All nonces should be unique regardless of the length used
        Assert.Equal(validLengths.Length * 10, nonces.Count);
    }

    [Fact]
    public void Generate_DefaultAndParameterizedVersions_BothReturnValidNonces()
    {
        // Act
        var defaultNonce = NonceGenerator.Generate();
        var parameterizedNonce = NonceGenerator.Generate(16);

        // Assert
        Assert.False(string.IsNullOrEmpty(defaultNonce));
        Assert.False(string.IsNullOrEmpty(parameterizedNonce));

        // Both should be URL-safe base64
        var urlSafeBase64Pattern = @"^[A-Za-z0-9\-_]+$";
        Assert.Matches(urlSafeBase64Pattern, defaultNonce);
        Assert.Matches(urlSafeBase64Pattern, parameterizedNonce);

        // Neither should contain padding
        Assert.DoesNotContain('=', defaultNonce);
        Assert.DoesNotContain('=', parameterizedNonce);
    }

    [Fact]
    public void Generate_ConsecutiveCalls_ReturnDifferentValues()
    {
        // Act
        var nonce1 = NonceGenerator.Generate();
        var nonce2 = NonceGenerator.Generate();
        var nonce3 = NonceGenerator.Generate();

        // Assert
        Assert.NotEqual(nonce1, nonce2);
        Assert.NotEqual(nonce2, nonce3);
        Assert.NotEqual(nonce1, nonce3);
    }

    [Fact]
    public void Generate_WithSameLength_ReturnsDifferentValues()
    {
        // Arrange
        const int length = 16;

        // Act
        var nonce1 = NonceGenerator.Generate(length);
        var nonce2 = NonceGenerator.Generate(length);
        var nonce3 = NonceGenerator.Generate(length);

        // Assert
        Assert.NotEqual(nonce1, nonce2);
        Assert.NotEqual(nonce2, nonce3);
        Assert.NotEqual(nonce1, nonce3);
    }
}
