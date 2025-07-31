# BrickLink API C# .NET 9.0 Client Library - MVP Tasks

This document outlines the high-level milestones and individual tasks required to complete the MVP for the BrickLink API client library.

## Development Approach (Updated)

**Just-In-Time Interface Creation**: This project follows a strict Just-In-Time approach for interface and contract creation. Interfaces are created ONLY when implementing their concrete classes, and models are designed based on actual API responses, not assumptions.

**Task Status Legend**:
- [x] Completed tasks
- [ ] Pending tasks  
- Tasks marked as "(Just-In-Time)" create interfaces only when implementing concrete classes
- Tasks marked as "(DEPRECATED)" used old "design-first" approach and are completed but not recommended for future work

## Project Scope (MVP)
- Authentication/Authorization (OAuth 1.0a-like signature)
- Simple consumer pattern for API interactions
- Retrieving set and part information from catalog
- **Excluded**: Order, Payment, Account functionality

## Milestone 1: Project Foundation & Structure

### 1.1 Basic Project Creation
- [x] Create .NET 9.0 class library project
- [x] Create xUnit test project
- [x] Create solution file linking both projects

### 1.2 Project Configuration
- [x] Configure main project file with package metadata (name, description, version)
- [x] Add System.Text.Json NuGet package reference
- [x] Add System.Net.Http NuGet package reference
- [x] Configure EditorConfig for C# formatting rules
- [x] Add code style settings (.editorconfig)

### 1.3 Project Structure Setup
- [x] Create BrickLink.Client root namespace folder
- [x] Create BrickLink.Client.Auth namespace folder
- [x] Create BrickLink.Client.Models namespace folder
- [x] Create BrickLink.Client.Enums namespace folder
- [x] Create BrickLink.Client.Services namespace folder

### 1.4 Architecture Contracts (DEPRECATED - COMPLETED WITH LEGACY APPROACH)
- [x] Define IApiClient interface for main client contract
- [x] Define IAuthenticationHandler interface
- [x] Define IApiService base interface for service contracts

**Note**: This task was completed using a legacy "design-first" approach that created all interfaces upfront with placeholder types. Future interface creation will follow Just-In-Time rules - creating interfaces only when implementing their concrete classes.

### 1.5 CI/CD and Coverage Integration
- [x] Create GitHub Actions workflow for CI/CD
- [x] Configure automated testing on feature branches and main
- [x] Set up Codecov integration for coverage reporting
- [x] Configure coverage thresholds and PR status checks
- [x] Add workflow badges to README.md

---

## ✅ **MILESTONE 1 COMPLETED** ✅

**Foundation & CI/CD Pipeline Successfully Established**

All Milestone 1 tasks have been completed, providing a solid foundation for BrickLink API client development:

**✅ Achievements:**
- Complete .NET 9.0 project structure with professional configuration
- Comprehensive CI/CD pipeline with automated testing, coverage reporting, and quality gates
- Zero-warning builds enforced with professional development standards
- 100% test coverage with meaningful unit tests validating the pipeline
- Cross-platform compatibility (LF line endings for Linux CI/CD runners)
- Professional README with badges and comprehensive documentation
- Established Just-In-Time Interface Creation principles for future development

**📊 Quality Metrics:**
- **Build Status**: ✅ Zero warnings in Release configuration
- **Test Coverage**: ✅ 100% line coverage (2/2 lines covered, 8 test cases)
- **CI/CD Pipeline**: ✅ All checks passing (Build, Test, Coverage, Security, Formatting)
- **Code Quality**: ✅ Professional formatting and documentation standards
- **Branch Protection**: ✅ Mandatory PR workflow with automated quality gates

**🚀 Ready for Development:**
The project foundation is now production-ready with automated quality assurance, comprehensive testing infrastructure, and professional development workflows. Development can proceed with confidence to **Milestone 2: Core Infrastructure**.

---

## Milestone 2: Core Infrastructure

### 2.1 API Response Models
- [x] Implement `ApiResponse<T>` generic wrapper class
- [x] Implement `Meta` model for response metadata
- [x] Add JSON property attributes for serialization

### 2.2 Exception Handling
- [x] Create `BrickLinkApiException` base exception class
- [x] Add properties for status code, message, and description
- [x] Implement exception factory methods for different error types

### 2.3 JSON Serialization Setup ✅
- [x] Configure JsonSerializerOptions for API requirements
- [x] Add custom JsonConverter for DateTimeOffset handling
- [x] Add custom JsonConverter for decimal precision handling
- [x] Create serialization helper class

**Completed**: Comprehensive JSON serialization infrastructure implemented with 96 unit tests, 88.5% coverage, and all PR review comments addressed.

### 2.4 HTTP Client Wrapper ✅
- [x] Create base HttpClient wrapper class
- [x] Configure SSL/TLS and UTF-8 encoding
- [x] Add base URL configuration

**Completed**: BrickLinkHttpClient wrapper implemented with comprehensive SSL/TLS configuration, UTF-8 encoding, base URL handling, and 37 unit tests achieving 100% line coverage.

### 2.5 Retry Logic Implementation ✅
- [x] Implement retry policy for transient failures
- [x] Add exponential backoff strategy
- [x] Configure retry limits and timeout handling

**Completed**: Comprehensive retry logic infrastructure implemented with exponential backoff, configurable policies, and robust error handling. Includes 68 unit tests achieving excellent coverage for all retry scenarios including transient failures, rate limiting, and timeout conditions.

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

### 3.6 Authentication Handler Implementation
- [ ] Create `IAuthenticationHandler` interface (Just-In-Time)
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

### 5.4 Main Client Class Implementation
- [ ] Create `IApiClient` interface (Just-In-Time)
- [ ] Create `BrickLinkClient` class with credential constructor
- [ ] Configure HttpClient with authentication handler
- [ ] Add client disposal pattern implementation
- [ ] Add service property accessors as services are implemented

## Milestone 6: Catalog Services (MVP Core)

### 6.1 Catalog Service Implementation
- [ ] Define `ICatalogService` interface based on actual API requirements
- [ ] Create `CatalogService` class inheriting from `BaseApiService`
- [ ] Design models based on actual BrickLink API responses (no placeholders)

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
- [ ] Define `IColorService` interface based on actual API requirements
- [ ] Create `ColorService` class inheriting from `BaseApiService`
- [ ] Design Color models from actual API responses
- [ ] Implement `GetColorsAsync()` method
- [ ] Implement `GetColorAsync(int colorId)` method

### 6.6 Category Service Implementation
- [ ] Define `ICategoryService` interface based on actual API requirements
- [ ] Create `CategoryService` class inheriting from `BaseApiService`
- [ ] Design Category models from actual API responses
- [ ] Implement `GetCategoriesAsync()` method
- [ ] Implement `GetCategoryAsync(int categoryId)` method

### 6.7 Item Mapping Service Implementation
- [ ] Define `IItemMappingService` interface based on actual API requirements
- [ ] Create `ItemMappingService` class inheriting from `BaseApiService`
- [ ] Design ItemMapping models from actual API responses
- [ ] Implement `GetElementIdAsync(string partNo, int? colorId)` method
- [ ] Implement `GetItemNumberAsync(string elementId)` method

## Milestone 7: Consumer Pattern Implementation

### 7.1 Catalog Query Builder Implementation
- [ ] Create `ICatalogQueryBuilder` interface (Just-In-Time)
- [ ] Implement `CatalogQueryBuilder` with method chaining
- [ ] Add query execution methods that return results

### 7.2 Price Guide Query Builder Implementation
- [ ] Create `IPriceGuideQueryBuilder` interface (Just-In-Time)
- [ ] Implement `PriceGuideQueryBuilder` with filtering options
- [ ] Add query execution methods that return results

### 7.3 Caching Infrastructure Implementation
- [ ] Create `IApiCache` interface (Just-In-Time)
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