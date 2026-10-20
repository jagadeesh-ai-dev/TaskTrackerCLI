# TaskTrackerCLI

A command-line task manager built with **C# and .NET 8**. Track tasks from creation through completion using interactive or direct commands, with data persisted in JSON.

## Features

- Create, update, and delete tasks
- Track `todo`, `in-progress`, and `done` statuses
- List tasks by status
- Search by keyword and optionally filter by status
- Sort by creation date or last update
- View task details by ID
- Interactive command mode with command history
- JSON-based local persistence

## Tech Stack

- C#
- .NET 8
- System.Text.Json
- JSON file storage

## Getting Started

### Prerequisites

Install the .NET 8 SDK.

### Run locally

```bash
git clone https://github.com/jagadeesh-ai-dev/TaskTrackerCLI.git
cd TaskTrackerCLI
dotnet run
```

To run a direct command, for example:

```bash
dotnet run add "Buy groceries"
```

## Command Reference

| Action | Example |
| --- | --- |
| Add a task | `add "Buy groceries"` |
| Update a task | `update 1 "Learn .NET 8"` |
| Delete with confirmation | `delete 1` |
| Delete without confirmation | `delete 1 --force` |
| Change status | `mark-in-progress 1` |
| Complete a task | `mark-done 1` |
| List all tasks | `list` |
| Filter by status | `list todo` |
| Sort newest first | `list newest` |
| Sort by recent update | `list recent` |
| Search tasks | `search "groceries"` |
| View task details | `view 1` |
| Show help | `help` |
| Clear the terminal | `clear` |
| Exit | `exit` |

Run `help` in the application for the supported command syntax.

## Design Notes

The project separates command handling, task models, service logic, and validation helpers. Task data is stored locally as JSON, so no database server is required.

## Roadmap

- Add automated unit tests
- Improve service-level error handling and result reporting
- Evaluate dependency injection and SQLite persistence

## Project Background

This project is also listed in the [roadmap.sh Task Tracker project](https://roadmap.sh/projects/task-tracker).

**Author:** [Jagadeesh](https://github.com/jagadeesh-ai-dev)
