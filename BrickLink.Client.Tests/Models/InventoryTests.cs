using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;

namespace BrickLink.Client.Tests.Models;

public class InventoryTests
{
    [Fact]
    public void Inventory_Constructor_InitializesWithDefaultValues()
    {
        // Act
        var inventory = new Inventory();

        // Assert
        Assert.Equal(0, inventory.InventoryId);
        Assert.NotNull(inventory.Item);
        Assert.Equal(0, inventory.ColorId);
        Assert.Equal(0, inventory.Quantity);
        Assert.Equal(NewOrUsed.New, inventory.NewOrUsed);
        Assert.Equal(0m, inventory.UnitPrice);
        Assert.Equal(0, inventory.BindId);
        Assert.Null(inventory.Description);
        Assert.Null(inventory.Remarks);
        Assert.Equal(0, inventory.Bulk);
        Assert.False(inventory.IsRetain);
        Assert.False(inventory.IsStockRoom);
        Assert.Null(inventory.StockRoomId);
        Assert.Equal(DateTimeOffset.MinValue, inventory.DateCreated);
        Assert.Null(inventory.TierQuantity1);
        Assert.Null(inventory.TierPrice1);
        Assert.Null(inventory.TierQuantity2);
        Assert.Null(inventory.TierPrice2);
        Assert.Null(inventory.TierQuantity3);
        Assert.Null(inventory.TierPrice3);
        Assert.Null(inventory.MyCost);
        Assert.Null(inventory.SaleRate);
        Assert.Null(inventory.MyWeight);
    }

    [Fact]
    public void Inventory_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var item = new InventoryItem
        {
            No = "3001",
            Name = "Brick 2 x 4",
            Type = ItemType.Part,
            CategoryId = 5
        };
        var dateCreated = DateTimeOffset.UtcNow;

        // Act
        var inventory = new Inventory
        {
            InventoryId = 123456789L,
            Item = item,
            ColorId = 5,
            Quantity = 10,
            NewOrUsed = NewOrUsed.Used,
            UnitPrice = 1.25m,
            BindId = 1,
            Description = "Great condition brick",
            Remarks = "Private seller notes",
            Bulk = 5,
            IsRetain = true,
            IsStockRoom = true,
            StockRoomId = "A",
            DateCreated = dateCreated,
            TierQuantity1 = 10,
            TierPrice1 = 1.00m,
            TierQuantity2 = 20,
            TierPrice2 = 0.90m,
            TierQuantity3 = 50,
            TierPrice3 = 0.80m,
            MyCost = 0.50m,
            SaleRate = 15,
            MyWeight = 2.45m
        };

        // Assert
        Assert.Equal(123456789L, inventory.InventoryId);
        Assert.Same(item, inventory.Item);
        Assert.Equal(5, inventory.ColorId);
        Assert.Equal(10, inventory.Quantity);
        Assert.Equal(NewOrUsed.Used, inventory.NewOrUsed);
        Assert.Equal(1.25m, inventory.UnitPrice);
        Assert.Equal(1, inventory.BindId);
        Assert.Equal("Great condition brick", inventory.Description);
        Assert.Equal("Private seller notes", inventory.Remarks);
        Assert.Equal(5, inventory.Bulk);
        Assert.True(inventory.IsRetain);
        Assert.True(inventory.IsStockRoom);
        Assert.Equal("A", inventory.StockRoomId);
        Assert.Equal(dateCreated, inventory.DateCreated);
        Assert.Equal(10, inventory.TierQuantity1);
        Assert.Equal(1.00m, inventory.TierPrice1);
        Assert.Equal(20, inventory.TierQuantity2);
        Assert.Equal(0.90m, inventory.TierPrice2);
        Assert.Equal(50, inventory.TierQuantity3);
        Assert.Equal(0.80m, inventory.TierPrice3);
        Assert.Equal(0.50m, inventory.MyCost);
        Assert.Equal(15, inventory.SaleRate);
        Assert.Equal(2.45m, inventory.MyWeight);
    }

    [Fact]
    public void Inventory_JsonSerialization_SerializesAllProperties()
    {
        // Arrange
        var dateCreated = DateTimeOffset.Parse("2023-12-01T10:30:00.000Z");
        var inventory = new Inventory
        {
            InventoryId = 987654321L,
            Item = new InventoryItem
            {
                No = "3003",
                Name = "Brick 2 x 2",
                Type = ItemType.Part,
                CategoryId = 5
            },
            ColorId = 4,
            Quantity = 25,
            NewOrUsed = NewOrUsed.New,
            UnitPrice = 0.75m,
            BindId = 0,
            Description = "Perfect condition",
            Remarks = "Batch purchase",
            Bulk = 1,
            IsRetain = false,
            IsStockRoom = false,
            StockRoomId = null,
            DateCreated = dateCreated,
            TierQuantity1 = 50,
            TierPrice1 = 0.70m,
            TierQuantity2 = 100,
            TierPrice2 = 0.65m,
            TierQuantity3 = null,
            TierPrice3 = null,
            MyCost = 0.40m,
            SaleRate = null,
            MyWeight = 1.89m
        };

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var json = JsonSerializer.Serialize(inventory, options);

        // Assert
        Assert.Contains("\"inventory_id\":987654321", json);
        Assert.Contains("\"item\":{", json);
        Assert.Contains("\"color_id\":4", json);
        Assert.Contains("\"quantity\":25", json);
        Assert.Contains("\"new_or_used\":\"N\"", json);
        Assert.Contains("\"unit_price\":0.75", json);
        Assert.Contains("\"bind_id\":0", json);
        Assert.Contains("\"description\":\"Perfect condition\"", json);
        Assert.Contains("\"remarks\":\"Batch purchase\"", json);
        Assert.Contains("\"bulk\":1", json);
        Assert.Contains("\"is_retain\":false", json);
        Assert.Contains("\"is_stock_room\":false", json);
        Assert.Contains("\"date_created\":\"2023-12-01T10:30:00.000Z\"", json);
        Assert.Contains("\"tier_quantity1\":50", json);
        Assert.Contains("\"tier_price1\":0.70", json);
        Assert.Contains("\"tier_quantity2\":100", json);
        Assert.Contains("\"tier_price2\":0.65", json);
        Assert.Contains("\"my_cost\":0.40", json);
        Assert.Contains("\"my_weight\":1.89", json);
    }

    [Fact]
    public void Inventory_JsonDeserialization_DeserializesAllProperties()
    {
        // Arrange
        var json = """
        {
            "inventory_id": 555777999,
            "item": {
                "no": "3004",
                "name": "Brick 1 x 2",
                "type": "PART",
                "category_id": 5
            },
            "color_id": 1,
            "quantity": 100,
            "new_or_used": "U",
            "unit_price": 0.15,
            "bind_id": 2,
            "description": "Good used condition",
            "remarks": "Estate sale lot",
            "bulk": 10,
            "is_retain": true,
            "is_stock_room": true,
            "stock_room_id": "B",
            "date_created": "2023-11-15T14:22:30.500Z",
            "tier_quantity1": 25,
            "tier_price1": 0.12,
            "tier_quantity2": 50,
            "tier_price2": 0.10,
            "tier_quantity3": 100,
            "tier_price3": 0.08,
            "my_cost": 0.05,
            "sale_rate": 20,
            "my_weight": 0.45
        }
        """;

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var inventory = JsonSerializer.Deserialize<Inventory>(json, options);

        // Assert
        Assert.NotNull(inventory);
        Assert.Equal(555777999L, inventory.InventoryId);
        Assert.NotNull(inventory.Item);
        Assert.Equal("3004", inventory.Item.No);
        Assert.Equal("Brick 1 x 2", inventory.Item.Name);
        Assert.Equal(ItemType.Part, inventory.Item.Type);
        Assert.Equal(5, inventory.Item.CategoryId);
        Assert.Equal(1, inventory.ColorId);
        Assert.Equal(100, inventory.Quantity);
        Assert.Equal(NewOrUsed.Used, inventory.NewOrUsed);
        Assert.Equal(0.15m, inventory.UnitPrice);
        Assert.Equal(2, inventory.BindId);
        Assert.Equal("Good used condition", inventory.Description);
        Assert.Equal("Estate sale lot", inventory.Remarks);
        Assert.Equal(10, inventory.Bulk);
        Assert.True(inventory.IsRetain);
        Assert.True(inventory.IsStockRoom);
        Assert.Equal("B", inventory.StockRoomId);

        var expectedDate = DateTimeOffset.Parse("2023-11-15T14:22:30.500Z");
        Assert.Equal(expectedDate, inventory.DateCreated);

        Assert.Equal(25, inventory.TierQuantity1);
        Assert.Equal(0.12m, inventory.TierPrice1);
        Assert.Equal(50, inventory.TierQuantity2);
        Assert.Equal(0.10m, inventory.TierPrice2);
        Assert.Equal(100, inventory.TierQuantity3);
        Assert.Equal(0.08m, inventory.TierPrice3);
        Assert.Equal(0.05m, inventory.MyCost);
        Assert.Equal(20, inventory.SaleRate);
        Assert.Equal(0.45m, inventory.MyWeight);
    }

    [Fact]
    public void Inventory_JsonSerialization_RoundTrip_PreservesData()
    {
        // Arrange
        var originalInventory = new Inventory
        {
            InventoryId = 111222333L,
            Item = new InventoryItem
            {
                No = "3005",
                Name = "Brick 1 x 1",
                Type = ItemType.Part,
                CategoryId = 5
            },
            ColorId = 2,
            Quantity = 500,
            NewOrUsed = NewOrUsed.New,
            UnitPrice = 0.05m,
            BindId = 1,
            Description = "Bulk lot",
            Remarks = "Sorted by color",
            Bulk = 50,
            IsRetain = false,
            IsStockRoom = false,
            StockRoomId = null,
            DateCreated = DateTimeOffset.Parse("2023-10-01T08:15:45.123Z"),
            TierQuantity1 = 100,
            TierPrice1 = 0.04m,
            TierQuantity2 = 250,
            TierPrice2 = 0.035m,
            TierQuantity3 = 500,
            TierPrice3 = 0.03m,
            MyCost = 0.02m,
            SaleRate = 10,
            MyWeight = 0.12m
        };

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var json = JsonSerializer.Serialize(originalInventory, options);
        var deserializedInventory = JsonSerializer.Deserialize<Inventory>(json, options);

        // Assert
        Assert.NotNull(deserializedInventory);
        Assert.Equal(originalInventory.InventoryId, deserializedInventory.InventoryId);
        Assert.Equal(originalInventory.Item.No, deserializedInventory.Item.No);
        Assert.Equal(originalInventory.Item.Name, deserializedInventory.Item.Name);
        Assert.Equal(originalInventory.Item.Type, deserializedInventory.Item.Type);
        Assert.Equal(originalInventory.Item.CategoryId, deserializedInventory.Item.CategoryId);
        Assert.Equal(originalInventory.ColorId, deserializedInventory.ColorId);
        Assert.Equal(originalInventory.Quantity, deserializedInventory.Quantity);
        Assert.Equal(originalInventory.NewOrUsed, deserializedInventory.NewOrUsed);
        Assert.Equal(originalInventory.UnitPrice, deserializedInventory.UnitPrice);
        Assert.Equal(originalInventory.BindId, deserializedInventory.BindId);
        Assert.Equal(originalInventory.Description, deserializedInventory.Description);
        Assert.Equal(originalInventory.Remarks, deserializedInventory.Remarks);
        Assert.Equal(originalInventory.Bulk, deserializedInventory.Bulk);
        Assert.Equal(originalInventory.IsRetain, deserializedInventory.IsRetain);
        Assert.Equal(originalInventory.IsStockRoom, deserializedInventory.IsStockRoom);
        Assert.Equal(originalInventory.StockRoomId, deserializedInventory.StockRoomId);
        Assert.Equal(originalInventory.DateCreated, deserializedInventory.DateCreated);
        Assert.Equal(originalInventory.TierQuantity1, deserializedInventory.TierQuantity1);
        Assert.Equal(originalInventory.TierPrice1, deserializedInventory.TierPrice1);
        Assert.Equal(originalInventory.TierQuantity2, deserializedInventory.TierQuantity2);
        Assert.Equal(originalInventory.TierPrice2, deserializedInventory.TierPrice2);
        Assert.Equal(originalInventory.TierQuantity3, deserializedInventory.TierQuantity3);
        Assert.Equal(originalInventory.TierPrice3, deserializedInventory.TierPrice3);
        Assert.Equal(originalInventory.MyCost, deserializedInventory.MyCost);
        Assert.Equal(originalInventory.SaleRate, deserializedInventory.SaleRate);
        Assert.Equal(originalInventory.MyWeight, deserializedInventory.MyWeight);
    }

    [Fact]
    public void Inventory_JsonDeserialization_HandlesNullValues()
    {
        // Arrange
        var json = """
        {
            "inventory_id": 999888777,
            "item": {
                "no": "3006",
                "name": "Brick 2 x 3",
                "type": "PART",
                "category_id": 5
            },
            "color_id": 3,
            "quantity": 5,
            "new_or_used": "N",
            "unit_price": 2.50,
            "bind_id": 0,
            "description": null,
            "remarks": null,
            "bulk": 1,
            "is_retain": false,
            "is_stock_room": false,
            "stock_room_id": null,
            "date_created": "2023-09-20T12:00:00.000Z",
            "tier_quantity1": null,
            "tier_price1": null,
            "tier_quantity2": null,
            "tier_price2": null,
            "tier_quantity3": null,
            "tier_price3": null,
            "my_cost": null,
            "sale_rate": null,
            "my_weight": null
        }
        """;

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var inventory = JsonSerializer.Deserialize<Inventory>(json, options);

        // Assert
        Assert.NotNull(inventory);
        Assert.Equal(999888777L, inventory.InventoryId);
        Assert.Null(inventory.Description);
        Assert.Null(inventory.Remarks);
        Assert.Null(inventory.StockRoomId);
        Assert.Null(inventory.TierQuantity1);
        Assert.Null(inventory.TierPrice1);
        Assert.Null(inventory.TierQuantity2);
        Assert.Null(inventory.TierPrice2);
        Assert.Null(inventory.TierQuantity3);
        Assert.Null(inventory.TierPrice3);
        Assert.Null(inventory.MyCost);
        Assert.Null(inventory.SaleRate);
        Assert.Null(inventory.MyWeight);
    }

    [Fact]
    public void Inventory_JsonDeserialization_HandlesMinimalJSON()
    {
        // Arrange - minimal required fields for inventory
        var json = """
        {
            "inventory_id": 123,
            "item": {
                "no": "3001",
                "name": "Brick 2 x 4",
                "type": "PART",
                "category_id": 5
            },
            "color_id": 4,
            "quantity": 1,
            "new_or_used": "N",
            "unit_price": 1.00,
            "bind_id": 0,
            "bulk": 1,
            "is_retain": false,
            "is_stock_room": false,
            "date_created": "2023-01-01T00:00:00.000Z"
        }
        """;

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var inventory = JsonSerializer.Deserialize<Inventory>(json, options);

        // Assert
        Assert.NotNull(inventory);
        Assert.Equal(123L, inventory.InventoryId);
        Assert.Equal("3001", inventory.Item.No);
        Assert.Equal("Brick 2 x 4", inventory.Item.Name);
        Assert.Equal(ItemType.Part, inventory.Item.Type);
        Assert.Equal(5, inventory.Item.CategoryId);
        Assert.Equal(4, inventory.ColorId);
        Assert.Equal(1, inventory.Quantity);
        Assert.Equal(NewOrUsed.New, inventory.NewOrUsed);
        Assert.Equal(1.00m, inventory.UnitPrice);
        Assert.Equal(0, inventory.BindId);
        Assert.Equal(1, inventory.Bulk);
        Assert.False(inventory.IsRetain);
        Assert.False(inventory.IsStockRoom);
        Assert.Equal(DateTimeOffset.Parse("2023-01-01T00:00:00.000Z"), inventory.DateCreated);
    }

    [Theory]
    [InlineData(NewOrUsed.New, "N")]
    [InlineData(NewOrUsed.Used, "U")]
    public void Inventory_NewOrUsedEnum_SerializesCorrectly(NewOrUsed condition, string expectedJson)
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 1,
            Item = new InventoryItem { No = "test", Name = "Test", Type = ItemType.Part, CategoryId = 1 },
            ColorId = 1,
            Quantity = 1,
            NewOrUsed = condition,
            UnitPrice = 1.00m,
            DateCreated = DateTimeOffset.Parse("2023-01-01T00:00:00.000Z")
        };

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var json = JsonSerializer.Serialize(inventory, options);

        // Assert
        Assert.Contains($"\"new_or_used\":\"{expectedJson}\"", json);
    }

    [Fact]
    public void Inventory_DecimalPrecision_HandlesFinancialValues()
    {
        // Arrange
        var inventory = new Inventory
        {
            InventoryId = 1,
            Item = new InventoryItem { No = "test", Name = "Test", Type = ItemType.Part, CategoryId = 1 },
            ColorId = 1,
            Quantity = 1,
            NewOrUsed = NewOrUsed.New,
            UnitPrice = 123.4567m,
            DateCreated = DateTimeOffset.Parse("2023-01-01T00:00:00.000Z"),
            TierPrice1 = 0.1234m,
            TierPrice2 = 999.9999m,
            TierPrice3 = 0.0001m,
            MyCost = 50.5050m,
            MyWeight = 12.3457m
        };

        var options = JsonSerializationHelper.DefaultOptions;

        // Act & Assert - Should not throw and should preserve precision
        var json = JsonSerializer.Serialize(inventory, options);
        var deserializedInventory = JsonSerializer.Deserialize<Inventory>(json, options);

        Assert.NotNull(deserializedInventory);
        Assert.Equal(123.4567m, deserializedInventory.UnitPrice);
        Assert.Equal(0.1234m, deserializedInventory.TierPrice1);
        Assert.Equal(999.9999m, deserializedInventory.TierPrice2);
        Assert.Equal(0.0001m, deserializedInventory.TierPrice3);
        Assert.Equal(50.5050m, deserializedInventory.MyCost);
        Assert.Equal(12.3457m, deserializedInventory.MyWeight);
    }

    [Fact]
    public void Inventory_DateTimeOffset_PreservesTimezoneInformation()
    {
        // Arrange
        var dateWithTimezone = new DateTimeOffset(2023, 12, 15, 14, 30, 45, 123, TimeSpan.FromHours(-5));
        var inventory = new Inventory
        {
            InventoryId = 1,
            Item = new InventoryItem { No = "test", Name = "Test", Type = ItemType.Part, CategoryId = 1 },
            ColorId = 1,
            Quantity = 1,
            NewOrUsed = NewOrUsed.New,
            UnitPrice = 1.00m,
            DateCreated = dateWithTimezone
        };

        var options = JsonSerializationHelper.DefaultOptions;

        // Act
        var json = JsonSerializer.Serialize(inventory, options);
        var deserializedInventory = JsonSerializer.Deserialize<Inventory>(json, options);

        // Assert
        Assert.NotNull(deserializedInventory);
        // The DateTimeOffset should be preserved through serialization
        Assert.Equal(dateWithTimezone.ToUniversalTime(), deserializedInventory.DateCreated.ToUniversalTime());
    }
}
