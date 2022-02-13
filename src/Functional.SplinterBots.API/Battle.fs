namespace Functional.SplinterBots.API

module Battle =

    open System.IO
    open System.Text
    open System.Text.Json
    open Functional.SplinterBots.API

     type BattleResult = 
        {
            winner: string
            ruleset: string
            details: string
        }

    type PlayerBatleStats = 
        {
            player: string
            battles: BattleResult array
        }

    type Position = 
        | Front
        | Second
        | Third
        | Support1
        | Support2
        | Support3
        | Avangard
    type Team = 
        {
            player: string
            color: string
            summoner: Cards.Card
            monsters: Cards.Card list
        }

    type BattleDetails = 
        {
            winningTeam: Team
            loosingTeam: Team
        }
    module BattleDetails =
        let bind (details: string) = 
            async {
                use detailsStream = new MemoryStream(Encoding.UTF8.GetBytes(details))
                let! battleData = JsonSerializer.DeserializeAsync<{|team1: Team; team2: Team; winner: string; loser: string|}>(detailsStream).AsTask() |> Async.AwaitTask   
                
                return
                    if (isNull <|  box battleData.team1) || (isNull <|  box battleData.team2)
                    then
                        None
                    else 
                        match battleData.team1.player with 
                        | x when x = battleData.winner -> 
                            Some { winningTeam = battleData.team1; loosingTeam = battleData.team2; }
                        | _ -> 
                            Some { winningTeam = battleData.team2; loosingTeam = battleData.team1; }
            }

    let getBattle userName =
        async {
            let uri = 
                getBattleApiUri "history" $"player={userName}"
            let! battleResult = executeApiCall<PlayerBatleStats> uri

            return battleResult.battles
        }

