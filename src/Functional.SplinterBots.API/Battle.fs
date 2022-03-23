namespace Functional.SplinterBots.API

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
                getBattleApiUri "history" $"player={userName}"
            let! battleResult = executeApiCall<PlayerBatleStats> uri

            return battleResult.battles
        }

    let getBatleDetails battleId = 
        async {
            let uri = 
                getBattleApiUri "result" $"id={battleId}"
            let! battleResult = executeApiCall<PlayerBatleStats> uri

            return battleResult.battles
        }


    //https://api2.splinterlands.com/battle/result?id=sl_82b715642ec2df5af891e9caf24d11d6&v=1647346276389&token=5LY94WARMO&username=assassyn
     
    let findNextMatch playerName postingKey =
        let transactionPayload = sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"
        let operations = API.createCustomJsonPostingKey playerName "" transactionPayload
        let txid = API.hive.create_transaction([| operations |] , [| postingKey |])
        //API.waitForTransaction playerName txid
        ()

     //string n = Helper.GenerateRandomString(10);
            //string json = "{\"match_type\":\"Ranked\",\"app\":\"" + Settings.SPLINTERLANDS_APP + "\",\"n\":\"" + n + "\"}";

            //COperations.custom_json custom_Json = CreateCustomJson(false, true, "sm_find_match", json);

            //try
            //{
            //    Log.WriteToLog($"{Username}: Finding match...");
            //    CtransactionData oTransaction = Settings.oHived.CreateTransaction(new object[] { custom_Json }, new string[] { PostingKey });
            //    var postData = GetStringForSplinterlandsAPI(oTransaction);
            //    return HttpWebRequest.WebRequestPost(Settings.CookieContainer, postData, "https://battle.splinterlands.com/battle/battle_tx", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0", "", Encoding.UTF8);
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteToLog($"{Username}: Error at finding match: " + ex.ToString(), Log.LogType.Error);
            //}
