using BrickLink.Client;

namespace BrickLink.Client.Tests;

/// <summary>
/// Temporary unit tests for Class1 to validate CI/CD pipeline and coverage reporting.
/// These will be removed when actual implementation begins.
/// </summary>
public class Class1Tests
{
    private readonly Class1 _class1 = new Class1();

    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 8;

        // Act
        int result = _class1.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_NegativeNumbers_ReturnsSum()
    {
        // Arrange
        int a = -5;
        int b = -3;
        int expected = -8;

        // Act
        int result = _class1.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Add_ZeroAndNumber_ReturnsNumber()
    {
        // Arrange
        int a = 0;
        int b = 42;
        int expected = 42;

        // Act
        int result = _class1.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(-100, false)]
    public void IsPositive_VariousNumbers_ReturnsExpectedResult(int number, bool expected)
    {
        // Act
        bool result = _class1.IsPositive(number);

        // Assert
        Assert.Equal(expected, result);
    }
}
