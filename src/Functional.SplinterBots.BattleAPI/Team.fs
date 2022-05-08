namespace Functional.SplinterBots.BattleAPI

open Functional.SplinterBots.API

   
type Team (summoner: Cards.Card, team: Cards.Card seq) =
    let secret = API.generateRandomString 10
    member this.Summoner 
        with get () = summoner
    member this.Team 
        with get () = team
    member this.Secret 
        with get () = secret
