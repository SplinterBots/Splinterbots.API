namespace Functional.SplinterBots.BattleAPI

open Functional.SplinterBots.API
open Functional.SplinterBots.API.CardTypes
  
type Team (summoner: Card, team: Card seq) =
    let secret = Randomizer.generateRandomString 10
    let getMonstersIdList () = 
        team
        |> Seq.map (fun monster -> monster.card_long_id)
        |> String.concat ","
    let teamHash =
        let toHash = 
            sprintf "%s,%s,%s"
                summoner.card_long_id
                (getMonstersIdList ())
                secret
        generateMD5Hash toHash

    member this.Summoner 
        with get () = summoner
    member this.Team 
        with get () = team
    member this.Secret 
        with get () = secret
    member this.TeamHash
        with get () = teamHash
