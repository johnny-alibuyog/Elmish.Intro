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

The application follows the **Model-View-Update** (MVU) pattern with F#-specific structural conventions:

```fsharp
// 1. Domain types in AutoOpen Models module
[<AutoOpen>]
module Models =
    type TodoId = TodoId of Guid
    module TodoId = let unwrap (TodoId id) = id
    type Todo = { Id: TodoId; Description: string; Completed: bool }

// 2. Messages union type
type Messages = NewTodoChanged of string | NewTodoCreateRequested | ...

// 3. State type  
type State = { Todos: Todo list; NewTodoDescription: string }

// 4. State module with private helper functions
module State =
    let private withSomeChange state = { state with SomeField = newValue }
    let private withNoCommand state = state, Cmd.none
    let update (msg: Messages) (state: State) = 
        match msg with
        | SomeMessage -> state |> withSomeChange |> withNoCommand

// 5. View module with private render functions
module View =
    let private renderSubComponent state dispatch = Html.div [...]
    let render (state: State) (dispatch: Messages -> unit) = Html.div [...]
```

### Key F# Patterns Used

- **Wrapper Types**: Single-case unions like `TodoId of Guid` with companion unwrap functions
- **Pipeline State Updates**: Always use `state |> withChange |> withNoCommand` pattern
- **Private Helper Functions**: Break complex state changes into `private with*` functions
- **AutoOpen Modules**: Domain types in `[<AutoOpen>] module Models` for clean namespacing

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

### F# Component Structure
Each Elmish component follows a consistent pattern (see `TodoComponent.fs`):

1. **Domain Models**: Types in `[<AutoOpen>] module Models` 
2. **Messages**: Union type for all possible actions
3. **State**: Application state record type
4. **State Module**: `init`, `update`, and private helper functions
5. **View Module**: `render` function and private UI helpers

### File Compilation Chain
- **F# → JavaScript**: Fable compiles `*.fs` to `*.fs.js` automatically
- **Entry Point**: `src/App.fs.js` referenced in `index.html`  
- **Build Process**: Vite watches JS output, ignores F# source files
- **Hot Reload**: Changes to F# files trigger Fable recompilation + browser update

### Styling Integration
- **Bulma CSS**: Legacy component styling (loaded in `index.html`)
- **Tailwind v4**: Modern utility classes via `@tailwindcss/vite`
- **Font Awesome**: Icons via CDN
- **shadcn/ui**: TypeScript components in `src/components/ui/` with F# bindings

### State Management Patterns
```fsharp
// Use pipeline composition with private helpers
let update msg state =
    match msg with
    | AddTodo description -> 
        state |> withNewTodo description |> withNoCommand
    | ToggleTodo id -> 
        state |> withTodoToggled id |> withNoCommand

// Private helper functions for clean state updates
let private withNewTodo description state = 
    { state with Todos = Todo.create description :: state.Todos }
```

## Dependency Management

### Dual Package Managers
- **Paket** (`paket.dependencies`): F# libraries (Elmish, Fable, Feliz)
- **npm** (`package.json`): JavaScript libraries (React, shadcn/ui, Vite)

### Critical Dependencies
**F# Stack:**
- `Elmish` - Core MVU framework
- `Fable.Elmish.HMR` - Hot module replacement
- `Feliz` - React DSL for F#
- `Feliz.Shadcn` - shadcn/ui F# bindings

**JavaScript Stack:**
- React 19 + ReactDOM
- shadcn/ui + Radix UI components  
- Tailwind CSS v4 + utilities
- Vite build tooling

### Installation Gotchas
```bash
# Always use --legacy-peer-deps for React 19 compatibility
npm install --legacy-peer-deps

# Use latest shadcn package (not deprecated shadcn-ui)
npx shadcn@latest add button

# F# compilation order matters in .fsproj files
```

## Building for Production

```bash
npm run build
```

## Common Troubleshooting

### Build Issues
- **`dotnet run` fails**: Use `npm start` instead - this is a Fable project, not standard .NET
- **React version conflicts**: Always install with `--legacy-peer-deps` flag
- **shadcn components missing**: Use `shadcn@latest`, not deprecated `shadcn-ui`

### Development Issues  
- **F# changes not reflecting**: Ensure Fable compilation completed (check terminal output)
- **TypeScript components not found**: Verify `@/` alias in `vite.config.ts`
- **Styling conflicts**: Check Bulma vs Tailwind class precedence

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
