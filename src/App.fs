module App

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.Helpers.React
open Fable.Helpers.React.Props

type Msg =
  | HomeMsg of Home.Msg

type Model = {
    home: Home.Model
  }

let init result =
  let home, cmd = Home.init()
  let model = { home = home }
  let cmd = cmd |> List.map (fun f -> fun d -> f (HomeMsg >> d))
  model, cmd

let update msg model =
  match msg with
  | HomeMsg msg ->
      let (home, homeCmd) = Home.update msg model.home
      { model with home = home }, Cmd.map HomeMsg homeCmd

let root model dispatch =
  div [ClassName "app-container"] [
    Home.view model.home dispatch
  ]

open Elmish.HMR
open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update root
|> Program.withHMR // Add the HMR support
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
