namespace TodoComponent

open System

type TodoId = TodoId of Guid

module TodoId =
    let unwrap (TodoId id) = id

type Todo = {
    Id: TodoId
    Description: string
    Completed: bool
}

module Todo =
    let create description = { 
        Id = TodoId (Guid.NewGuid()); 
        Description = description; 
        Completed = false 
    }

    let toggleCompleted todo = 
        { todo with Completed = not todo.Completed }

type State = {
    Todos: Todo list
    NewTodoDescription: string
}

type Msg =
    | NewTodoChanged of string 
    | NewTodoCreateRequested
    | DeleteTodo of TodoId
    | ToggleTodoCompleted of TodoId


module State =
    open Elmish

    let private withNewTodo description state = 
        let newTodo = { 
            Id = TodoId (Guid.NewGuid()); 
            Description = description; 
            Completed = false 
        }
        
        { state with 
            Todos = state.Todos @ [ newTodo ]; 
            NewTodoDescription = "" 
        }

    let private withNewTodoDescription description state = 
        { state with NewTodoDescription = description }

    let private withTodoCompletedToggle id state = 
        let mapToToggle toToggleId todo = 
            if todo.Id = toToggleId then 
                Todo.toggleCompleted todo
            else 
                todo

        let todos = state.Todos |> List.map (mapToToggle id)

        { state with Todos = todos }

    let private withoutTodo id state = 
        let todos = state.Todos |> List.filter (fun todo -> todo.Id <> id)

        { state with Todos = todos }

    let private withNoCommand state = 
        state, Cmd.none

    let init () = 
        { Todos = [ Todo.create "Buy Egg"; Todo.create "Do something" ]; NewTodoDescription = "" }, Cmd.none

    let update (msg: Msg) (state: State): State * Cmd<Msg> =
        match msg with
            | NewTodoChanged description -> 
                state 
                |> withNewTodoDescription description
                |> withNoCommand

            | NewTodoCreateRequested -> 
                state 
                |> withNewTodo state.NewTodoDescription
                |> withNoCommand

            | DeleteTodo id -> 
                state
                |> withoutTodo id
                |> withNoCommand

            | ToggleTodoCompleted id ->
                state
                |> withTodoCompletedToggle id
                |> withNoCommand

module View =
    open Feliz

    let private renderTitle text =
        Html.p [
            attr.classes [ "title" ]
            attr.text (sprintf "%s" text)
        ]

    let private renderNewTodoInput (currentnewTodo: string) (dispatch: Msg -> unit) =
        Html.div [
            attr.classes [ "field"; "has-addons" ]
            attr.children [
                Html.p [
                    attr.classes [ "control"; "is-expanded" ]
                    attr.children [
                        Html.input [ 
                            attr.classes [ "input"; "is-medium" ]
                            attr.valueOrDefault currentnewTodo
                            attr.onTextChange (fun text -> dispatch (NewTodoChanged text))
                            attr.onKeyUp (key.enter, (fun _ -> 
                                if String.IsNullOrWhiteSpace currentnewTodo |> not then
                                    dispatch NewTodoCreateRequested
                            ))
                        ]
                    ]
                ]

                Html.p [
                    attr.classes [ "control" ]
                    attr.children [
                        Html.button [
                            attr.classes [ "button"; "is-primary"; "is-medium" ]
                            attr.onClick (fun _ -> dispatch NewTodoCreateRequested)
                            attr.disabled (String.IsNullOrWhiteSpace currentnewTodo)
                            attr.children [
                                Html.i [ attr.classes [ "fas"; "fa-plus" ] ] 
                                Html.text "Add"
                            ]
                        ]
                    ]
                ]
            ]
        ]

    let private renderTodoList (todos: Todo list) (dispatch: Msg -> unit) =
        Html.ul [
            attr.children [ 
                for todo in todos do 
                    Html.div [
                        attr.key (todo.Id |> TodoId.unwrap |> string)
                        attr.classes [ "box" ]
                        attr.children [
                            Html.div [
                                attr.classes [ "columns"; "is-mobile" ]
                                attr.children [
                                    Html.div [
                                        attr.classes [ "column"; "is-11" ]
                                        attr.children [
                                            Html.p [
                                                match todo.Completed with 
                                                | true -> attr.style [ style.textDecoration.lineThrough ]
                                                | false -> attr.style [ style.textDecoration.none ]
                                                attr.classes [ "subtitle" ]
                                                attr.text todo.Description
                                            ]
                                        ]
                                    ]

                                    Html.div [
                                        attr.classes [ "column";  ]
                                        attr.children [
                                            Html.div [
                                                attr.classes [ "buttons"; ]
                                                attr.children [
                                                    Html.button [
                                                        attr.onClick (fun _ -> dispatch (ToggleTodoCompleted todo.Id))
                                                        match todo.Completed with
                                                        | true -> attr.classes [ "button"; "is-success" ]
                                                        | false -> attr.classes [ "button" ]
                                                        attr.children [
                                                            Html.i [ attr.classes [ "fas"; "fa-check" ] ]
                                                        ]
                                                    ]

                                                    Html.button [
                                                        attr.classes [ "button"; "is-danger" ]
                                                        attr.onClick (fun _ -> dispatch (DeleteTodo todo.Id))
                                                        attr.children [
                                                            Html.i [ attr.classes [ "fas"; "fa-trash" ] ]
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
            ]
        ]

    let render (state: State) (dispatch: Msg -> unit) =
        Html.div [
            attr.style [ style.padding 20 ]
            attr.children [
                renderTitle "Elmish To-do Ap"
                renderNewTodoInput state.NewTodoDescription dispatch
                renderTodoList state.Todos dispatch
            ]
        ]
