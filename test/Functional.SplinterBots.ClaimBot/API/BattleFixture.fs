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
    let ``Can get battle result`` () =
        async {
            FsHttp.initialiseMocking ()

            let username = "test_player"
            let! bronzeLeaderboard = API.Battle.getBatleDetails username

            bronzeLeaderboard
            |> Seq.length
            |> should equal 50
        }

    [<Fact>]
    let ``Be sure that winner is as expected `` () =
        async {
            FsHttp.initialiseMocking ()

            let username = "test_player"
            let! bronzeLeaderboard = API.Battle.getBatleDetails username

            let firstBattle = 
                bronzeLeaderboard
                |> Seq.item 0
            firstBattle.winner
            |> should equal username
        }
