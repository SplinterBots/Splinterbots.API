namespace Functional.SplinterBots.BattleAPI

open System
open Newtonsoft.Json

module OutstandingMatchTypes = 
    type OutstandingMatch = 
        {
            id: string
            player: string 
            team_hash: string
        }

module OutstandingMatch =

    open OutstandingMatchTypes
    open Functional.SplinterBots.API
    
    let getOutstandingMatch username =
        async {
            let url = Urls.getPlayerUri "outstanding_match" $"?username={username}"
            let! response = Http.executeApiCall<OutstandingMatch option> url
            return response
        }
