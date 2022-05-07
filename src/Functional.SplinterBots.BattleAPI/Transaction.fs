namespace Functional.SplinterBots.BattleAPI

open Newtonsoft.Json.Linq

type Transaction = 
    {
        id: string
        success: bool
    }
module Transaction = 
    let bind (token: JToken) =
        {
            id = token.["id"].ToString()
            success = token.["success"].ToObject<bool>()
        }
