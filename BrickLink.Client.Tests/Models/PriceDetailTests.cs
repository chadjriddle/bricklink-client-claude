using System.Text.Json;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class PriceDetailTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public PriceDetailTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void PriceDetail_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var priceDetail = new PriceDetail();

        // Assert
        Assert.Equal(0, priceDetail.Quantity);
        Assert.Equal(0m, priceDetail.UnitPrice);
        Assert.Null(priceDetail.ShippingAvailable);
        Assert.Null(priceDetail.SellerCountryCode);
        Assert.Null(priceDetail.BuyerCountryCode);
        Assert.Null(priceDetail.DateOrdered);
    }

    [Fact]
    public void PriceDetail_PropertiesCanBeSet()
    {
        // Arrange
        var priceDetail = new PriceDetail();
        var dateOrdered = DateTimeOffset.UtcNow;

        // Act
        priceDetail.Quantity = 5;
        priceDetail.UnitPrice = 12.3456m;
        priceDetail.ShippingAvailable = true;
        priceDetail.SellerCountryCode = "US";
        priceDetail.BuyerCountryCode = "CA";
        priceDetail.DateOrdered = dateOrdered;

        // Assert
        Assert.Equal(5, priceDetail.Quantity);
        Assert.Equal(12.3456m, priceDetail.UnitPrice);
        Assert.True(priceDetail.ShippingAvailable);
        Assert.Equal("US", priceDetail.SellerCountryCode);
        Assert.Equal("CA", priceDetail.BuyerCountryCode);
        Assert.Equal(dateOrdered, priceDetail.DateOrdered);
    }

    [Fact]
    public void PriceDetail_SerializesToJson()
    {
        // Arrange
        var dateOrdered = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var priceDetail = new PriceDetail
        {
            Quantity = 2,
            UnitPrice = 5.7800m,
            ShippingAvailable = false,
            SellerCountryCode = "DE",
            BuyerCountryCode = "FR",
            DateOrdered = dateOrdered
        };

        // Act
        var json = JsonSerializer.Serialize(priceDetail, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("quantity", out var quantityProperty));
        Assert.Equal(2, quantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("unit_price", out var unitPriceProperty));
        Assert.Equal(5.7800m, unitPriceProperty.GetDecimal());

        Assert.True(root.TryGetProperty("shipping_available", out var shippingProperty));
        Assert.False(shippingProperty.GetBoolean());

        Assert.True(root.TryGetProperty("seller_country_code", out var sellerProperty));
        Assert.Equal("DE", sellerProperty.GetString());

        Assert.True(root.TryGetProperty("buyer_country_code", out var buyerProperty));
        Assert.Equal("FR", buyerProperty.GetString());

        Assert.True(root.TryGetProperty("date_ordered", out var dateProperty));
        var deserializedDate = DateTimeOffset.Parse(dateProperty.GetString()!);
        Assert.Equal(dateOrdered, deserializedDate);
    }

    [Fact]
    public void PriceDetail_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "quantity": 1,
            "unit_price": 0.1234,
            "shipping_available": true,
            "seller_country_code": "US",
            "buyer_country_code": "CA",
            "date_ordered": "2024-01-15T10:30:00.000Z"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Quantity);
        Assert.Equal(0.1234m, result.UnitPrice);
        Assert.True(result.ShippingAvailable);
        Assert.Equal("US", result.SellerCountryCode);
        Assert.Equal("CA", result.BuyerCountryCode);
        Assert.NotNull(result.DateOrdered);

        var expectedDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);
        Assert.Equal(expectedDate, result.DateOrdered);
    }

    [Fact]
    public void PriceDetail_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalDetail = new PriceDetail
        {
            Quantity = 10,
            UnitPrice = 99.9999m,
            ShippingAvailable = null,
            SellerCountryCode = "JP",
            BuyerCountryCode = "AU",
            DateOrdered = new DateTimeOffset(2023, 12, 25, 15, 45, 30, TimeSpan.FromHours(9))
        };

        // Act
        var json = JsonSerializer.Serialize(originalDetail, _jsonOptions);
        var deserializedDetail = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedDetail);
        Assert.Equal(originalDetail.Quantity, deserializedDetail.Quantity);
        Assert.Equal(originalDetail.UnitPrice, deserializedDetail.UnitPrice);
        Assert.Equal(originalDetail.ShippingAvailable, deserializedDetail.ShippingAvailable);
        Assert.Equal(originalDetail.SellerCountryCode, deserializedDetail.SellerCountryCode);
        Assert.Equal(originalDetail.BuyerCountryCode, deserializedDetail.BuyerCountryCode);
        Assert.Equal(originalDetail.DateOrdered, deserializedDetail.DateOrdered);
    }

    [Fact]
    public void PriceDetail_HandlesNullOptionalFields()
    {
        // Arrange
        var json = """
        {
            "quantity": 3,
            "unit_price": 15.50,
            "shipping_available": null,
            "seller_country_code": null,
            "buyer_country_code": null,
            "date_ordered": null
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Quantity);
        Assert.Equal(15.50m, result.UnitPrice);
        Assert.Null(result.ShippingAvailable);
        Assert.Null(result.SellerCountryCode);
        Assert.Null(result.BuyerCountryCode);
        Assert.Null(result.DateOrdered);
    }

    [Fact]
    public void PriceDetail_HandlesDecimalPrecision()
    {
        // Arrange
        var priceDetail = new PriceDetail
        {
            Quantity = 1,
            UnitPrice = 123.4567m
        };

        // Act
        var json = JsonSerializer.Serialize(priceDetail, _jsonOptions);
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123.4567m, result.UnitPrice);
    }

    [Fact]
    public void PriceDetail_HandlesZeroQuantityAndPrice()
    {
        // Arrange
        var json = """
        {
            "quantity": 0,
            "unit_price": 0.0000
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Quantity);
        Assert.Equal(0.0000m, result.UnitPrice);
    }

    [Fact]
    public void PriceDetail_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "quantity": 1,
            "unit_price": 1.00
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Quantity);
        Assert.Equal(1.00m, result.UnitPrice);
        Assert.Null(result.ShippingAvailable);
        Assert.Null(result.SellerCountryCode);
        Assert.Null(result.BuyerCountryCode);
        Assert.Null(result.DateOrdered);
    }

    [Fact]
    public void PriceDetail_HandlesLargeQuantityAndPrice()
    {
        // Arrange
        var priceDetail = new PriceDetail
        {
            Quantity = int.MaxValue,
            UnitPrice = 9999.9999m
        };

        // Act
        var json = JsonSerializer.Serialize(priceDetail, _jsonOptions);
        var result = JsonSerializer.Deserialize<PriceDetail>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(int.MaxValue, result.Quantity);
        Assert.Equal(9999.9999m, result.UnitPrice);
    }
}
