open Suave.Web
open Suave.Combinator
open Suave.Template
open Suave.Html
open System
open System.Text.RegularExpressions
open Taggen.Core
open Taggen.HtmlHelpers
open Taggen.Punctuation
open Taggen.Utils
open System.Net.Mail
open System.Configuration
open System.IO

  
let (|Match|_|) (pat:string) (inp:string) =
    let m = Regex.Match(inp, pat) in
    if m.Success
    then Some (List.tail [ for g in m.Groups -> g.Value ])
    else None

let (|Match2|_|) (pat:string) (inp:string) =
    match (|Match|_|) pat inp with
    | Some (fst :: snd :: []) -> Some (fst, snd)
    | Some [] -> failwith "Match2 succeeded, but no groups found. Use '(.*)' to capture groups"
    | Some _ -> failwith "Match2 succeeded, but did not find exactly two matches."
    | None -> None  

let urlSplitter url =
    match url with
    | Match2 "/(.*)/(.*)" (emailName, address) -> Some(emailName, address)
    | _ -> None

let niceResponse name address =
    html
    +<> (head +<> title % (sprintf "You asked for email: %s" name))
    +<> (body
        +<> (ol
            +<> (li +<> Text name)
            +<> (li +<> Text address)
            )
        )

let sendEmail name address =
    use smtpClient =
        new SmtpClient(ConfigurationManager.AppSettings.["SmtpServer"])
    smtpClient.EnableSsl <- false
    let mutable message = new MailMessage("michael.newton@15below.com", address)
    let body =
        if File.Exists (String.Concat [name;".html"]) then
            message.IsBodyHtml <- true
            File.ReadAllText(String.Concat [name;".html"])
        else
            name
    message.Body <- body
    message.Subject <- name
    smtpClient.Send(message)

let emailRequestHandler (hr : HttpRequest) =
    match urlSplitter hr.Url with
    | Some(name, address) -> (fun hr -> sendEmail name address; !(niceResponse name address) |> bytes) |> ok 
    | None -> (fun request -> sprintf "Url incorrect: %A" hr.Url |> bytes) |> failure

choose [
    Console.OpenStandardOutput() |> log >>= never
    meth0d "GET" >>= warbler(emailRequestHandler)
    ]
    |> web_server [|HTTP, "127.0.0.1",64537|] 
    |> Async.RunSynchronously
    |> ignore
