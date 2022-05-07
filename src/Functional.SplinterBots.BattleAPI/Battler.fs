namespace Functional.SplinterBots.BattleAPI

open Websocket.Client
open System
open System.Collections.Generic
open Newtonsoft.Json.Linq
open Functional.SplinterBots.API
open Functional.SplinterBots.BattleAPI.WebSocket

module Battler =
    type Username = string
    type PostingKey = string
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

    type StartFight = Username -> PostingKey -> Async<Transaction>
    type GetTeam = Username -> unit
    type SubmitTeam = Username -> unit
    type RevealTeam = Username -> unit 

    let startNewMatch username postingkey =
        async {
            let n = Generator.generateRandomString 10
            let json = "{\"match_type\":\"Ranked\",\"app\":\"" + API.applicationIdentifier + "\",\"n\":\"" + n + "\"}";

            let customJson = API.createCustomJsonPostingKey username "sm_find_match"
            let transaction = API.hive.create_transaction ([| customJson |], [| postingkey |])
            let postData = Generator.getStringForSplinterlandsAPI transaction
            let battleUrl = "https://battle.splinterlands.com/battle/battle_tx"
            let! response = API.executeApiPostCall<Transaction> battleUrl postData
            return response
        }

    type WaitResult = 
        | Ok
        | Error

    let rec waitForTransaction (webSocket: WebSocketListener) transactionId attempts =  
        async {
            let timeToWait = (10 - attempts) * 1000 * 2
            do! Async.Sleep timeToWait

            let transactionExists = webSocket.ContainsState GameState.transaction_complete 

            match transactionExists with 
            | true -> 
                let completedTransaction = webSocket.GetState GameState.transaction_complete 
                let previousTtransaction = completedTransaction.["trx_info"] |> Transaction.bind

                match previousTtransaction.id = transactionId && previousTtransaction.success with
                | true -> 
                    return Ok
                | _ -> 
                    return Error
            | _ -> 
                match attempts with 
                | 0 -> 
                    return Error 
                | _ -> 
                    return! waitForTransaction webSocket transactionId (attempts - 1)
        }
    let fight 
            (startFight: StartFight) 
            (getTeam: GetTeam) 
            (submitTeam: SubmitTeam) 
            (revealTeam: RevealTeam)
            (webSocket: WebSocket.WebSocketListener)
            (username: Username)
            (postingKey: PostingKey) =
        async {
            let! transaction = startFight username postingKey

            let! result = waitForTransaction webSocket transaction.id 10
            
            ()
        }
