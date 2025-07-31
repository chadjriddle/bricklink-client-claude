# BrickLink API C# .NET 9.0 Client Library - MVP Tasks

This document outlines the high-level milestones and individual tasks required to complete the MVP for the BrickLink API client library.

## Project Scope (MVP)
- Authentication/Authorization (OAuth 1.0a-like signature)
- Simple consumer pattern for API interactions
- Retrieving set and part information from catalog
- **Excluded**: Order, Payment, Account functionality

## Milestone 1: Project Foundation & Structure

### 1.1 Basic Project Creation
- [ ] Create .NET 9.0 class library project
- [ ] Create xUnit test project
- [ ] Create solution file linking both projects

### 1.2 Project Configuration
- [ ] Configure main project file with package metadata (name, description, version)
- [ ] Add System.Text.Json NuGet package reference
- [ ] Add System.Net.Http NuGet package reference
- [ ] Configure EditorConfig for C# formatting rules
- [ ] Add code style settings (.editorconfig)

### 1.3 Project Structure Setup
- [ ] Create BrickLink.Client root namespace folder
- [ ] Create BrickLink.Client.Auth namespace folder
- [ ] Create BrickLink.Client.Models namespace folder
- [ ] Create BrickLink.Client.Enums namespace folder
- [ ] Create BrickLink.Client.Services namespace folder

### 1.4 Architecture Contracts
- [ ] Define IApiClient interface for main client contract
- [ ] Define IAuthenticationHandler interface
- [ ] Define IApiService base interface for service contracts

## Milestone 2: Core Infrastructure

### 2.1 API Response Models
- [ ] Implement `ApiResponse<T>` generic wrapper class
- [ ] Implement `Meta` model for response metadata
- [ ] Add JSON property attributes for serialization

### 2.2 Exception Handling
- [ ] Create `BrickLinkApiException` base exception class
- [ ] Add properties for status code, message, and description
- [ ] Implement exception factory methods for different error types

### 2.3 JSON Serialization Setup
- [ ] Configure JsonSerializerOptions for API requirements
- [ ] Add custom JsonConverter for DateTimeOffset handling
- [ ] Add custom JsonConverter for decimal precision handling
- [ ] Create serialization helper class

### 2.4 HTTP Client Wrapper
- [ ] Create base HttpClient wrapper class
- [ ] Configure SSL/TLS and UTF-8 encoding
- [ ] Add base URL configuration

### 2.5 Retry Logic Implementation
- [ ] Implement retry policy for transient failures
- [ ] Add exponential backoff strategy
- [ ] Configure retry limits and timeout handling

### 2.6 Request/Response Logging
- [ ] Create logging interfaces and abstractions
- [ ] Implement request logging functionality
- [ ] Implement response logging functionality

## Milestone 3: Authentication System

### 3.1 Credential Management
- [ ] Create `BrickLinkCredentials` class with required properties
- [ ] Add validation for required credential fields
- [ ] Implement secure string handling for secrets

### 3.2 OAuth Parameter Generation
- [ ] Implement nonce generation utility
- [ ] Implement timestamp generation utility
- [ ] Create OAuth parameter collection class

### 3.3 String Encoding and Normalization
- [ ] Implement RFC3986 percent encoding utility
- [ ] Create parameter normalization methods
- [ ] Add parameter sorting functionality

### 3.4 Signature Generation
- [ ] Implement signature base string construction
- [ ] Create HMAC-SHA1 signature generation
- [ ] Add signature key construction logic

### 3.5 Authorization Header Construction
- [ ] Create Authorization header string builder
- [ ] Implement OAuth parameter formatting
- [ ] Add header validation

### 3.6 Authentication Handler
- [ ] Implement `AuthenticationHandler` inheriting from `DelegatingHandler`
- [ ] Integrate all authentication components
- [ ] Add request interception and modification

### 3.7 Authentication Integration
- [ ] Create HttpClient factory with authentication
- [ ] Add authentication handler to client pipeline
- [ ] Implement credential injection patterns

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

### 5.1 Base Service Architecture
- [ ] Create `BaseApiService` abstract class
- [ ] Add HttpClient dependency injection
- [ ] Implement base URL construction

### 5.2 HTTP Method Implementations
- [ ] Implement generic GET method with response handling
- [ ] Implement generic POST method with request/response handling
- [ ] Implement generic PUT method with request/response handling
- [ ] Implement generic DELETE method with response handling

### 5.3 Parameter and Response Handling
- [ ] Create query parameter serialization helpers
- [ ] Implement request body serialization
- [ ] Add response deserialization with error checking
- [ ] Create response validation and exception throwing

### 5.4 Main Client Class Structure
- [ ] Create `BrickLinkClient` class with credential constructor
- [ ] Configure HttpClient with authentication handler
- [ ] Add client disposal pattern implementation

### 5.5 Service Property Accessors
- [ ] Add ICatalogService property accessor
- [ ] Add IColorService property accessor  
- [ ] Add ICategoryService property accessor
- [ ] Add IItemMappingService property accessor

## Milestone 6: Catalog Services (MVP Core)

### 6.1 Catalog Service Interface
- [ ] Define `ICatalogService` interface with method signatures
- [ ] Create `CatalogService` class inheriting from `BaseApiService`

### 6.2 Basic Item Operations
- [ ] Implement `GetItemAsync(ItemType type, string itemNo)` method
- [ ] Implement `GetItemImageAsync(ItemType type, string itemNo, int colorId)` method
- [ ] Implement `GetItemColorsAsync(ItemType type, string itemNo)` method

### 6.3 Set Relationship Operations
- [ ] Implement `GetSupersetsAsync(ItemType type, string itemNo, int? colorId)` method
- [ ] Implement `GetSubsetsAsync(ItemType type, string itemNo, bool? includeBox, bool? includeInstructions)` method

### 6.4 Price Guide Operations
- [ ] Implement `GetPriceGuideAsync(ItemType type, string itemNo, int? colorId, string guideType, NewOrUsed? condition)` method

### 6.5 Color Service Implementation
- [ ] Define `IColorService` interface
- [ ] Create `ColorService` class inheriting from `BaseApiService`
- [ ] Implement `GetColorsAsync()` method
- [ ] Implement `GetColorAsync(int colorId)` method

### 6.6 Category Service Implementation
- [ ] Define `ICategoryService` interface
- [ ] Create `CategoryService` class inheriting from `BaseApiService`
- [ ] Implement `GetCategoriesAsync()` method
- [ ] Implement `GetCategoryAsync(int categoryId)` method

### 6.7 Item Mapping Service Implementation
- [ ] Define `IItemMappingService` interface
- [ ] Create `ItemMappingService` class inheriting from `BaseApiService`
- [ ] Implement `GetElementIdAsync(string partNo, int? colorId)` method
- [ ] Implement `GetItemNumberAsync(string elementId)` method

## Milestone 7: Consumer Pattern Implementation

### 7.1 Query Builder Interfaces
- [ ] Create `ICatalogQueryBuilder` interface for fluent catalog queries
- [ ] Create `IPriceGuideQueryBuilder` interface for price guide queries
- [ ] Define builder method signatures for common operations

### 7.2 Query Builder Implementations
- [ ] Implement `CatalogQueryBuilder` with method chaining
- [ ] Implement `PriceGuideQueryBuilder` with filtering options
- [ ] Add query execution methods that return results

### 7.3 Caching Infrastructure
- [ ] Create `IApiCache` interface for caching abstraction
- [ ] Implement `MemoryApiCache` using IMemoryCache
- [ ] Add cache key generation utilities

### 7.4 Caching Integration
- [ ] Add caching to color service methods
- [ ] Add caching to category service methods  
- [ ] Implement cache invalidation strategies
- [ ] Configure cache expiration policies

## Milestone 8: Testing & Documentation

### 8.1 Authentication Unit Tests
- [ ] Test OAuth signature generation with known values
- [ ] Test parameter normalization and encoding
- [ ] Test nonce and timestamp generation
- [ ] Test credential validation

### 8.2 Model Serialization Tests
- [ ] Test ApiResponse<T> serialization/deserialization
- [ ] Test all model classes with JSON samples
- [ ] Test DateTimeOffset and decimal handling
- [ ] Test enum serialization with string values

### 8.3 Service Unit Tests
- [ ] Create mock HTTP responses for each service method
- [ ] Test catalog service methods with mocked responses
- [ ] Test color and category service methods
- [ ] Test error handling and exception scenarios

### 8.4 Integration Test Setup
- [ ] Create integration test project structure
- [ ] Set up test credentials configuration
- [ ] Create test data fixtures and helpers

### 8.5 Integration Test Implementation
- [ ] Test authentication flow end-to-end with real API
- [ ] Test basic catalog operations against live API
- [ ] Test rate limiting and retry logic
- [ ] Validate error handling with actual API errors

### 8.6 API Documentation
- [ ] Add XML documentation to all public interfaces
- [ ] Add XML documentation to all public classes
- [ ] Add XML documentation to all public methods
- [ ] Add parameter and return value documentation

### 8.7 Usage Documentation
- [ ] Update README with installation instructions
- [ ] Add getting started guide with code examples
- [ ] Document authentication setup process
- [ ] Add troubleshooting section for common issues

## Milestone 9: Packaging & Distribution

### 9.1 NuGet Package Metadata
- [ ] Configure package ID, title, and description in project file
- [ ] Add author, copyright, and repository information
- [ ] Set up semantic versioning with initial version
- [ ] Add package tags and categories

### 9.2 Package Assets
- [ ] Create package icon (64x64 PNG)
- [ ] Add README.md to package
- [ ] Include LICENSE file in package
- [ ] Add release notes template

### 9.3 Build Configuration
- [ ] Configure Release build configuration
- [ ] Set up XML documentation generation
- [ ] Configure assembly signing (if needed)
- [ ] Add package validation rules

### 9.4 CI/CD Pipeline Setup
- [ ] Create GitHub Actions workflow file
- [ ] Add automated build on push/PR
- [ ] Configure automated test execution
- [ ] Set up NuGet package publishing on release tags

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