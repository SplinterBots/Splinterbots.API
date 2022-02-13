module UnionTools

open Microsoft.FSharp.Reflection
open System.Text.Json.Serialization

let toString (x:'a) = 
    let (case, _ ) = FSharpValue.GetUnionFields(x, typeof<'a>)
    case.Name

let optionsAsString<'a> () =
    FSharpType.GetUnionCases(typedefof<'a>)
    |> Seq.map (fun info -> info.Name)

let fromString<'a> defaultValue (s:string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
    |[|case|] -> FSharpValue.MakeUnion(case,[||]) :?> 'a
    |_ -> defaultValue

let (=~) discrminativeUnion actualString =
    let unionAsString = toString discrminativeUnion
    let areEqual = actualString  = unionAsString
    areEqual
