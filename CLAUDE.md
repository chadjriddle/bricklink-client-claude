# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Quick Reference

### Essential Commands
```bash
# Build & verify (run before every commit)
dotnet build -c Release                          # Zero warnings required
dotnet format --verify-no-changes --verbosity diagnostic
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

# Coverage report (review before commit)
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/coveragereport" -reporttypes:Html
start ./TestResults/coveragereport/index.html

# Code formatting
dotnet format                                    # Auto-fix formatting issues
```

### Critical Policies
- **Zero-Warning Policy**: All build warnings must be resolved before commit
- **Coverage Requirements**: 85% minimum (90% auth, 95% public APIs)
- **Mandatory PR Workflow**: No direct commits to main
- **Just-In-Time Interfaces**: Create interfaces only when implementing concrete classes

---

## Project Overview

This is a C# .NET 9.0 client library for the BrickLink API. The project is focused on creating an MVP that provides authentication/authorization and catalog data access (sets and parts information). Order, Payment, and Account functionality are explicitly excluded from the MVP scope.

### Technology Stack
- **Framework**: .NET 9.0 Class Library
- **Language**: C# 12
- **HTTP Client**: System.Net.Http with custom DelegatingHandler for OAuth authentication
- **JSON Serialization**: System.Text.Json
- **Authentication**: OAuth 1.0a-like signature scheme with HMAC-SHA1
- **Testing**: xUnit
- **Documentation**: XML documentation comments

### MVP Scope
**Included:**
- OAuth 1.0a-like authentication system
- Catalog item retrieval (parts, sets, minifigures)
- Set contents (subsets) and part usage (supersets)
- Price guide information
- Color and category reference data
- Item mapping between BrickLink and LEGO Element IDs

**Excluded:**
- Order management, Payment processing, Account management
- Inventory management, Webhook/push notifications

## Project Architecture

### Namespace Structure
- `BrickLink.Client` - Main client class and core functionality
- `BrickLink.Client.Auth` - OAuth authentication handler and related classes
- `BrickLink.Client.Models` - Data models for API resources (CatalogItem, etc.)
- `BrickLink.Client.Enums` - Enumeration types (ItemType, etc.)
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

### Data Type Mappings
Critical mappings for data integrity:
- **Financial values** → `decimal` (prevents floating-point errors)
- **Timestamps** → `DateTimeOffset` (preserves timezone info)
- **Identifiers** → `long` (accommodates large IDs)
- **Controlled vocabularies** → C# enums (compile-time safety)

## Development Workflow

### Core Principles
- **Atomic Units of Work** - Each task = single, complete, testable unit
- **Just-In-Time Interface Creation** - Create interfaces ONLY when implementing concrete classes
- **Zero-Warning Policy** - All build warnings must be resolved before commit
- **Mandatory PR Workflow** - No direct commits to main branch

### Task Workflow Summary
1. **Start**: Pull latest, create feature branch, verify clean state
2. **Develop**: One task per branch, complete implementation with tests
3. **Verify**: Run full verification commands (see Quick Reference)
4. **Commit**: Single atomic commit with coverage results
5. **PR**: Create PR, get approval, squash merge, cleanup branch

### Common Build Warnings & Fixes
- **CS1591**: Missing XML docs → Add `/// <summary>` to public members
- **CS0168/CS0219**: Unused variables → Remove or use discard `_`
- **CS8618**: Non-nullable reference types → Initialize or mark nullable

### Coverage Requirements
- **85%** minimum for all new code
- **90%** minimum for authentication components (critical security code)
- **95%** minimum for public APIs (external-facing interfaces)
- **100%** for exception paths (all error handling must be tested)

### Coverage Setup (One-time)
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Branch Naming Conventions
- `feature/milestone-X-task-name` - For milestone tasks
- `feature/implement-oauth-handler` - For specific implementations
- `hotfix/fix-authentication-bug` - For urgent fixes

### PR Requirements
- **Title Format**: `feat:`, `fix:`, `docs:`, `test:`, `refactor:`
- **Review Required**: All PRs need approval before merging
- **Merge Strategy**: Use "Squash and merge" for clean history

### PR Template
```markdown
## Changes Made
- [Specific changes implemented]

## Testing
- All tests pass: ✅
- Coverage: X% (meets requirements)
- HTML coverage report reviewed: ✅

## Checklist
- [ ] Code follows project conventions
- [ ] Coverage thresholds met
- [ ] XML documentation updated
- [ ] No breaking changes
```

## Documentation

See `docs/tasks.md` for detailed project milestones and task breakdown.
See `docs/BrickLink API Client Specification.md` for complete API specification.