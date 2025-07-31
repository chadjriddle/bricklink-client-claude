# Continue Next Task

Help me automatically determine and continue with the next pending task from the project's task list in `docs/tasks.md`.

## What this prompt does:

1. **Analyze Task List**:
   - Read `docs/tasks.md` to understand current project status
   - Identify completed vs pending tasks across all milestones
   - Determine the next logical task in the sequence

2. **Set Up Task Implementation**:
   - Break down complex tasks into manageable subtasks
   - Create feature branch following naming conventions (`feature/milestone-X-task-name`)
   - Ensure clean working directory before starting

3. **Implement Task Following Project Standards**:
   - Follow Just-In-Time interface creation principles (create interfaces only when implementing concrete classes)
   - Implement with comprehensive unit tests (85%+ coverage, 90%+ for authentication components)
   - Adhere to zero-warning build policy
   - Use proper C# 12 conventions and modern patterns

4. **Quality Assurance**:
   - Run full test suite with coverage reports
   - Verify formatting with `dotnet format --verify-no-changes`
   - Ensure zero-warning build with `dotnet build -c Release`
   - Generate and review HTML coverage report

5. **Complete Workflow**:
   - Update task documentation in `docs/tasks.md`
   - Create atomic commit with descriptive message
   - Push feature branch and create PR following established templates

## Project Context:
- This is a C# .NET 9.0 BrickLink API client library
- Focus on MVP scope: authentication and catalog data access
- Exclude: order management, payments, account management
- Use OAuth 1.0a-like authentication with HMAC-SHA1
- Follow namespace structure: `BrickLink.Client.*`

## Key Requirements:
- **Zero-warning policy**: All build warnings must be resolved
- **Coverage thresholds**: 85% minimum, 90% for auth, 95% for public APIs
- **Just-in-time interfaces**: Create interfaces only when implementing concrete classes
- **Atomic commits**: Each commit should be complete, working functionality
- **XML documentation**: Document all public APIs

Please analyze the current task status and implement the next pending task following these guidelines.
