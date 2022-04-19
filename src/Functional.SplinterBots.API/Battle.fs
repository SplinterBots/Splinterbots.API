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
            let transactionPayload = sprintf """{"match_type":"Ranked","app":"%s","n":"%s"}"""
            let operations = API.createCustomJsonPostingKey playerName "sm_find_match" transactionPayload
            let transactionData = API.hive.create_transaction([| operations |] , [| postingKey |])
            let postData = getStringForSplinterlandsAPI transactionData
            let! battleData = API.executeApiPostCall<StartBattleInfo> API.battleUri postData
            return battleData
        }
    
    type CardId = int
    type TeamSelection = 
        {
            summoner_id: int
            monsters: CardId seq
        }

    let concatenateMonsters monsters =
        ""
    let getPlayableCard  (cards: Cards.Card seq) cardId =
        cards |> Seq.find (fun card -> card.card_detail_id = cardId)

    let submitTeam ownedCards playerName postingKey transactionId team =
        async {
            let summoner = 
                let summoner = getPlayableCard ownedCards team.summoner_id
                summoner.uid
            let monsters = 
                team.monsters
                |> Seq.map (getPlayableCard ownedCards)
                |> Seq.map (fun card -> card.uid)
                |> String.concat ","
            let secret = API.generateRandomString 10
            let n = API.generateRandomString 10
            let teamHash = API.generateMD5Hash (sprintf "%s,%s,%s" summoner monsters secret)

            let transactionPayload = 
                sprintf """{"trx_id":"%s","team_hash":"%s","app":"%s","n":"%s"}"""
                    transactionId
                    teamHash
            let operations = API.createCustomJsonPostingKey playerName "sm_submit_team" transactionPayload
            let transactionData = API.hive.create_transaction([| operations|], [| postingKey |]);
            let postData = getStringForSplinterlandsAPI transactionData
            let! battleData = API.executeApiPostCall<StartBattleInfo> API.battleUri postData
            let transaction = battleData.id

            return (secret, transaction)
        }

    let revealTeam ownedCards playerName postingKey transactionId team secret =
        async {
            let summoner = 
                let summoner = getPlayableCard ownedCards team.summoner_id
                summoner.uid
            let monsters = 
                team.monsters
                |> Seq.map (getPlayableCard ownedCards)
                |> Seq.map (fun card -> card.uid)
                |> String.concat ","

            let transactionPayload = 
                sprintf """{"trx_id":"%s","summoner":"%s","monsters":[%s],"secret":"%s","app":"%s","n":"%s"}"""
                    transactionId
                    summoner
                    monsters
                    secret
            let operations = API.createCustomJsonPostingKey playerName "sm_team_reveal" transactionPayload
            let transactionData = API.hive.create_transaction([| operations|], [| postingKey |]);
            let postData = getStringForSplinterlandsAPI transactionData
            let! _ = API.executeApiPostCall<StartBattleInfo> API.battleUri postData
            ()
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
            //opponent: string 
            //match_block_num: int 
            status: int 
            reveal_tx: obj 
            //reveal_block_id: obj 
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
