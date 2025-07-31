# GitHub Copilot Instructions

This file provides guidance to GitHub Copilot when working with code in this repository.

## Project Overview

This is a C# .NET 9.0 client library for the BrickLink API. The project is focused on creating an MVP that provides authentication/authorization and catalog data access (sets and parts information). Order, Payment, and Account functionality are explicitly excluded from the MVP scope.

## Technology Stack

- **Framework**: .NET 9.0 Class Library
- **Language**: C# 12
- **HTTP Client**: System.Net.Http with custom DelegatingHandler for OAuth authentication
- **JSON Serialization**: System.Text.Json
- **Authentication**: OAuth 1.0a-like signature scheme with HMAC-SHA1
- **Testing**: xUnit with Moq for mocking
- **Documentation**: XML documentation comments

## Build Commands

```bash
# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Restore NuGet packages
dotnet restore

# Format code (auto-fix issues)
dotnet format

# Verify formatting (check without fixing)
dotnet format --verify-no-changes --verbosity diagnostic
```

## Test Commands

```bash
# Run all tests
dotnet test

# Run tests with coverage and generate HTML report
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/coveragereport" -reporttypes:Html

# Run specific test project with coverage
dotnet test BrickLink.Client.Tests --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

# View coverage report (opens HTML report in default browser)
start ./TestResults/coveragereport/index.html    # Windows
open ./TestResults/coveragereport/index.html     # macOS
xdg-open ./TestResults/coveragereport/index.html # Linux
```

## Project Architecture

### Namespace Structure
- `BrickLink.Client` - Main client class and core functionality
- `BrickLink.Client.Auth` - OAuth authentication handler and related classes
- `BrickLink.Client.Models` - Data models for API resources (Order, Inventory, CatalogItem, etc.)
- `BrickLink.Client.Enums` - Enumeration types (OrderStatus, ItemType, etc.)
- `BrickLink.Client.Services` - Service classes for different API resource categories

### Key Components
- **BrickLinkClient** - Main entry point, configured with OAuth credentials
- **AuthenticationHandler** - Custom DelegatingHandler for OAuth 1.0a signature generation
- **ApiResponse<T>** - Generic wrapper for all API responses with metadata
- **Catalog Services** - Services for retrieving set and part information from BrickLink catalog

### Authentication Flow
The API uses a simplified OAuth 1.0a-like authentication where:
1. Consumer Key/Secret obtained from BrickLink developer registration
2. Access Token/Token Secret obtained after IP address registration
3. Each request requires HMAC-SHA1 signature with oauth_nonce and oauth_timestamp
4. Authentication is handled transparently by the AuthenticationHandler

## Data Type Mappings

Critical mappings for data integrity:
- **Financial values** → `decimal` (prevents floating-point errors)
- **Timestamps** → `DateTimeOffset` (preserves timezone info)
- **Identifiers** → `long` (accommodates large IDs)
- **Controlled vocabularies** → C# enums (compile-time safety)

## Code Coverage Requirements

### Coverage Tools Setup
```bash
# Install ReportGenerator globally (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Verify installation
reportgenerator -help
```

### Coverage Standards
- **Minimum Coverage**: 85% for all new code
- **Authentication Components**: 90% minimum (critical security code)
- **Public APIs**: 95% minimum (external-facing interfaces)
- **Exception Paths**: 100% (all error handling must be tested)

### Coverage Workflow Integration
Every task completion must include:
1. Running tests with coverage collection
2. Generating HTML coverage report (for local development)
3. Reviewing coverage metrics in browser
4. Meeting minimum coverage thresholds before commit
5. Including coverage summary in commit message when relevant

**Note**: GitHub Actions CI/CD pipeline uses Codecov for automated coverage reporting and PR status checks. Local HTML reports are for development verification only.

### Coverage Report Structure
- **Line Coverage**: Percentage of executed code lines
- **Branch Coverage**: Percentage of executed decision paths
- **Method Coverage**: Percentage of methods with at least one test
- **Class Coverage**: Percentage of classes with test coverage

### Exclusions from Coverage
- Auto-generated code (marked with `[ExcludeFromCodeCoverage]`)
- Model classes with only properties (data transfer objects)
- Explicit interface implementations that delegate to main implementation

## Development Environment

- **Platform**: Windows
- **IDE**: Visual Studio 2022 or VS Code with C# extension
- **Directory**: `C:\code\ClaudeCode\bricklink-client-claude`
- **Git Repository**: Active git repository with main branch

## Current Status

**Project Progress**: Milestone 2 (Core Infrastructure) completed
- ✅ Milestone 1: Project Foundation & Structure (Completed)
- ✅ Milestone 2: Core Infrastructure (Completed) 
  - API Response Models, Exception Handling, JSON Serialization
  - HTTP Client Wrapper, Retry Logic, Request/Response Logging
- 🚧 Milestone 3: Authentication System (Next)

**CI/CD Pipeline**: Fully operational with GitHub Actions
- Automated testing with xUnit
- Code coverage reporting via Codecov (85% minimum threshold)
- Security scanning and formatting checks
- Mandatory PR workflow with quality gates

## MVP Scope

**Included:**
- OAuth 1.0a-like authentication system
- Catalog item retrieval (parts, sets, minifigures)
- Set contents (subsets) and part usage (supersets)
- Price guide information
- Color and category reference data
- Item mapping between BrickLink and LEGO Element IDs

**Excluded:**
- Order management
- Payment processing
- Account management
- Inventory management (for store owners)
- Webhook/push notifications

## Development Principles

### Code Quality Standards
Follow these principles when generating or suggesting code:

- **Single Responsibility** - Each class, method, and component should have one clear purpose
- **Atomic Implementation** - Build functionality incrementally with each step being testable
- **Clear Boundaries** - Separate concerns cleanly (authentication, models, services) with well-defined interfaces
- **Comprehensive Testing** - Include unit tests for all new functionality
- **XML Documentation** - Document all public APIs with `/// <summary>` comments

### CRITICAL: Just-In-Time Interface Creation
**Create interfaces and contracts only when implementing their concrete classes - never in advance.**

**Rules for Interface Creation:**
- **Create interfaces ONLY when implementing their concrete classes** - No "architectural contracts" in advance
- **Design interfaces based on actual implementation needs** - Not theoretical requirements
- **One interface per implementation** - Focus on immediate, concrete needs
- **No placeholder types** - Create real models based on actual API responses
- **Validate interface design against real data** - Test with actual BrickLink API responses

**Example of CORRECT approach:**
```csharp
// ✅ Create IAuthenticationHandler only when implementing AuthenticationHandler
public interface IAuthenticationHandler
{
    Task<HttpRequestMessage> AuthenticateRequestAsync(HttpRequestMessage request);
}

public class AuthenticationHandler : DelegatingHandler, IAuthenticationHandler
{
    // Implementation based on actual OAuth 1.0a requirements
}
```

### CRITICAL: Zero-Warning Policy
**All build warnings must be resolved before any commit.**

**Common Warning Types & Resolutions:**
- **CS1591 - Missing XML documentation**: Add `/// <summary>` documentation to all public members
- **CS0168 - Variable declared but not used**: Remove unused variables or use discard `_`
- **CS8618 - Non-nullable reference types**: Initialize properties or mark as nullable

**Always run before suggesting code:**
```bash
dotnet build -c Release  # Must show zero warnings
```

### Task Workflow
When implementing features:

1. **Create Feature Branch** - Use naming convention: `feature/task-description`
2. **Implement with Tests** - Include comprehensive unit tests
3. **Verify Coverage** - Ensure minimum 85% code coverage
4. **Format Code** - Run `dotnet format` and verify with `--verify-no-changes`
5. **Zero Warnings** - Build in Release mode with no warnings
6. **Update Documentation** - Include XML docs for public APIs

### Required Quality Checks
Before any code suggestion or generation:

1. **Formatting Verification**
   ```bash
   dotnet format --verify-no-changes --verbosity diagnostic
   ```
2. **Build Verification** 
   ```bash
   dotnet build -c Release  # Must be zero warnings
   ```
3. **Test Coverage**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   # Generate and review HTML coverage report
   ```

## Branch Naming Conventions
- `feature/milestone-X-task-name` - For milestone tasks
- `feature/implement-oauth-handler` - For specific implementations
- `feature/add-catalog-models` - For model/data structure tasks
- `hotfix/fix-authentication-bug` - For urgent bug fixes

## Pull Request Requirements

### PR Template Elements
- **Title Format**: Use conventional commit prefixes: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`
- **Coverage Report**: Include coverage percentage and confirm HTML report review
- **Review Required**: All PRs require approval before merging
- **Status Checks**: All automated tests must pass

### Commit Strategy
- Each commit should represent complete, working functionality
- Use descriptive commit messages explaining the "why"
- Follow conventional commit format
- Ensure commits compile and pass all tests

## Documentation

- See `docs/tasks.md` for detailed project milestones and task breakdown
- See `docs/BrickLink API Client Specification.md` for complete API specification
- All public APIs must have XML documentation comments
- Update README.md for user-facing changes

## Code Generation Guidelines

When generating code suggestions:

1. **Follow C# 12 conventions** - Use modern C# features appropriately
2. **Include comprehensive error handling** - Use custom exceptions from `BrickLink.Client.Exceptions`
3. **Add XML documentation** - Document all public members
4. **Design for testability** - Use dependency injection and interfaces
5. **Follow async patterns** - Use `async`/`await` for I/O operations
6. **Validate inputs** - Check parameters and throw appropriate exceptions
7. **Use nullable reference types** - Be explicit about nullability
8. **Apply data type mappings** - Use `decimal` for money, `DateTimeOffset` for timestamps, etc.

## Testing Guidelines

When suggesting test code:

1. **Use xUnit framework** - Follow existing test patterns
2. **Mock external dependencies** - Use Moq for HTTP clients and external services
3. **Test error conditions** - Include negative test cases
4. **Achieve coverage targets** - 85% minimum, 90% for auth, 95% for public APIs
5. **Use descriptive test names** - Clearly indicate what is being tested
6. **Arrange-Act-Assert pattern** - Structure tests clearly
7. **Test async methods properly** - Use appropriate async test patterns
