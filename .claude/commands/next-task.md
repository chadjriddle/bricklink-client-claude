---
description: Automatically determines and continues with the next pending task from the project's task list in docs/tasks.md
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
  - Task
argument-hint: "(no arguments required)"
---

# Continue Next Task

This command will:

1. **Analyze Task List** by:
   - Reading `docs/tasks.md` to understand current project status
   - Identifying completed vs pending tasks across all milestones
   - Determining the next logical task in the sequence
2. **Set Up Task Tracking** by:
   - Creating comprehensive TodoWrite list for the identified task
   - Breaking down complex tasks into manageable subtasks
   - Setting up proper task status tracking
3. **Create Feature Branch** by:
   - Following project naming conventions (`feature/milestone-X-task-name`)
   - Ensuring clean working directory before starting
   - Creating branch from latest main
4. **Implement Task** by:
   - Following Just-In-Time interface creation principles
   - Implementing with comprehensive unit tests (85%+ coverage, 90%+ for auth)
   - Adhering to zero-warning build policy
5. **Complete Workflow** by:
   - Running full test suite with coverage reports
   - Verifying formatting and build requirements
   - Updating task documentation
   - Creating PR following established templates

## Usage Examples:
- `/next-task` - Finds and begins the next pending task

## Process:
The command follows these steps systematically:
1. Use Read tool to analyze `docs/tasks.md` for current status
2. Identify the first uncompleted task in milestone order
3. Use TodoWrite to create structured task tracking
4. Use Bash to create feature branch following naming conventions
5. Implement task using appropriate tools (Edit/MultiEdit/Write)
6. Run comprehensive testing and validation
7. Update task documentation and create PR

$ARGUMENTS