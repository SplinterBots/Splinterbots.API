namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader
open Functional.SplinterBots.API.Battle

module CardsFixture =

    [<Fact>]
    let ``Can get battle result`` () =
        async {
            FsHttp.initialiseMocking ()
            
            Cards.cardsList
            |> Seq.length
            |> should equal 450
        }
