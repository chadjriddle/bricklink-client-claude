# Review and Update PR

Help me review the latest pull request for comments, suggestions, and status checks, then implement necessary changes to address all feedback.

## What this prompt does:

1. **Fetch PR Information**:
   - Get details of the latest PR (or specify PR number)
   - Analyze all review comments from reviewers
   - Check CI/CD pipeline status and failure details
   - Review automated tool suggestions

2. **Analyze Feedback**:
   - Parse reviewer comments for requested changes
   - Identify failed tests, builds, or quality checks
   - Understand code coverage issues
   - Review formatting or documentation feedback

3. **Implement Changes**:
   - Address all reviewer feedback systematically
   - Fix failing tests or builds
   - Resolve code quality issues
   - Update documentation if requested
   - Maintain project coding standards

4. **Validate Updates**:
   - Run full test suite to ensure nothing breaks
   - Verify zero-warning build with `dotnet build -c Release`
   - Check code formatting with `dotnet format --verify-no-changes`
   - Generate coverage reports and verify thresholds
   - Test any new functionality thoroughly

5. **Update PR**:
   - Commit changes with descriptive messages explaining what was addressed
   - Push updates to the PR branch
   - Add explanatory comments for any suggestions not implemented
   - Respond to reviewer feedback appropriately

## Project Standards to Maintain:
- **Zero-warning policy**: Resolve all build warnings
- **Coverage requirements**: 85% minimum, 90% for auth, 95% for public APIs
- **C# 12 conventions**: Use modern language features appropriately
- **XML documentation**: Ensure all public APIs are documented
- **Testing patterns**: Follow xUnit with Moq for mocking
- **Async patterns**: Use proper `async`/`await` for I/O operations

## Quality Checks to Run:
```bash
# Formatting verification
dotnet format --verify-no-changes --verbosity diagnostic

# Zero-warning build
dotnet build -c Release

# Full test suite with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

# Coverage report generation
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/coveragereport" -reporttypes:Html
```

## Common PR Issues to Address:
- Missing XML documentation (CS1591 warnings)
- Insufficient test coverage
- Code formatting inconsistencies
- Missing error handling
- Non-nullable reference type issues
- Performance or security concerns

Please review the latest PR, implement all necessary changes, and ensure all quality gates pass before updating the PR.
