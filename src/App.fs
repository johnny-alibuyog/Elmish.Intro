open Elmish
open Elmish.HMR
open TodoComponent

Program.mkProgram State.init State.update View.render
|> Program.withConsoleTrace
|> Program.withReactSynchronous "root"
|> Program.run


// ######################################################################################################
// open Browser

// let div = document.createElement "div"
// div.innerHTML <- "Hello world!"
// document.body.appendChild div |> ignore



// ######################################################################################################
// module App

// open Feliz

// [<ReactComponent>]
// let Counter() =
//     let (count, setCount) = React.useState(0)
//     Html.div [
//         Html.h1 count
//         Html.button [
//             prop.text "Increment"
//             prop.onClick (fun _ -> setCount(count + 1))
//         ]
//     ]

// open Browser.Dom

// ReactDOM
//     .createRoot(document.getElementById "root")
//     .render(Counter())