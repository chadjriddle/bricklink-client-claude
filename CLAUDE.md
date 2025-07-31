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
```

## Test Commands

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/BrickLink.Client.Tests
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
3. **Test Coverage** - Include tests for each new piece of functionality
4. **Documentation** - Update XML docs and README as functionality is added
5. **Clean Code** - Follow C# coding conventions and ensure code is self-documenting

#### After Task Completion:
1. **Verify & Test** - Ensure all tests pass and code compiles successfully
2. **Commit Changes** - Make a single, atomic commit for the completed task
3. **Push Branch** - Push the feature branch to remote repository
   ```bash
   git push -u origin feature/task-name
   ```
4. **Merge to Main** - Merge the feature branch back to main (or create PR if using pull request workflow)
5. **Clean Up** - Delete the feature branch locally and remotely after successful merge

### Branch Naming Conventions
- `feature/milestone-1-project-setup` - For milestone tasks
- `feature/implement-oauth-handler` - For specific implementation tasks
- `feature/add-catalog-models` - For model/data structure tasks
- `hotfix/fix-authentication-bug` - For urgent bug fixes

### Commit Strategy
- Each commit should represent a complete, working feature or fix
- Commits should compile and pass all tests
- Use descriptive commit messages that explain the "why" not just the "what"
- Follow conventional commit format when possible: `feat:`, `fix:`, `docs:`, `test:`

## Documentation

See `docs/tasks.md` for detailed project milestones and task breakdown.
See `docs/BrickLink API Client Specification.md` for complete API specification.