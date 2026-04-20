# 🧾 Task Tracker CLI (.NET 8)

A simple yet powerful Command Line Interface (CLI) application to manage tasks efficiently.

Track what you need to do, what you're working on, and what you've completed — all from your terminal.

---

## 🚀 Features

- ✅ Add, update, and delete tasks
- 🔄 Mark tasks as todo, in-progress, or done
- 📋 List tasks with filters and sorting
- 🔍 Search tasks by keyword and status
- 📄 View full task details
- ⌨️ Interactive mode with command history (↑ ↓)
- 💾 Persistent storage using JSON

---

## 🛠️ Tech Stack

- .NET 8
- C#
- System.Text.Json
- File-based storage (JSON)

---

## 📁 Project Structure


TaskTrackerCLI/
  Commands/ # Handles CLI commands & output formatting
  Models/ # Task data model
  Services/ # Business logic & file handling
  Utils/ # Validation helpers
  Program.cs # Entry point (interactive + direct mode)
  tasks.json # Data storage (auto-created)
  README.md # Instructions and about 


---

## ▶️ How to Run

### 1. Clone the repository

git clone https://github.com/jagadeesh-ai-dev/TaskTrackerCLI.git
cd TaskTrackerCLI


### 2. Run the application

#### 🔹 Interactive Mode

dotnet run


#### 🔹 Direct Command Mode

dotnet run add "Buy groceries"


---

## 📌 Available Commands

### ➕ Create & Update

add "task description"          => Write the description in "" for multiple words
add task						=> Single word description
update <id> "new description"   => Update existing task description


---

### ❌ Delete

delete <id>					 => Delete task with confirmation
delete <id> --force			 => Delete without confirmation


---

### 🔄 Status

mark-in-progress <id>		=> Mark task as in-progress
mark-done <id>              => Mark task as done


---

### 📋 List Tasks

list                       => Get all tasks
list todo                  => Get tasks with status "todo"
list done                  => Get tasks with status "done"
list in-progress           => Get tasks with status "in-progress"



---

### 🔃 Sorting

list newest				   => Get tasks sorted by creation date (newest first)
list oldest				   => Get tasks sorted by creation date (oldest first)
list recent				   => Get tasks sorted by last updated (most recent first)
list stale				   => Get tasks sorted by last updated (least recent first)


---

### 🔍 Search

search "keyword" 				=> Search all tasks by keyword(from description or status)
search "keyword" done			=> Search "keyword" in done tasks
search "keyword" in-progress    => Search "keyword" in in-progress tasks
search "keyword" todo           => Search "keyword" in todo tasks


---

### 📄 View Details

view <id>						=> View full details of a task by ID


---

### ⚙️ Utility

help							=> Show available commands and usage
clear # clear screen            => Clear the terminal screen
exit # exit app                 => Exit the application


---

## ⌨️ Interactive Mode Features

- ↑ / ↓ → Navigate command history	=> Use up/down arrow keys to cycle through previously entered commands

- Supports quoted inputs:

add "Learn .NET properly"

## Few Example Commands
update 1 "Learn .NET 8 features"
delete 2 --force
delete 3
mark-in-progress 4
mark-done 5
search "groceries"
list in-progress
list todo
list done

---

## 💾 Data Storage

- Tasks are stored in a JSON file:

bin/Debug/net8.0/tasks.json


- Automatically created on first run

---

## 📊 Task Structure

Each task contains:

```json
{
  "id": 1,
  "description": "Buy groceries",
  "status": "todo",
  "createdAt": "2026-04-19T08:31:00Z",
  "updatedAt": null
}

⚠️ Notes

-> IDs must be positive integers
-> Task descriptions must be unique (case-insensitive)
-> Status values:
	todo
	in-progress
	done

🧠 Design Highlights

-> Clean separation of concerns:
	Command handling
	Business logic
	File storage
-> Input normalization & validation
-> Duplicate prevention
-> Safe JSON handling

🚧 Future Improvements

🔹 Dependency Injection
🔹 Result pattern (remove Console from services)
🔹 SQLite database support
🔹 Advanced filtering & multi-argument parsing
🔹 Unit testing


👨‍💻 Author

   Jagadeesh

⭐ If you like this project

Give it a star ⭐ and share it!
