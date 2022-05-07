namespace Functional.SplinterBots.BattleAPI

open Functional.SplinterBots.API

   
type Team (summoner: Cards.Card, team: Cards.Card list) =
    let teamHash = ""
    let secret = ""

    member this.GetHash () = 
        teamHash
    member this.GetSecret () = 
        secret
    member this.GetSummoner () =
        summoner.card_detail_id
    member this.GetTeam () = 
        team 
        |> Seq.map (fun m -> m.card_detail_id)
        |> Array.ofSeq
