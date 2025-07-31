# BrickLink API Client for .NET

A modern, strongly-typed C# client library for the BrickLink API, built on .NET 9.0.

## Overview

This client library provides easy access to the BrickLink API for retrieving catalog information about LEGO parts, sets, and minifigures. It handles the complex OAuth 1.0a-like authentication automatically and provides a clean, intuitive API for .NET developers.

## Features

- **Automatic Authentication** - Handles OAuth 1.0a-like signature generation transparently
- **Strongly Typed** - All API responses are mapped to strongly-typed C# models
- **Async/Await Support** - Built with modern async patterns for optimal performance
- **Comprehensive Catalog Access** - Retrieve detailed information about parts, sets, and minifigures
- **Price Guide Integration** - Access current and historical pricing data
- **Set Analysis** - Find what sets contain specific parts and what parts are in sets
- **Error Handling** - Robust error handling with meaningful exceptions

## Current Status

🚧 **This project is currently in development (MVP phase)**

The MVP focuses on:
- ✅ Authentication/Authorization system
- ✅ Catalog item retrieval (parts, sets, minifigures)
- ✅ Set contents and part usage queries
- ✅ Price guide information
- ✅ Reference data (colors, categories)

**Not included in MVP:**
- Order management
- Payment processing
- Account management
- Store inventory management

## Quick Start

### Installation

```bash
# This package is not yet published to NuGet
# Clone and build locally for now
git clone https://github.com/your-org/bricklink-client-claude.git
cd bricklink-client-claude
dotnet build
```

### Authentication Setup

1. Register as a developer at [BrickLink API](https://www.bricklink.com/v3/api.page)
2. Obtain your Consumer Key and Consumer Secret
3. Register your application's IP address to get Access Token and Token Secret

### Basic Usage

```csharp
using BrickLink.Client;

// Initialize the client with your credentials
var client = new BrickLinkClient(
    consumerKey: "your-consumer-key",
    consumerSecret: "your-consumer-secret", 
    token: "your-access-token",
    tokenSecret: "your-token-secret"
);

// Get information about a specific part
var part = await client.Catalog.GetItemAsync(ItemType.PART, "3001");
Console.WriteLine($"Part: {part.Name}");

// Find all sets that contain this part
var supersets = await client.Catalog.GetSupersetsAsync(ItemType.PART, "3001");
foreach (var superset in supersets)
{
    Console.WriteLine($"Found in set: {superset.Item.Name}");
}

// Get current pricing information
var priceGuide = await client.Catalog.GetPriceGuideAsync(
    ItemType.PART, "3001", 
    colorId: 1, // White
    guideType: "stock" // Current listings
);
Console.WriteLine($"Average price: {priceGuide.AvgPrice:C}");
```

## API Coverage

### Catalog Services ✅
- Get item details, images, and specifications
- Retrieve set contents (subsets) and part usage (supersets) 
- Access price guide data for parts and sets
- Query available colors for items
- Browse item categories

### Reference Data ✅
- Colors and color information
- Item categories and hierarchy
- Item type classifications

### Utilities ✅
- Convert between BrickLink item numbers and LEGO Element IDs
- Image URL generation for parts in specific colors

## Authentication

This library handles BrickLink's OAuth 1.0a-like authentication automatically. Each API request is signed with HMAC-SHA1 using your credentials. The complex signature generation process is completely abstracted away.

**Security Note:** Never commit your API credentials to source control. Use environment variables, configuration files, or secure credential storage.

## Error Handling

The library provides comprehensive error handling:

```csharp
try 
{
    var item = await client.Catalog.GetItemAsync(ItemType.PART, "invalid-part");
}
catch (BrickLinkApiException ex)
{
    Console.WriteLine($"API Error {ex.StatusCode}: {ex.Message}");
    Console.WriteLine($"Details: {ex.Description}");
}
```

## Requirements

- .NET 9.0 or later
- Valid BrickLink API credentials
- Static IP address (required by BrickLink API)

## Contributing

This project is in active development. See [docs/tasks.md](docs/tasks.md) for current development tasks and milestones.

## Documentation

- [API Specification](docs/BrickLink%20API%20Client%20Specification.md) - Complete API documentation
- [Development Tasks](docs/tasks.md) - Project milestones and task breakdown
- [Developer Guide](CLAUDE.md) - Technical implementation details

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer

This is an unofficial client library. BrickLink is a trademark of BrickLink Corporation. This project is not affiliated with or endorsed by BrickLink Corporation or the LEGO Group.