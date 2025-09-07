# Elmish.Intro

A modern F# frontend application demonstrating the **Elmish MVU (Model-View-Update)** pattern with **Fable** compilation to JavaScript, **React** rendering, and **shadcn/ui** components.

## Architecture Overview

This project showcases a functional approach to frontend development using F# that compiles to JavaScript, combining the power of functional programming with modern web technologies.

### Technology Stack

- **[F#](https://fsharp.org/)**: Primary programming language
- **[Fable](https://fable.io/)**: F# to JavaScript compiler
- **[Elmish](https://elmish.github.io/elmish/)**: F# implementation of the Elm architecture (MVU pattern)
- **[Feliz](https://zaid-ajaj.github.io/Feliz/)**: F# DSL for React components
- **[Feliz.Shadcn](https://github.com/Dzoukr/Feliz.Shadcn)**: F# bindings for shadcn/ui components
- **[React 19](https://react.dev/)**: UI rendering framework
- **[Vite](https://vitejs.dev/)**: Fast development server and build tool
- **[shadcn/ui](https://ui.shadcn.com/)**: Modern component library built on Radix UI
- **[Tailwind CSS v4](https://tailwindcss.com/)**: Utility-first CSS framework
- **[Paket](https://fsprojects.github.io/Paket/)**: .NET dependency manager

### MVU Architecture Pattern

The application follows the **Model-View-Update** (MVU) pattern, which provides a predictable and functional approach to state management:

1. **Model**: Immutable state representation (`State` type)
2. **View**: Pure functions that render the UI based on state (`View.render`)
3. **Update**: Pure functions that handle messages and return new state (`State.update`)

```fsharp
// Domain types
type Todo = { Id: TodoId; Description: string; Completed: bool }
type State = { Todos: Todo list; NewTodoDescription: string }
type Msg = NewTodoChanged of string | NewTodoCreateRequested | ...

// MVU components
module State =
    let init () = initialState, Cmd.none
    let update (msg: Msg) (state: State) = // Returns new state

module View =
    let render (state: State) (dispatch: Msg -> unit) = // Pure UI function
```

### Project Structure

```
├── src/
│   ├── App.fs              # Application entry point & Elmish program setup
│   ├── TodoComponent.fs    # Main Todo component (Model-View-Update)
│   ├── components/ui/      # shadcn/ui components (TypeScript)
│   ├── hooks/              # React hooks (TypeScript)
│   └── lib/utils.ts        # Utility functions
├── fable_modules/          # Compiled Fable dependencies
├── paket.dependencies      # .NET package dependencies
├── package.json           # Node.js dependencies & scripts
├── components.json        # shadcn/ui configuration
├── vite.config.ts         # Vite bundler configuration
└── index.html            # HTML entry point
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS version recommended)

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Elmish.Intro
   ```

2. **Install .NET dependencies**
   ```bash
   dotnet paket install
   # or alternatively:
   dotnet restore
   ```

3. **Install JavaScript dependencies**
   ```bash
   npm install --legacy-peer-deps
   ```
   > **Note**: The `--legacy-peer-deps` flag is required due to React 19 compatibility with some shadcn/ui components.

### Running the Application

**Start the development server:**
```bash
npm start
```

This command:
- Compiles F# code to JavaScript using Fable in watch mode
- Starts the Vite development server on port 8080
- Enables hot module replacement for instant updates

The application will be available at: **http://localhost:8080/**

> **⚠️ Important**: Never use `dotnet run` for this project. Always use `npm start` as this is a Fable project that compiles to JavaScript, not a traditional .NET application.

### Adding shadcn/ui Components

To add new shadcn/ui components:

```bash
npx shadcn@latest add [component-name]
```

Example:
```bash
npx shadcn@latest add button
npx shadcn@latest add card
```

## Development Workflow

### File Compilation
- F# files (`.fs`) are compiled to JavaScript (`.fs.js`) automatically by Fable
- Vite watches the generated JavaScript files, not the F# source files
- Changes to F# files trigger recompilation and hot reload

### State Management Pattern
The application uses functional state updates with helper functions:

```fsharp
let update msg state =
    match msg with
    | SomeAction -> 
        state 
        |> withSomeChange
        |> withNoCommand
```

### Component Structure
Each Elmish component typically contains:
- **Types**: Domain models and message types
- **State module**: `init` and `update` functions
- **View module**: `render` function for UI

## Building for Production

```bash
npm run build
```

## Project Features

This Todo application demonstrates:
- ✅ Functional reactive programming with F#
- ✅ Type-safe state management
- ✅ Modern UI components with shadcn/ui
- ✅ Hot module replacement during development
- ✅ Integration between F# and JavaScript ecosystems

## Learn More

- [Elmish Documentation](https://elmish.github.io/elmish/)
- [Fable Documentation](https://fable.io/docs/)
- [Feliz Documentation](https://zaid-ajaj.github.io/Feliz/)
- [shadcn/ui Documentation](https://ui.shadcn.com/)
