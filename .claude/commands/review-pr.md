---
description: Review the last PR for comments, suggestions, and checks. Analyze feedback, revise code, validate changes, and update the PR.
allowed-tools:
  - Bash
  - Read
  - Write
  - Edit
  - MultiEdit
  - Grep
  - Glob
  - LS
  - TodoWrite
  - WebFetch
argument-hint: "[pr-number] (optional - defaults to latest PR)"
---

# Review and Update PR

This command will:

1. **Fetch the latest PR** (or specified PR number from arguments)
2. **Analyze all feedback** including:
   - Review comments from reviewers
   - Failed CI/CD checks and their details
   - Automated suggestions from tools
   - Status check failures
3. **Revise code accordingly** by:
   - Implementing requested changes
   - Fixing failing tests or builds
   - Addressing code quality issues
   - Updating documentation if needed
4. **Validate changes** by:
   - Running full test suite
   - Verifying build with zero warnings
   - Checking code formatting
   - Generating coverage reports
5. **Update the PR** by:
   - Committing changes with descriptive messages
   - Pushing updates to the PR branch
   - Adding explanatory comments for any suggestions not implemented
   - Responding to reviewer feedback appropriately

## Usage Examples:
- `/review-pr` - Reviews the most recent PR
- `/review-pr 12` - Reviews PR #12 specifically

## Process:
The command follows these steps systematically:
1. Use `gh pr list` and `gh pr view` to get PR details and feedback
2. Use `gh pr checks` to analyze CI/CD status  
3. Parse comments and suggestions from the PR conversation
4. Implement code changes using appropriate tools (Edit/MultiEdit/Write)
5. Run verification commands (build, test, format)
6. Commit and push changes with proper messages
7. Add PR comments explaining decisions where needed

$ARGUMENTS