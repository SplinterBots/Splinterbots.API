namespace Functional.SplinterBots.API

open HiveAPI
open System

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


    let getBattleHistory userName =
        async {
            let uri = 
                getBattleUri "history" $"player={userName}"
            let! battleResult = executeApiCall<PlayerBatleStats> uri

            return battleResult.battles
        }

    let getBatleDetails battleId = 
        async {
            let uri = 
                getBattleUri "result" $"id={battleId}"
            let! battleResult = executeApiCall<PlayerBatleStats> uri

            return battleResult.battles
        }

    type StartBattleInfo =
        {
            success: bool 
            id: string 
        }
        
    let private getStringForSplinterlandsAPI (transaction: CHived.CtransactionData) = 
        let json = Newtonsoft.Json.JsonConvert.SerializeObject(transaction.tx)
        let fixedJson = 
            json
                .Replace("operations\":[{", "operations\":[[\"custom_json\",{")
                .Replace(",\"opid\":18}", "}]")
        let postData = sprintf "signed_tx=%s" fixedJson
        postData

    let startNextMatch playerName postingKey =
        async {
            let transactionPayload = sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"
            let operations = API.createCustomJsonPostingKey playerName "sm_find_match" transactionPayload
            let transactionData = API.hive.create_transaction([| operations |] , [| postingKey |])
            let postData = getStringForSplinterlandsAPI transactionData
            let! battleData = API.executeApiPostCall<StartBattleInfo> API.battleUri postData
            return battleData
        }

    type OutstandingMatch = 
        {
            id: string 
            created_block_num: int 
            expiration_block_num: int 
            player: string 
            team_hash: string option  
            match_type: string 
            mana_cap: int 
            opponent: string 
            match_block_num: int 
            status: int 
            reveal_tx: obj 
            reveal_block_id: obj 
            team: obj 
            summoner_level: obj 
            ruleset: string 
            inactive: string 
            opponent_player: string 
            opponent_team_hash: string option  
            submit_expiration_block_num: int 
            settings: string 
            app: obj 
            created_date: DateTime 
            expiration_date: DateTime 
            match_date: DateTime 
            submit_expiration_date: DateTime 
            recent_opponents: string 
            is_critical: bool 
        }

    module OutstandingMatch = 
        let isTeamSubmited outstandingMatch =
            outstandingMatch.team_hash.IsSome

    let getOutstandingMatch playerName =
        async {
            let uri = 
                getPlayerUri "outstanding_match" $"username={playerName}"
            let! outstandingMatch = executeApiCall<OutstandingMatch option> uri

            return outstandingMatch
        }
