module Util

[<RequireQualifiedAccess>]
module Literals =
    let [<Literal>] tile1 = "tile1"
    let [<Literal>] tile2 = "tile2"
    let [<Literal>] URL_texts_en  = "/data/texts_en.yaml"

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React.Props

type [<Pojo>] InnerHtml =
  { __html: string }

let parseMarkdown (markdown: string): string = importDefault "marked"

let setInnerHtml (html: string) =
  DangerouslySetInnerHTML { __html = html }

let inline (~%) x = createObj x
let inline (|Cast|) x = unbox<'T> x
