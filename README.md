# 🔗 Connect 4

Connect 4 (Unity) is a structured implementation of the classic Connect 4 game, focusing on clean architecture and testable core logic.
Developed in Unity (C#), it separates pure game rules from the Unity layer, featuring configurable board settings, animated 3D visuals, undo support, and robust win detection across multiple directions.

---

## ⚙️ Tech Overview

| Area           | Implementation                                                                 |
|----------------|-------------------------------------------------------------------------------|
| **Engine**         | Unity 6000.2.6f2                                                              |
| **Language**       | C#                                                                            |
| **Core Systems**   | Board state, move handling, undo, win detection, turn management             |
| **Tools**          | Scriptable configuration, centralized logging |

---

## 🧩 Core Features

- Local multiplayer Connect 4
- Configurable:
  - Board size (rows, columns)
  - Connect length (e.g. 4 in a row, 5 in a row, etc.)
- Undo last move
- Win detection (horizontal, vertical, and both diagonals)
- Main menu (New Game / Load Game / Quit)
- Animated falling discs & highlighted winning line

Core game rules (board state, moves, win checks) are implemented as pure C# logic. Unity components handle visuals, input, and scene management.

---

## 🧭 How to Run

1. Open the project in **Unity 6000.2.6f2**.
2. In **Build Settings**, ensure these scenes are added:
   - `StartScene` (index 0)
   - `GameScene` (index 1)
3. Open `StartScene` and press **Play**.

### Controls

**StartScene**

- **New Game** – start a fresh game in `GameScene`.
- **Load Game** – resume an in-memory session if one exists.
- **Quit** – exit the application.

**GameScene**

- Click a column to drop a disc.
- Use UI buttons to:
  - Undo last move  
  - Restart game  
  - Quit to menu  

---

## 💻 Configuration

Game rules are configured via a `GameConfigurator` ScriptableObject assigned to `GameManager`:

- `rows` – board rows  
- `columns` – board columns  
- `connectLength` – required streak length to win  
- `playerCount` – number of players (used for turn rotation)  

These values are tweakable in the Inspector; the board, win condition, and interactions adapt accordingly.

---

## 🧱 Architecture Overview

### Core Logic (C#)

- `BoardState` – grid, moves, undo, full-board checks  
- `BoardPosition`, `MoveResult`, `WinResult` – small value types for passing around state  
- `WinChecker` – last-move-based win detection in four directions  

These classes don’t depend on Unity APIs and are designed to be testable independently.

### Unity Layer

- `GameManager` – singleton that persists across scenes; owns `BoardState`, current player, move history, and scene transitions  
- `GameController` – handles a single game in `GameScene` (input, turns, undo, win/draw handling)  
- `BoardView` / `BoardHighlighter` – board rendering, disc spawning/animation, winning-line highlight  
- `UIController`, `ClickHandler`, `Spinner` – menu & in-game UI wiring, column click handling, simple menu visuals  
- `GameLogger` – central debug logger with a global enable/disable flag  

---

## 📜 Logging

All non-trivial logs go through `GameLogger`:

- `GameLogger.IsEnabled` can be toggled via `GameManager`’s `enableLogging` field.
- Provides `Log`, `LogWarning`, and `LogError` wrappers around `Debug.Log*`.

---
