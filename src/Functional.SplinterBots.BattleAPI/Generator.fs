namespace Functional.SplinterBots.BattleAPI

open System
open Newtonsoft.Json

module Generator =
    let generateRandomString numebrOfCharacters = 
        let randomizer = Random()
        let chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray()
        let sz = Array.length chars in
        String(Array.init numebrOfCharacters (fun _ -> chars.[randomizer.Next sz])).ToString()
        
    let getStringForSplinterlandsAPI (transaction: HiveAPI.CHived.CtransactionData ) =
        let json = JsonConvert.SerializeObject transaction.tx
        let postData = 
            "signed_tx=" + json.Replace("operations\":[{", "operations\":[[\"custom_json\",{").Replace(",\"opid\":18}", "}]");
        postData


