# Elmish + Fable + Shadcn/ui Copilot Instructions

## Architecture Overview
F# Elmish application compiling to JavaScript via Fable, using React + shadcn/ui. Follows **MVU (Model-View-Update)** pattern with functional state management.

## Critical Workflow Commands
- **NEVER use `dotnet run`** - this will fail with Fable binding errors
- **Start dev**: `npm start` (runs Fable watch + Vite on port 8080)
- **Install deps**: `npm install --legacy-peer-deps` (React 19 compatibility required)
- **Add UI components**: `npx shadcn@latest add [component-name]`

## F# Component Structure Pattern
Every Elmish component in `src/` follows this exact structure (see `TodoComponent.fs`):

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

// 4. State module with private helper functions + update
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

## Key Domain Patterns
- **Wrapper types**: Use single-case unions like `TodoId of Guid` with unwrap functions
- **State updates**: Always use pipeline `state |> withChange |> withNoCommand`
- **Private helpers**: Break down complex state changes into `private with*` functions
- **Message dispatch**: Every view function takes `(Messages -> unit)` dispatch parameter

## Feliz + Shadcn Integration
```fsharp
// Standard HTML with Feliz
Html.div [
    prop.classes [ "bulma-class" ]  // Legacy Bulma CSS
    prop.children [ /* elements */ ]
]

// shadcn/ui components via Feliz.Shadcn
Shadcn.button [
    button.variant.secondary  // Type-safe variants
    prop.onClick (fun _ -> dispatch SomeMessage)
    prop.children [ Html.text "Click" ]
]
```

## File System & Build Chain
- **F# → JS**: Fable compiles `*.fs` to `*.fs.js` automatically
- **Entry point**: `src/App.fs.js` referenced in `index.html`
- **Vite config**: Ignores `.fs` files (Fable handles F# compilation)
- **Component structure**: F# logic in `src/`, shadcn UI components in `src/components/ui/`

## Styling Stack Integration
- **Bulma CSS**: Legacy styling loaded in `index.html`
- **Tailwind v4**: Modern utilities via `@tailwindcss/vite`
- **Font Awesome**: Icons via CDN in `index.html`
- **Utility function**: `@/lib/utils.ts` exports `cn()` for className merging

## Dependency Management Gotchas
- **Dual package managers**: Paket for .NET deps, npm for JS deps
- **React compatibility**: Always use `--legacy-peer-deps` flag
- **shadcn package**: Use `shadcn@latest`, NOT deprecated `shadcn-ui`
- **F# compilation order**: Dependencies must come first in `.fsproj`
