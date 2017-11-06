[<RequireQualifiedAccess>]
module Home

open System
open Elmish
open Fable.PowerPack
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Elements
open Fulma.Layouts
open Util

let [<Literal>] CARD_HEADER_SIZE = 85
let [<Literal>] REACT_CARDSTACK = "../../paket-files/alfonsogarciacaro/react-cardstack/dist/index"

type RCom<'T> = Fable.Import.React.ComponentClass<'T>
type Model = Map<string, obj>

type IYaml =
  abstract safeLoad: string -> obj

type Msg =
  | ChangeTexts of Model

let CardStack: RCom<obj> = importMember REACT_CARDSTACK
let Card: RCom<obj> = importMember REACT_CARDSTACK
let YAML: IYaml = importAll "js-yaml"

let init () : Model * Cmd<Msg> =
  let fetchTexts dispatch =
    Fetch.fetch Literals.URL_texts_en []
    |> Promise.bind (fun res ->
      if res.Ok
      then res.text()
      else Promise.lift "{}")
    |> Promise.iter (fun x ->
      let y = YAML.safeLoad(x)
      inflate y |> ChangeTexts |> dispatch)
  Map.empty, [fetchTexts]

let update msg model : Model * Cmd<Msg> =
  match msg with
  | ChangeTexts texts ->
      texts, []

let splitFirstLine (txt: string) =
  let i = txt.IndexOf('\n')
  txt.[..i-1], txt.[i+1..]

let renderTitleAndBody header txt =
  let title, body = splitFirstLine txt
  let innerHtml = setInnerHtml (parseMarkdown body)
  div [] [
    header [] [str title]
    br []
    Content.content [Content.props [innerHtml]] []
  ]

let notificationRandomColor =
  let rnd = Random()
  fun () ->
    match rnd.Next(5) with
    | 0 -> Notification.isPrimary
    | 1 -> Notification.isInfo
    | 2 -> Notification.isSuccess
    | 3 -> Notification.isWarning
    | _ -> Notification.isDanger

let getText id (model: Model) =
  match Map.tryFind id model with
  | Some(Cast txt) ->
    let innerHtml = setInnerHtml (parseMarkdown txt)
    Content.content [
      Content.props [innerHtml]
    ] []
  | None -> div [] []

let tileChild id model =
  Tile.child [] [
    div [Style [Height "100%" ]] [
      getText id model
    ]
    // Notification.notification [
    //   notificationRandomColor ()
    //   Notification.props [Style [Height "100%" ]]
    // ] [getText id model]
  ]

let cardstack id model =
  match Map.tryFind ("card_"+id) model with
  | Some(Cast texts) ->
    let cards =
      texts |> Array.mapi (fun i txt ->
        let header = if i = 0 then Heading.h3 else Heading.h5
        from Card %["background" ==> "white"]
                  [div [ClassName (id + " card-" + string (i+1))
                        Style [Height "100%"; Padding "25px"]]
                    [renderTitleAndBody header txt]])
    div [ClassName "cardstack"] [
      from CardStack %["height"      ==> Array.length texts * CARD_HEADER_SIZE
                       "width"       ==> "100%"
                       "background"  ==> "#f8f8f8"
                       "hoverOffset" ==> 25]
                      (List.ofArray cards)
    ]
  | _ -> div [] []

let tileCardstack id model =
  Tile.child [] [
    cardstack id model
  ]

let tileImage src =
  Tile.child [] [
    Image.image [] [img [Src src]]
  ]

let tileNotification notificationType body =
  Tile.child [] [
    Notification.notification [
      notificationType
      Notification.props [Style [Height "100%"]]
    ] [body]
  ]

let tileClassed className body =
  Tile.child [] [
    div [ClassName className; Style [Height "100%"]] [body]
  ]

let view model dispatch =
  Tile.ancestor [] [
    Tile.parent [Tile.isVertical] [
      Tile.tile [] [
        Tile.parent [Tile.isVertical] [
          tileCardstack "presentation" model
          tileImage "/img/Alfonso.jpg"
        ]
        Tile.parent [Tile.isVertical] [
          tileClassed "introduction" (getText Literals.tile1 model)
          // flex-grow is necessary to adjust the tile to the cardstack height
          Tile.child [Tile.props [Style [FlexGrow 0.]]] [
            cardstack "demand" model
          ]
        ]
      ]
      Tile.parent [] [
        tileCardstack "supply" model
        tileClassed "quotation" (getText Literals.tile2 model)
      ]
    ]
  ]
