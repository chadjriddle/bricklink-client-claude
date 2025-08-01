using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class PriceGuideTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public PriceGuideTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void PriceGuide_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var priceGuide = new PriceGuide();

        // Assert
        Assert.NotNull(priceGuide.Item);
        Assert.Equal(NewOrUsed.New, priceGuide.NewOrUsed);
        Assert.Equal(string.Empty, priceGuide.CurrencyCode);
        Assert.Equal(0m, priceGuide.MinPrice);
        Assert.Equal(0m, priceGuide.MaxPrice);
        Assert.Equal(0m, priceGuide.AvgPrice);
        Assert.Equal(0m, priceGuide.QtyAvgPrice);
        Assert.Equal(0, priceGuide.UnitQuantity);
        Assert.Equal(0, priceGuide.TotalQuantity);
        Assert.NotNull(priceGuide.PriceDetail);
        Assert.Empty(priceGuide.PriceDetail);
    }

    [Fact]
    public void PriceGuide_PropertiesCanBeSet()
    {
        // Arrange
        var priceGuide = new PriceGuide();
        var inventoryItem = new InventoryItem
        {
            No = "3001",
            Type = ItemType.Part
        };
        var priceDetails = new List<PriceDetail>
        {
            new PriceDetail
            {
                Quantity = 1,
                UnitPrice = 0.50m
            }
        };

        // Act
        priceGuide.Item = inventoryItem;
        priceGuide.NewOrUsed = NewOrUsed.Used;
        priceGuide.CurrencyCode = "USD";
        priceGuide.MinPrice = 0.25m;
        priceGuide.MaxPrice = 2.00m;
        priceGuide.AvgPrice = 0.75m;
        priceGuide.QtyAvgPrice = 0.68m;
        priceGuide.UnitQuantity = 150;
        priceGuide.TotalQuantity = 450;
        priceGuide.PriceDetail = priceDetails;

        // Assert
        Assert.Equal(inventoryItem, priceGuide.Item);
        Assert.Equal(NewOrUsed.Used, priceGuide.NewOrUsed);
        Assert.Equal("USD", priceGuide.CurrencyCode);
        Assert.Equal(0.25m, priceGuide.MinPrice);
        Assert.Equal(2.00m, priceGuide.MaxPrice);
        Assert.Equal(0.75m, priceGuide.AvgPrice);
        Assert.Equal(0.68m, priceGuide.QtyAvgPrice);
        Assert.Equal(150, priceGuide.UnitQuantity);
        Assert.Equal(450, priceGuide.TotalQuantity);
        Assert.Equal(priceDetails, priceGuide.PriceDetail);
    }

    [Fact]
    public void PriceGuide_SerializesToJson()
    {
        // Arrange
        var priceGuide = new PriceGuide
        {
            Item = new InventoryItem
            {
                No = "3001",
                Type = ItemType.Part
            },
            NewOrUsed = NewOrUsed.New,
            CurrencyCode = "EUR",
            MinPrice = 0.10m,
            MaxPrice = 5.00m,
            AvgPrice = 1.25m,
            QtyAvgPrice = 1.15m,
            UnitQuantity = 75,
            TotalQuantity = 300,
            PriceDetail = new List<PriceDetail>
            {
                new PriceDetail
                {
                    Quantity = 1,
                    UnitPrice = 1.50m,
                    ShippingAvailable = true
                },
                new PriceDetail
                {
                    Quantity = 10,
                    UnitPrice = 1.20m,
                    ShippingAvailable = false
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(priceGuide, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("item", out var itemProperty));
        Assert.True(itemProperty.TryGetProperty("no", out var noProperty));
        Assert.Equal("3001", noProperty.GetString());

        Assert.True(root.TryGetProperty("new_or_used", out var newOrUsedProperty));
        Assert.Equal("N", newOrUsedProperty.GetString());

        Assert.True(root.TryGetProperty("currency_code", out var currencyProperty));
        Assert.Equal("EUR", currencyProperty.GetString());

        Assert.True(root.TryGetProperty("min_price", out var minPriceProperty));
        Assert.Equal(0.10m, minPriceProperty.GetDecimal());

        Assert.True(root.TryGetProperty("max_price", out var maxPriceProperty));
        Assert.Equal(5.00m, maxPriceProperty.GetDecimal());

        Assert.True(root.TryGetProperty("avg_price", out var avgPriceProperty));
        Assert.Equal(1.25m, avgPriceProperty.GetDecimal());

        Assert.True(root.TryGetProperty("qty_avg_price", out var qtyAvgPriceProperty));
        Assert.Equal(1.15m, qtyAvgPriceProperty.GetDecimal());

        Assert.True(root.TryGetProperty("unit_quantity", out var unitQuantityProperty));
        Assert.Equal(75, unitQuantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("total_quantity", out var totalQuantityProperty));
        Assert.Equal(300, totalQuantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("price_detail", out var priceDetailProperty));
        Assert.Equal(JsonValueKind.Array, priceDetailProperty.ValueKind);
        Assert.Equal(2, priceDetailProperty.GetArrayLength());
    }

    [Fact]
    public void PriceGuide_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3001",
                "type": "PART"
            },
            "new_or_used": "U",
            "currency_code": "GBP",
            "min_price": 0.05,
            "max_price": 10.00,
            "avg_price": 2.50,
            "qty_avg_price": 2.25,
            "unit_quantity": 25,
            "total_quantity": 100,
            "price_detail": [
                {
                    "quantity": 1,
                    "unit_price": 2.50,
                    "shipping_available": true
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceGuide>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal("3001", result.Item.No);
        Assert.Equal(ItemType.Part, result.Item.Type);
        Assert.Equal(NewOrUsed.Used, result.NewOrUsed);
        Assert.Equal("GBP", result.CurrencyCode);
        Assert.Equal(0.05m, result.MinPrice);
        Assert.Equal(10.00m, result.MaxPrice);
        Assert.Equal(2.50m, result.AvgPrice);
        Assert.Equal(2.25m, result.QtyAvgPrice);
        Assert.Equal(25, result.UnitQuantity);
        Assert.Equal(100, result.TotalQuantity);
        Assert.NotNull(result.PriceDetail);
        Assert.Single(result.PriceDetail);
        Assert.Equal(1, result.PriceDetail[0].Quantity);
        Assert.Equal(2.50m, result.PriceDetail[0].UnitPrice);
        Assert.True(result.PriceDetail[0].ShippingAvailable);
    }

    [Fact]
    public void PriceGuide_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalGuide = new PriceGuide
        {
            Item = new InventoryItem
            {
                No = "2456",
                Type = ItemType.Part
            },
            NewOrUsed = NewOrUsed.New,
            CurrencyCode = "CAD",
            MinPrice = 1.2345m,
            MaxPrice = 9.8765m,
            AvgPrice = 5.5555m,
            QtyAvgPrice = 5.4321m,
            UnitQuantity = 42,
            TotalQuantity = 168,
            PriceDetail = new List<PriceDetail>
            {
                new PriceDetail
                {
                    Quantity = 4,
                    UnitPrice = 5.25m,
                    SellerCountryCode = "CA",
                    BuyerCountryCode = "US"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(originalGuide, _jsonOptions);
        var deserializedGuide = JsonSerializer.Deserialize<PriceGuide>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedGuide);
        Assert.Equal(originalGuide.Item.No, deserializedGuide.Item.No);
        Assert.Equal(originalGuide.Item.Type, deserializedGuide.Item.Type);
        Assert.Equal(originalGuide.NewOrUsed, deserializedGuide.NewOrUsed);
        Assert.Equal(originalGuide.CurrencyCode, deserializedGuide.CurrencyCode);
        Assert.Equal(originalGuide.MinPrice, deserializedGuide.MinPrice);
        Assert.Equal(originalGuide.MaxPrice, deserializedGuide.MaxPrice);
        Assert.Equal(originalGuide.AvgPrice, deserializedGuide.AvgPrice);
        Assert.Equal(originalGuide.QtyAvgPrice, deserializedGuide.QtyAvgPrice);
        Assert.Equal(originalGuide.UnitQuantity, deserializedGuide.UnitQuantity);
        Assert.Equal(originalGuide.TotalQuantity, deserializedGuide.TotalQuantity);
        Assert.Equal(originalGuide.PriceDetail.Count, deserializedGuide.PriceDetail.Count);
    }

    [Fact]
    public void PriceGuide_HandlesEmptyPriceDetailArray()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3005",
                "type": "PART"
            },
            "new_or_used": "N",
            "currency_code": "USD",
            "min_price": 0,
            "max_price": 0,
            "avg_price": 0,
            "qty_avg_price": 0,
            "unit_quantity": 0,
            "total_quantity": 0,
            "price_detail": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceGuide>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("3005", result.Item.No);
        Assert.Equal(NewOrUsed.New, result.NewOrUsed);
        Assert.NotNull(result.PriceDetail);
        Assert.Empty(result.PriceDetail);
    }

    [Fact]
    public void PriceGuide_HandlesLargePricesAndQuantities()
    {
        // Arrange
        var priceGuide = new PriceGuide
        {
            MinPrice = 9999.9999m,
            MaxPrice = 99999.9999m,
            AvgPrice = 12345.6789m,
            QtyAvgPrice = 11111.1111m,
            UnitQuantity = int.MaxValue,
            TotalQuantity = int.MaxValue
        };

        // Act
        var json = JsonSerializer.Serialize(priceGuide, _jsonOptions);
        var result = JsonSerializer.Deserialize<PriceGuide>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(9999.9999m, result.MinPrice);
        Assert.Equal(99999.9999m, result.MaxPrice);
        Assert.Equal(12345.6789m, result.AvgPrice);
        Assert.Equal(11111.1111m, result.QtyAvgPrice);
        Assert.Equal(int.MaxValue, result.UnitQuantity);
        Assert.Equal(int.MaxValue, result.TotalQuantity);
    }

    [Fact]
    public void PriceGuide_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "item": {},
            "new_or_used": "N",
            "currency_code": "",
            "min_price": 0,
            "max_price": 0,
            "avg_price": 0,
            "qty_avg_price": 0,
            "unit_quantity": 0,
            "total_quantity": 0,
            "price_detail": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<PriceGuide>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(NewOrUsed.New, result.NewOrUsed);
        Assert.Equal(string.Empty, result.CurrencyCode);
        Assert.NotNull(result.PriceDetail);
        Assert.Empty(result.PriceDetail);
    }
}
