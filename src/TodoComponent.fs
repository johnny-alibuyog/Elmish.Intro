namespace TodoComponent

open System

[<AutoOpen>]
module Models =
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


type Messages =
    | NewTodoChanged of string 
    | NewTodoCreateRequested
    | DeleteTodo of TodoId
    | ToggleTodoCompleted of TodoId

type State = {
    Todos: Todo list
    NewTodoDescription: string
}

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

    let update (msg: Messages) (state: State): State * Cmd<Messages> =
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
    open Feliz.Shadcn

    let private renderTitle text =
        Html.p [
            prop.classes [ "title" ]
            prop.text (sprintf "%s" text)
        ]

    let private renderNewTodoInput (currentnewTodo: string) (dispatch: Messages -> unit) =
        Html.div [
            prop.classes [ "field"; "has-addons" ]
            prop.children [
                Html.p [
                    prop.classes [ "control"; "is-expanded" ]
                    prop.children [
                        Html.input [ 
                            prop.classes [ "input"; "is-medium" ]
                            prop.valueOrDefault currentnewTodo
                            prop.onTextChange (fun text -> dispatch (NewTodoChanged text))
                            prop.onKeyUp (key.enter, (fun _ -> 
                                if String.IsNullOrWhiteSpace currentnewTodo |> not then
                                    dispatch NewTodoCreateRequested
                            ))
                        ]
                    ]
                ]

                Html.p [
                    prop.classes [ "control" ]
                    prop.children [
                        Shadcn.button [
                            prop.classes [ "button"; "is-primary"; "is-medium" ]
                            prop.onClick (fun _ -> dispatch NewTodoCreateRequested)
                            prop.disabled (String.IsNullOrWhiteSpace currentnewTodo)
                            prop.children [
                                Html.i [ prop.classes [ "fas"; "fa-plus" ] ] 
                                Html.text "Add"
                            ]
                        ]
                    ]
                ]
            ]
        ]

    let private renderTodoList (todos: Todo list) (dispatch: Messages -> unit) =
        Html.ul [
            prop.children [ 
                for todo in todos do 
                    Html.div [
                        prop.key (todo.Id |> TodoId.unwrap |> string)
                        prop.classes [ "box" ]
                        prop.children [
                            Html.div [
                                prop.classes [ "columns"; "is-mobile" ]
                                prop.children [
                                    Html.div [
                                        prop.classes [ "column"; "is-11" ]
                                        prop.children [
                                            Html.p [
                                                match todo.Completed with 
                                                | true -> prop.style [ style.textDecoration.lineThrough ]
                                                | false -> prop.style [ style.textDecoration.none ]
                                                prop.classes [ "subtitle" ]
                                                prop.text todo.Description
                                            ]
                                        ]
                                    ]

                                    Html.div [
                                        prop.classes [ "column";  ]
                                        prop.children [
                                            Html.div [
                                                prop.classes [ "buttons"; ]
                                                prop.children [
                                                    Html.button [
                                                        prop.onClick (fun _ -> dispatch (ToggleTodoCompleted todo.Id))
                                                        match todo.Completed with
                                                        | true -> prop.classes [ "button"; "is-success" ]
                                                        | false -> prop.classes [ "button" ]
                                                        prop.children [
                                                            Html.i [ prop.classes [ "fas"; "fa-check" ] ]
                                                        ]
                                                    ]

                                                    Html.button [
                                                        prop.classes [ "button"; "is-danger" ]
                                                        prop.onClick (fun _ -> dispatch (DeleteTodo todo.Id))
                                                        prop.children [
                                                            Html.i [ prop.classes [ "fas"; "fa-trash" ] ]
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

    let render (state: State) (dispatch: Messages -> unit) =
        Html.div [
            prop.style [ style.padding 20 ]
            prop.children [
                renderTitle "Elmish To-do Ap"
                renderNewTodoInput state.NewTodoDescription dispatch
                renderTodoList state.Todos dispatch
            ]
        ]
        // Html.div [
        //     Shadcn.button [
        //         button.variant.secondary
        //         prop.text "Click me"
        //         prop.onClick (fun _ -> Browser.Dom.window.alert "Hello, shadcn/ui!")
        //     ]
        // ]


    // let renderAlert () =
    //     Shadcn.accordion [
    //         accordion.type'.single
    //         prop.children [
    //             Shadcn.accordionItem [
    //                 prop.value "item-1"
    //                 prop.children [
    //                     Shadcn.accordionTrigger "Is it accessible?"
    //                     Shadcn.accordionContent "Yes. It adheres to the WAI-ARIA design pattern."
    //                 ]
    //             ]
    //             Shadcn.accordionItem [
    //                 prop.value "item-2"
    //                 prop.children [
    //                     Shadcn.accordionTrigger "Is it styled?"
    //                     Shadcn.accordionContent
    //                         "Yes. It comes with default styles that matches the other components & aesthetic.."
    //                 ]
    //             ]
    //         ]
    //     ]

    // let renderAlertDialog () =
    //     Shadcn.alertDialog [
    //         Shadcn.alertDialogTrigger [
    //             alertDialogTrigger.asChild
    //             prop.children [ Shadcn.button [ button.variant.outline; prop.text "Show Dialog" ] ]
    //         ]
    //         Shadcn.alertDialogContent [
    //             Shadcn.alertDialogHeader [
    //                 Shadcn.alertDialogTitle "Are you absolutely sure?"
    //                 Shadcn.alertDialogDescription
    //                     "This action cannot be undone. This will permanently delete your account and remove your data from our servers."
    //             ]
    //             Shadcn.alertDialogFooter [ Shadcn.alertDialogCancel "Cancel"; Shadcn.alertDialogAction "Continue" ]
    //         ]
    //     ]

    // let renderButton () =
    //     Html.div [
    //         Html.h2 "Button"
    //         Shadcn.button [ 
    //             button.variant.outline
    //             prop.text "Button" 
    //         ]
    //     ]

    // let render (state: State) (dispatch: Msg -> unit) =
    //     Html.div [
    //         prop.children [
    //             renderTitle "Elmish To-do App"
    //             renderAlert ()
    //             renderAlertDialog ()
    //             renderButton ()
    //         ]
    //     ]