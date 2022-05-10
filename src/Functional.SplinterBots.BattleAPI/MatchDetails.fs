namespace Functional.SplinterBots.BattleAPI

open Newtonsoft.Json.Linq
open Functional.SplinterBots.API

type MatchDetails =
    {
        mana_cap: int
        rulesets: string 
        allowedSplinters: string[]
        gameIdPlayer: string 
        opponentLookupName: string 
        gameHashId: string
    }

module MatchDetails =
    let bind (matchDetails: JToken) =
        let gameIdPlayer = matchDetails["id"].ToString()
        let opponentLookupName = matchDetails.["opponent"].["lookup_name"].ToString()
        {
           mana_cap = matchDetails.["mana_cap"].ToObject<int>()
           rulesets = matchDetails.["ruleset"].ToString()
           allowedSplinters = matchDetails.["inactive"].ToString() |> Splinters.getAllowedSplinters
           gameIdPlayer = gameIdPlayer
           opponentLookupName = opponentLookupName
           gameHashId = sprintf "%s/%s" (MD5.generateMD5Hash gameIdPlayer) (MD5.generateMD5Hash opponentLookupName)
        }
