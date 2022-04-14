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
    
    type CardId = int
    type TeamSelection = 
        {
            summoner_id: int
            monsters: CardId seq
        }

    let concatenateMonsters monsters =
        ""
    let submitTeam (ownedCards: Cards.Card seq) transactionId (team: TeamSelection) =
        async {
            let summonerLongId = 
                let summoner = ownedCards |> Seq.find (fun card -> card.card_detail_id = team.summoner_id)
                summoner.card_detail_id
            let monsters = 
            return ()
        }
//private async Task<(string secret, string tx, JToken team)> SubmitTeamAsync(string tx, JToken matchDetails, JToken team, bool secondTry = false)
//    {
//        try
//        {
//            var cardQuery = CardsCached.Where(x => x.card_detail_id == (string)team["summoner_id"]);
//            string summoner = cardQuery.Any() ? cardQuery.First().card_long_id : null;
//            string monsters = "";
//            for (int i = 0; i < 6; i++)
//            {
//                var monster = CardsCached.Where(x => x.card_detail_id == (string)team[$"monster_{i + 1}_id"]).FirstOrDefault();
//                if (monster == null || summoner == null)
//                {
//                    if (Settings.UsePrivateAPI && !secondTry)
//                    {
//                        Log.WriteToLog($"{Username}: Requesting team from public API - private API needs card update!", Log.LogType.Warning);
//                        CardsCached = await SplinterlandsAPI.GetPlayerCardsAsync(Username);
//                        team = await GetTeamAsync(matchDetails, ignorePrivateAPI: true);
//                        BattleAPI.UpdateCardsForPrivateAPI(Username, CardsCached);
//                        return await SubmitTeamAsync(tx, matchDetails, team, secondTry: true);
//                    }
//                    else
//                    {
//                        continue;
//                    }
//                }
//                if (monster.card_detail_id.Length == 0)
//                {
//                    break;
//                }

//                monsters += "\"" + monster.card_long_id + "\",";
//            }
//            monsters = monsters[..^1];

//            string secret = Helper.GenerateRandomString(10);
//            string n = Helper.GenerateRandomString(10);

//            string monsterClean = monsters.Replace("\"", "");

//            string teamHash = Helper.GenerateMD5Hash(summoner + "," + monsterClean + "," + secret);

//            string json = "{\"trx_id\":\"" + tx + "\",\"team_hash\":\"" + teamHash + "\",\"app\":\"" + Settings.SPLINTERLANDS_APP + "\",\"n\":\"" + n + "\"}";

//            COperations.custom_json custom_Json = CreateCustomJson(false, true, "sm_submit_team", json);

//            Log.WriteToLog($"{Username}: Submitting team...");
//            CtransactionData oTransaction = Settings.oHived.CreateTransaction(new object[] { custom_Json }, new string[] { PostingKey });
//            var postData = GetStringForSplinterlandsAPI(oTransaction);
//            var response = HttpWebRequest.WebRequestPost(Settings.CookieContainer, postData, "https://battle.splinterlands.com/battle/battle_tx", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0", "https://splinterlands.com/", Encoding.UTF8);
//            string responseTx = Helper.DoQuickRegex("id\":\"(.*?)\"", response);
//            return (secret, responseTx, team);
//        }
//        catch (Exception ex)
//        {
//            Log.WriteToLog($"{Username}: Error at submitting team: " + ex.ToString(), Log.LogType.Error);
//            // update cards for private API
//            APICounter = 100;
//        }
//        return ("", "", null);
//    }

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
