namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader

module SplinterFixture =

    [<Fact>]
    let ``Get all splinters`` () =
        let inactive = ""

        let result = Splinters.getAllowedSplinters inactive

        result |> should contain "fire"
        result |> should contain "water"
        result |> should contain "earth"
        result |> should contain "life"
        result |> should contain "death"
        result |> should contain "dragon"

    [<Fact>]
    let ``Can exclude one splinter`` () =
        let inactive = "green"

        let result = Splinters.getAllowedSplinters inactive

        result |> should not' (contain "earth")

        
    [<Fact>]
    let ``Can exclude multiple splinters`` () =
        let inactive = "red,green,gold"

        let result = Splinters.getAllowedSplinters inactive

        result |> should not' (contain "fire")
        result |> should not' (contain "earth")
        result |> should not' (contain "dragon")
           
    [<Fact>]
    let ``Can exclude all splinters`` () =
        let inactive = "red,green,blue,black,gold,white"

        let result = Splinters.getAllowedSplinters inactive

        result |> should not' (contain "fire")
        result |> should not' (contain "water")
        result |> should not' (contain "earth")
        result |> should not' (contain "life")
        result |> should not' (contain "death")
        result |> should not' (contain "dragon")
