namespace Functional.SplinterBots.BattleAPI

open System
open Newtonsoft.Json

module OutstandingMatchTypes = 
    type OutstandingMatch = 
        {
            id: string
        }
module OutstandingMatch =

    open OutstandingMatchTypes
    open Functional.SplinterBots.API
    
    let getOutstandingMatch username =
        
        let url = Urls.getPlayerUri "outstanding_match" $"?username={username}"
        let response = Http.executeApiCall<OutstandingMatch option>
        ()
