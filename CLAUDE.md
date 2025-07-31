# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a C# .NET 9.0 client library for the BrickLink API. The project is focused on creating an MVP that provides authentication/authorization and catalog data access (sets and parts information). Order, Payment, and Account functionality are explicitly excluded from the MVP scope.

## Technology Stack

- **Framework**: .NET 9.0 Class Library
- **Language**: C# 12
- **HTTP Client**: System.Net.Http with custom DelegatingHandler for OAuth authentication
- **JSON Serialization**: System.Text.Json
- **Authentication**: OAuth 1.0a-like signature scheme with HMAC-SHA1
- **Testing**: xUnit (planned)
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
dotnet test tests/BrickLink.Client.Tests --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

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
- **Git Status**: Not currently a git repository

## Current Permissions

Claude Code has limited bash permissions configured in `.claude/settings.local.json`:
- `dir` command access
- `ls` command access  
- `find` command access

Additional permissions may need to be granted for .NET CLI operations.

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

### Atomic Development & Single Responsibility
Every task and commit should follow these core principles:

- **Atomic Units of Work** - Each task should represent a single, complete unit of functionality that can be developed, tested, and committed independently
- **Single Responsibility** - Each class, method, and commit should have one clear purpose and reason to change
- **Incremental Progress** - Build functionality incrementally, with each step being a working, testable improvement
- **Clear Boundaries** - Separate concerns cleanly (authentication, models, services, etc.) with well-defined interfaces

### Task Workflow
When working on tasks from `docs/tasks.md`, follow this strict workflow:

#### Before Starting Any Task:
1. **Pull Latest Changes** - Always start with `git pull origin main` to ensure you have the latest code
2. **Create Feature Branch** - Create a new branch using the naming convention: `feature/task-description` or `feature/milestone-X-task-name`
   ```bash
   git checkout -b feature/implement-authentication-handler
   ```
3. **Verify Clean State** - Ensure working directory is clean and on the correct branch

#### During Task Development:
1. **One Task, One Branch** - Each task gets its own dedicated feature branch
2. **Complete Implementation** - Don't leave tasks partially implemented
3. **Test Coverage** - Include comprehensive unit tests for each new piece of functionality
4. **Documentation** - Update XML docs and README as functionality is added
5. **Clean Code** - Follow C# coding conventions and ensure code is self-documenting

#### CRITICAL: Just-In-Time Interface Creation
**DO NOT create interfaces, contracts, or placeholder types until they are immediately needed for the specific functional task being implemented.**

**Rules for Interface Creation:**
- **Create interfaces ONLY when implementing their concrete classes** - Never create "architectural contracts" in advance
- **Design interfaces based on actual implementation needs** - Not theoretical or anticipated requirements
- **One interface per task** - If a task requires multiple interfaces, break it into smaller tasks
- **No placeholder types** - Create real models based on actual API responses, not assumptions
- **Validate interface design against real data** - Test with actual BrickLink API responses before finalizing contracts

**Example of WRONG approach:**
```
❌ Task: "Create all service interfaces for future implementation"
❌ Creating IApiClient, ICatalogService, IColorService all at once
❌ Using placeholder models without real API data
```

**Example of CORRECT approach:**
```
✅ Task: "Implement authentication handler"
✅ Create IAuthenticationHandler only when implementing AuthenticationHandler class
✅ Task: "Implement catalog item retrieval"
✅ Create ICatalogService only when implementing CatalogService class
✅ Design CatalogItem model based on actual API response JSON
```

**Benefits of Just-In-Time Creation:**
- Interfaces reflect actual implementation needs, not assumptions
- Models match real API data structures
- Smaller, focused PRs that are easier to review
- Eliminates over-engineering and premature optimization
- True adherence to YAGNI (You Aren't Gonna Need It) principle
- Reduces coupling between components

#### CRITICAL: Zero-Warning Policy
**All build warnings must be resolved before any commit or PR creation.**

**Common Warning Types & Resolutions:**
- **CS1591 - Missing XML documentation**: Add `/// <summary>` documentation to all public types and members
- **CS0168 - Variable declared but not used**: Remove unused variables or use discard `_`
- **CS0219 - Variable assigned but not used**: Remove unused assignments
- **CS8618 - Non-nullable reference types**: Initialize properties or mark as nullable
- **Obsolete API warnings**: Replace with recommended alternatives

**Warning Resolution Workflow:**
1. Run `dotnet build -c Release` and capture all warnings
2. Address each warning individually 
3. Re-run build to confirm zero warnings
4. Only proceed with commit after achieving zero-warning build

#### After Task Completion:
1. **Run Full Test Suite** - Execute all tests to ensure nothing is broken
   ```bash
   dotnet test
   ```
2. **Generate Coverage Report** - Run tests with coverage and generate HTML report
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"
   reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/coveragereport" -reporttypes:Html
   ```
3. **Review Coverage Report** - Open and review the HTML coverage report to ensure adequate test coverage
   ```bash
   start ./TestResults/coveragereport/index.html  # Windows
   ```
4. **Coverage Requirements** - Ensure minimum 85% code coverage for new code, 90% for critical authentication components
5. **Formatting Verification** - Ensure code follows consistent formatting standards
   ```bash
   dotnet format --verify-no-changes --verbosity diagnostic
   ```
   **CRITICAL**: All formatting issues must be resolved before committing.
   - If formatting check fails, run `dotnet format` to auto-fix issues
   - Re-run verification to ensure all issues are resolved
   - Common issues: missing final newlines, inconsistent indentation, spacing
6. **Build Verification** - Ensure code compiles successfully with NO warnings in Release mode
   ```bash
   dotnet build -c Release
   ```
   **CRITICAL**: All build warnings must be resolved before committing. Zero-warning policy enforced.
   - Fix missing XML documentation warnings (CS1591)
   - Address any code analysis warnings
   - Resolve obsolete API usage warnings
   - Clean up unused using statements and variables
7. **Update Task Documentation** - Mark the completed task in `docs/tasks.md` with a checkbox ✅
   ```markdown
   - [x] 1.1 Basic Project Creation - Create .NET solution and projects
   ```
8. **Commit Changes** - Make a single, atomic commit for the completed task with coverage results
9. **Push Branch** - Push the feature branch to remote repository
   ```bash
   git push -u origin feature/task-name
   ```
10. **Create Pull Request** - Create a PR from the feature branch to main
   ```bash
   gh pr create --title "feat: [Task Description]" --body "Completes task: [task description]

   ## Changes Made
   - [List key changes]
   - [Include any architectural decisions]

   ## Testing
   - All tests pass
   - Coverage: [X]% (meets minimum requirements)
   - HTML coverage report generated and reviewed

   ## Checklist
   - [ ] Code follows project conventions
   - [ ] All tests pass
   - [ ] Coverage thresholds met
   - [ ] XML documentation updated
   - [ ] No breaking changes (or documented if necessary)
   "
   ```
11. **PR Review Process** - Wait for approval before merging
12. **Clean Up** - After PR is merged, delete the feature branch locally and remotely

### Branch Naming Conventions
- `feature/milestone-1-project-setup` - For milestone tasks
- `feature/implement-oauth-handler` - For specific implementation tasks
- `feature/add-catalog-models` - For model/data structure tasks
- `hotfix/fix-authentication-bug` - For urgent bug fixes

### Pull Request Workflow

This project uses a **mandatory PR workflow** - direct commits to main are not allowed.

#### PR Requirements
- **Title Format**: Use conventional commit prefixes: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`
- **Description**: Must include Changes Made, Testing summary, and completion checklist
- **Coverage Report**: Include coverage percentage and confirm HTML report was reviewed
- **Review Required**: All PRs require approval before merging
- **Status Checks**: All automated tests must pass

#### PR Template
```markdown
## Changes Made
- [Specific changes implemented]
- [Any architectural decisions or patterns introduced]

## Testing
- All tests pass: ✅
- Coverage: X% (meets minimum requirements)
- HTML coverage report generated and reviewed: ✅
- Integration tests (if applicable): ✅

## Checklist
- [ ] Code follows C# and project conventions
- [ ] All unit tests pass
- [ ] Coverage thresholds met (85%+ general, 90%+ auth, 95%+ public APIs)
- [ ] XML documentation added/updated for public APIs
- [ ] No breaking changes (or breaking changes documented)
- [ ] Task completion verified against acceptance criteria
```

#### After PR Approval
1. **Merge Strategy**: Use "Squash and merge" to maintain clean history
2. **Branch Cleanup**: GitHub will automatically delete the feature branch
3. **Local Cleanup**: Delete local feature branch after merge
   ```bash
   git checkout main
   git pull origin main
   git branch -d feature/task-name
   ```

### Commit Strategy
- Each commit should represent a complete, working feature or fix
- Commits should compile and pass all tests
- Use descriptive commit messages that explain the "why" not just the "what"
- Follow conventional commit format: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`

## Post-Development Workflow

### Final Task Completion Steps
- Ensure that `tasks.md` is updated before finishing and committing changes

## Documentation

See `docs/tasks.md` for detailed project milestones and task breakdown.
See `docs/BrickLink API Client Specification.md` for complete API specification.