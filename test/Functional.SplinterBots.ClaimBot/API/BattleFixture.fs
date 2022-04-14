namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader
open Functional.SplinterBots.API.Battle

module BattleFixture =

    [<Fact>]
    let ``Should compile all monsters properly`` () =
        async {
            FsHttp.initialiseMocking ()
            
            Cards.cardsList
            |> Seq.length
            |> should equal 450
        }
