# /next-task

Automatically determines and continues with the next pending task from the project's task list.

## What this command does:

1. **Reads Task List**: Analyzes `docs/tasks.md` to understand current project status
2. **Identifies Next Task**: Finds the first uncompleted task in the milestone sequence
3. **Creates Todo List**: Sets up structured task tracking using TodoWrite
4. **Begins Implementation**: Starts working on the identified task following project conventions
5. **Follows Workflow**: Adheres to branch creation, testing, and PR requirements from CLAUDE.md

## Behavior:

- Automatically determines the next logical task based on completion status
- Creates feature branch following naming conventions
- Sets up comprehensive todo list for task tracking
- Implements the task with full test coverage
- Follows zero-warning build policy
- Generates coverage reports and ensures thresholds are met
- Updates task documentation upon completion

## Project Workflow Integration:

This command fully integrates with the established development workflow:
- Just-In-Time interface creation principles
- Atomic development with single responsibility
- Comprehensive testing with coverage requirements
- Professional CI/CD integration
- Mandatory PR workflow compliance

Use this command whenever you want to continue with the next task in the project's roadmap.