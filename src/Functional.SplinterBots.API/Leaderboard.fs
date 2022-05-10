namespace Functional.SplinterBots.API

module Leaderboard = 

    open System    
    
    type LeaderboardPlayer = 
        {
            rank : int
            player: string
        }

    type Leaderboard = LeaderboardPlayer array

    [<Flags>]
    type Leauge = 
        | Bronze = 0
        | Silver = 1
        | Gold = 2
        | Diamond = 3
        | Champion = 4

    let getLeaderboard (leauge: Leauge) = 
        async {
            let uri = 
                Urls.cacheApiUri "players/leaderboard" $"leaderboard={int(leauge)}"
            return! executeApiCall<Leaderboard> uri
        }
