namespace Functional.SplinterBots.API

[<AutoOpen>]
module API =

    open System
    open System.Text.Json
    open System.Net.Http
    open HiveAPI
    open FsHttp
    open FsHttp.DslCE
    open FsHttp.Response

    let applicationIdentifier = "SplinterBots/0.9"
    let hiveApi = "anyx.io"
    let hiveNodeUrl = $"https://{hiveApi}"

    let hive = new CHived(new HttpClient(), hiveNodeUrl)

    let api2Uri = 
        "https://api2.splinterlands.com"
    let cacheApiUri action queryParameters= 
        $"https://cache-api.splinterlands.com/{action}?{queryParameters}"
    let gameApiUri action =
        $"https://game-api.splinterlands.com/{action}" 
        
    let getPlayerUri action queryParameters =
        $"{api2Uri}/players/{action}?{queryParameters}"
    let getCardsUri action = 
        $"{api2Uri}/cards/{action}"
    let getMarketUri action =
        $"{api2Uri}/market/{action}"
    let getBattleUri action queryParameters = 
        $"{api2Uri}/battle/{action}?{queryParameters}"
    let getECUri action queryParameters =
        $"https://ec-api.splinterlands.com/players/{action}?{queryParameters}"
    let battleUri  = "https://battle.splinterlands.com/battle/battle_tx"

    let setToUserNameWhenTrue username isTrue =
        match isTrue with 
        | true -> [| username |]
        | _ -> [||]

    let executeApiCallToString url  =
        async {
            let! response =
                httpAsync {
                    GET url
                    CacheControl "no-cache"
                    UserAgent "SplinterBots"
                }
            return response |> Response.toString Int32.MaxValue
        }

    let executeApiCall<'T> url  =
        async {
            let! response =
                httpAsync {                   
                    GET url
                    CacheControl "no-cache"
                    UserAgent "SplinterBots"
                }
                
            let! responseStream =  response |> toStreamAsync
            return! JsonSerializer.DeserializeAsync<'T>(responseStream).AsTask() |> Async.AwaitTask   
        }

    
    let executeApiPostCall<'T> url payload = 
        async {
            let! response =
                httpAsync {                   
                    POST url
                    UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0"
                    ContentType "application/x-www-form-urlencoded"                                      
                    body 
                    json payload
                }
            
            let! responseStream =  response |> toStreamAsync
            return! JsonSerializer.DeserializeAsync<'T>(responseStream).AsTask() |> Async.AwaitTask 
        }


    let generateRandomString numebrOfCharacters = 
        let randomizer = Random()
        let chars = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
        let sz = Array.length chars in
        String(Array.init numebrOfCharacters (fun _ -> chars.[randomizer.Next sz])).ToString()

    let private createCustomJson username activeKey postingKey methodName json = 
        new COperations.custom_json (
            id = methodName,
            json = json,
            required_auths = setToUserNameWhenTrue username activeKey,
            required_posting_auths = setToUserNameWhenTrue username postingKey)

    let createCustomJsonActiveKey username methodName getJson = 
        let json = getJson applicationIdentifier (generateRandomString 10)
        createCustomJson username true false methodName json

    let createCustomJsonPostingKey username methodName getJson = 
        let json = getJson applicationIdentifier (generateRandomString 10)
        createCustomJson username false true methodName json
        
    type TransactionId = string
    type TransactionResult = 
        | FinishedOk of TransactionId
        | DidNotFinished of TransactionId
        
    let waitForTransaction playerName transactionId =
        let sleepTime = TimeSpan.FromSeconds 5.0
        let mutable counter = 25
        while counter > 0 do 
            let transactions = hive.get_account_history (playerName, int64 -1, uint32 3)
            let containsTransaction = 
                transactions.Children()
                |> Seq.map (fun token -> token.Last)
                |> Seq.map (fun trans -> trans.["trx_id"].ToString() )
                |> Array.ofSeq
                |> Array.contains transactionId

            if  not containsTransaction 
            then 
                Threading.Thread.Sleep sleepTime
                counter <- counter - 1 
            else 
                counter <- 0

        match counter with 
        | x when x = 0 -> DidNotFinished transactionId
        | _  -> FinishedOk transactionId

