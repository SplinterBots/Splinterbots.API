namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader

module LeaderboardFixture =

    [<Fact>]
    let ``Can get leaderboard result`` () =
        async {
            FsHttp.initialiseMocking ()
            let! bronzeLeaderboard = API.Leaderboard.getLeaderboard Leaderboard.Leauge.Bronze

            bronzeLeaderboard.Length 
            |> should equal 100
        }


        
    [<Fact>]
    let ``Top player is correct`` () =
        async {
            FsHttp.initialiseMocking ()
            let! bronzeLeaderboard = API.Leaderboard.getLeaderboard Leaderboard.Leauge.Bronze
            let topPlayer = bronzeLeaderboard.[0]

            topPlayer.player 
            |> should equal "unibronze"

            topPlayer.rank
            |> should equal 1
        }

