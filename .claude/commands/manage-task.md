---
description: Create, modify, or update tasks in docs/tasks.md with full workflow management including branch handling, work status checks, and PR creation
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
argument-hint: "<task-description-or-update>"
---

# Manage Task

This command provides comprehensive task management for the project's task list in `docs/tasks.md`. It handles the complete workflow from initial work status checks through PR creation.

## Workflow Process:

### 1. **Pre-Task Checks** - Ensure clean working state:
   - Check current branch status and uncommitted changes
   - If work in progress, prompt user to:
     - Commit current work, OR
     - Stash changes for later
   - Refuse to proceed until working directory is clean

### 2. **Branch Management** - Prepare for task work:
   - Checkout main branch
   - Pull latest changes from remote
   - Create new task update branch with naming: `feature/task-update-YYYYMMDD`

### 3. **Task Management** - Work with user on task:
   - Read current `docs/tasks.md` to understand project status  
   - Based on user's argument, either:
     - **Create new task**: Add new task to appropriate milestone
     - **Modify existing task**: Update task description, status, or details
     - **Update task status**: Mark tasks as completed or change priority
   - Use TodoWrite to track the task management work
   - Collaborate with user until they're satisfied with changes

### 4. **Completion Workflow** - Finalize changes:
   - Commit changes with descriptive message
   - Push branch to remote
   - Create PR with proper template
   - Provide PR URL to user

## Usage Examples:
- `/manage-task "Add OAuth integration testing"` - Creates new task
- `/manage-task "Update authentication handler implementation status"` - Modifies existing task  
- `/manage-task "Mark milestone 3 tasks as completed"` - Updates task statuses

## Command Arguments:
- **task-description-or-update**: Description of the task to create/modify or update instruction

## Safety Features:
- **Work Status Validation**: Ensures no uncommitted work is lost
- **Branch Protection**: Creates isolated branch for task updates
- **User Confirmation**: Collaborates with user before making changes
- **Rollback Support**: Changes can be reverted if needed

## Process Flow:
1. Use `git status` to check for uncommitted work
2. If work exists, prompt user for commit/stash decision and wait for confirmation
3. Use `git checkout main && git pull origin main` to prepare main branch
4. Create feature branch for task updates
5. Use Read to analyze current `docs/tasks.md`
6. Work interactively with user to make desired task changes
7. Use Edit/MultiEdit to update `docs/tasks.md`
8. Use Bash commands to commit, push, and create PR
9. Provide completion summary with PR link

## Integration:
- Follows project's Just-In-Time Interface Creation principles
- Maintains task documentation standards
- Integrates with existing CI/CD pipeline
- Supports milestone-based project structure

$ARGUMENTS