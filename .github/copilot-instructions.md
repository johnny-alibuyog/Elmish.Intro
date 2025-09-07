# Elmish + Fable + Shadcn/ui Project Instructions

## Architecture Overview

This is an F# Elmish application that compiles to JavaScript via Fable, using React for rendering and shadcn/ui for components. The project follows the **MVU (Model-View-Update)** pattern with functional F# on the frontend.

### Key Technologies
- **Fable**: F# to JavaScript compiler
- **Elmish**: F# implementation of the Elm architecture (MVU pattern)
- **Feliz**: F# DSL for React components
- **Feliz.Shadcn**: F# bindings for shadcn/ui components
- **Vite**: Frontend build tool and dev server
- **Paket**: .NET dependency manager

## Project Structure

```
src/
├── App.fs              # Entry point - wires up the Elmish program
├── TodoComponent.fs    # Main application logic (Model-View-Update)
├── components/ui/      # shadcn/ui components (TypeScript)
└── lib/utils.ts        # Utility functions for shadcn/ui
```

## Development Workflow

### Essential Commands
- **Start dev server**: `npm start` (compiles F# via Fable + runs Vite)
- **Install .NET deps**: `dotnet paket install` or `dotnet restore`
- **Install JS deps**: `npm install --legacy-peer-deps` (required for React 19 compatibility)
- **Add shadcn component**: `npx shadcn@latest add [component-name]`

### Critical: Never use `dotnet run`
This project uses Fable compilation. Always use `npm start` which runs:
`dotnet fable watch --verbose --run npx vite --port 8080`

## F# Elmish Patterns

### MVU Component Structure (see `TodoComponent.fs`)
```fsharp
// 1. Types first - Domain model + Message types
type Todo = { Id: TodoId; Description: string; Completed: bool }
type State = { Todos: Todo list; NewTodoDescription: string }
type Msg = NewTodoChanged of string | NewTodoCreateRequested | ...

// 2. State module with init and update functions
module State =
    let init () = initialState, Cmd.none
    let update (msg: Msg) (state: State): State * Cmd<Msg> = ...

// 3. View module with render function
module View =
    let render (state: State) (dispatch: Msg -> unit) = ...
```

### State Updates Pattern
Use pipeline composition with helper functions:
```fsharp
let update msg state =
    match msg with
    | SomeAction -> 
        state 
        |> withSomeChange
        |> withNoCommand
```

### Feliz View Patterns
```fsharp
// Use Feliz for React components
Html.div [
    prop.classes [ "class-name" ]
    prop.children [ /* child elements */ ]
]

// For shadcn/ui components
Shadcn.button [
    button.variant.secondary
    prop.text "Click me"
    prop.onClick (fun _ -> dispatch SomeMessage)
]
```

## Dependency Management

### F# Dependencies (`paket.dependencies`)
- **Elmish**: Core MVU framework
- **Fable.Elmish.HMR**: Hot module replacement
- **Feliz**: React DSL for F#
- **Feliz.Shadcn**: shadcn/ui bindings

### JavaScript Dependencies
- React 19 (requires `--legacy-peer-deps` for compatibility)
- shadcn/ui + Radix components for UI
- Tailwind CSS v4 for styling

## Integration Points

### F# ↔ JavaScript
- Fable compiles F# to `.fs.js` files automatically
- Entry point: `src/App.fs.js` referenced in `index.html`
- Vite watches JS output, ignores F# source files (configured in `vite.config.ts`)

### shadcn/ui Integration
- Components added via CLI land in `src/components/ui/` (TypeScript)
- F# bindings available through `Feliz.Shadcn`
- Utilities in `src/lib/utils.ts` for className merging

### Styling Stack
- Bulma CSS for legacy styling (loaded in `index.html`)
- Tailwind CSS v4 for modern utility classes
- Font Awesome for icons

## Common Gotchas

1. **React version conflicts**: Always use `--legacy-peer-deps` when installing npm packages
2. **F# compilation order**: Files must be ordered correctly in `.fsproj` (dependencies first)
3. **shadcn package**: Use `shadcn@latest`, not deprecated `shadcn-ui`
4. **Vite config**: Configured to ignore `.fs` files since Fable handles F# compilation

## File Naming Conventions

- F# modules use PascalCase: `TodoComponent.fs`
- Generated JS files: `TodoComponent.fs.js`
- TypeScript components: kebab-case in `components/ui/`
