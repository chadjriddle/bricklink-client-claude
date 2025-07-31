# BrickLink API C# .NET 9.0 Client Library - MVP Tasks

This document outlines the high-level milestones and individual tasks required to complete the MVP for the BrickLink API client library.

## Project Scope (MVP)
- Authentication/Authorization (OAuth 1.0a-like signature)
- Simple consumer pattern for API interactions
- Retrieving set and part information from catalog
- **Excluded**: Order, Payment, Account functionality

## Milestone 1: Project Foundation & Structure

### 1.1 Project Setup
- [ ] Create .NET 9.0 class library project structure
- [ ] Configure project file with appropriate package metadata
- [ ] Set up solution structure with test project
- [ ] Configure EditorConfig and code style settings
- [ ] Add necessary NuGet package references (System.Text.Json, System.Net.Http)

### 1.2 Namespace & Architecture Design
- [ ] Define namespace structure (BrickLink.Client, BrickLink.Client.Auth, BrickLink.Client.Models, BrickLink.Client.Enums)
- [ ] Create base project structure with appropriate folders
- [ ] Define architectural interfaces and contracts

## Milestone 2: Core Infrastructure

### 2.1 Response Handling Framework
- [ ] Implement `ApiResponse<T>` generic wrapper class
- [ ] Implement `Meta` model for response metadata
- [ ] Create `BrickLinkApiException` for error handling
- [ ] Implement JSON serialization/deserialization with System.Text.Json
- [ ] Add proper DateTimeOffset and decimal handling for API requirements

### 2.2 HTTP Client Foundation
- [ ] Create base HTTP client wrapper
- [ ] Implement retry logic and error handling
- [ ] Add request/response logging capabilities
- [ ] Configure proper SSL/TLS and UTF-8 encoding

## Milestone 3: Authentication System

### 3.1 OAuth 1.0a-like Authentication Handler
- [ ] Implement `AuthenticationHandler` inheriting from `DelegatingHandler`
- [ ] Create credential management (ConsumerKey, ConsumerSecret, Token, TokenSecret)
- [ ] Implement signature generation logic (HMAC-SHA1)
- [ ] Add parameter collection and normalization (RFC3986 encoding)
- [ ] Implement signature base string construction
- [ ] Add nonce and timestamp generation
- [ ] Create Authorization header construction

### 3.2 Authentication Integration
- [ ] Integrate authentication handler into HttpClient pipeline
- [ ] Add authentication testing utilities
- [ ] Implement secure credential storage patterns

## Milestone 4: Data Models

### 4.1 Core Enumerations
- [ ] `ItemType` enum (PART, SET, MINIFIG, etc.)
- [ ] `NewOrUsed` enum (N, U)
- [ ] `Completeness` enum (C, B, S)
- [ ] `Direction` enum (in, out)

### 4.2 Catalog Item Models
- [ ] `CatalogItem` model with all properties
- [ ] `InventoryItem` model for basic item info
- [ ] `Color` model for color information
- [ ] `Category` model for item categorization

### 4.3 Inventory Models
- [ ] `Inventory` model for store inventory items
- [ ] Support for quantity, pricing, and condition fields

### 4.4 Advanced Catalog Models
- [ ] `SupersetEntry` and `SupersetItem` models
- [ ] `SubsetEntry` and `SubsetItem` models
- [ ] `PriceGuide` and `PriceDetail` models
- [ ] `ItemMapping` model for BrickLink/LEGO ID mapping

## Milestone 5: Core Client Implementation

### 5.1 Main Client Class
- [ ] Implement `BrickLinkClient` main class
- [ ] Add constructor with credential parameters
- [ ] Configure HttpClient with authentication handler
- [ ] Implement resource-specific property accessors

### 5.2 Base Service Classes
- [ ] Create base service class for common API operations
- [ ] Implement generic GET, POST, PUT, DELETE methods
- [ ] Add parameter serialization helpers
- [ ] Implement response deserialization with error handling

## Milestone 6: Catalog Services (MVP Core)

### 6.1 Item Catalog Service
- [ ] Implement `ICatalogService` interface
- [ ] `GetItemAsync(ItemType type, string itemNo)` - Get specific item details
- [ ] `GetItemImageAsync(ItemType type, string itemNo, int colorId)` - Get item image URLs
- [ ] `GetItemColorsAsync(ItemType type, string itemNo)` - Get known colors for item

### 6.2 Set and Part Information Services
- [ ] `GetSupersetsAsync(ItemType type, string itemNo, int? colorId)` - Get sets containing item
- [ ] `GetSubsetsAsync(ItemType type, string itemNo, bool? includeBox, bool? includeInstructions)` - Get set contents
- [ ] `GetPriceGuideAsync(ItemType type, string itemNo, int? colorId, string guideType, NewOrUsed? condition)` - Get pricing data

### 6.3 Reference Data Services
- [ ] `GetColorsAsync()` - Get all available colors
- [ ] `GetColorAsync(int colorId)` - Get specific color details
- [ ] `GetCategoriesAsync()` - Get all item categories
- [ ] `GetCategoryAsync(int categoryId)` - Get specific category

### 6.4 Item Mapping Service
- [ ] `GetElementIdAsync(string partNo, int? colorId)` - Convert BrickLink to LEGO Element ID
- [ ] `GetItemNumberAsync(string elementId)` - Convert LEGO Element ID to BrickLink

## Milestone 7: Consumer Pattern Implementation

### 7.1 Fluent API Design
- [ ] Design fluent interface for common operations
- [ ] Implement method chaining for filtering and querying
- [ ] Add builder patterns for complex queries

### 7.2 Caching Strategy
- [ ] Implement memory caching for reference data (colors, categories)
- [ ] Add cache invalidation and expiration policies
- [ ] Optimize for frequently accessed catalog data

## Milestone 8: Testing & Documentation

### 8.1 Unit Testing
- [ ] Create comprehensive unit tests for authentication
- [ ] Test all model serialization/deserialization
- [ ] Mock HTTP responses for API endpoint testing
- [ ] Test error handling and exception scenarios

### 8.2 Integration Testing
- [ ] Create integration tests with actual API (using test credentials)
- [ ] Test rate limiting and retry logic
- [ ] Validate authentication flow end-to-end

### 8.3 Documentation
- [ ] Create comprehensive XML documentation for all public APIs
- [ ] Write README with getting started guide
- [ ] Add code examples for common use cases
- [ ] Document authentication setup process

## Milestone 9: Packaging & Distribution

### 9.1 NuGet Package Preparation
- [ ] Configure NuGet package metadata
- [ ] Create package icon and documentation
- [ ] Set up semantic versioning
- [ ] Add license and attribution information

### 9.2 Build & CI Setup
- [ ] Configure build pipeline
- [ ] Add automated testing in CI
- [ ] Set up package publishing workflow

## MVP Acceptance Criteria

The MVP will be considered complete when:

1. **Authentication Works**: Can successfully authenticate and make signed requests to BrickLink API
2. **Basic Catalog Access**: Can retrieve item details, colors, and categories
3. **Set Information**: Can get set contents (subsets) and find sets containing specific parts (supersets)
4. **Part Information**: Can retrieve part details, available colors, and pricing information
5. **Simple Consumer Pattern**: Provides intuitive, strongly-typed C# API for common operations
6. **Proper Error Handling**: Gracefully handles API errors with meaningful exceptions
7. **Documentation**: Clear documentation and examples for getting started

## Future Enhancements (Post-MVP)
- Order management functionality
- Inventory management for store owners
- Webhook/push notification support
- Advanced caching strategies
- Bulk operations support
- Payment processing integration