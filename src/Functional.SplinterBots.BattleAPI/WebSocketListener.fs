namespace Functional.SplinterBots.BattleAPI

open Websocket.Client
open System
open System.Collections.Generic
open Newtonsoft.Json.Linq

module WebSocket =
    [<Flags>]
    type GameState = 
        | match_found = 0
        | opponent_submit_team = 1
        | battle_result = 2
        | transaction_complete = 3
        | rating_update = 4
        | ecr_update = 5
        | balance_update = 6
        | quest_progress = 7
        | battle_cancelled = 8
        | received_gifts = 9

    let private bindTransaction (token: JToken) =
        {|
            id = token.["id"].ToString()
            success = token.["success"].ToObject<bool>()
        |}

    type WebSocketListener (url: string, username: string, accessToken: string) = 
        let client = new WebsocketClient(new Uri(url))
        let gamesStates = new Dictionary<GameState, JToken>()
        let handleMessage (message: ResponseMessage) =
            let recordMessage = 
                let messageIsText = message.MessageType = System.Net.WebSockets.WebSocketMessageType.Text
                let messageContainsId = message.Text.Contains "\"id\""
                messageIsText && messageContainsId

            if recordMessage
            then 
                let json = JToken.Parse(message.Text);
                let (isValid, state) = Enum.TryParse<GameState>(json["id"].ToString())
                if isValid 
                then 
                    gamesStates[state] <- json.["data"]
            
        let authenticate (client: WebsocketClient) username accessToken = 
            let sessionId = Generator.generateRandomString 10
            let message = 
                sprintf 
                    "{\"type\":\"auth\",\"player\":\"%s\",\"access_token\":\"%s\",\"session_id\":\"%s\"}"
                    username 
                    accessToken 
                    sessionId 
            client.Send message
        let messageRecivedeHandler = client.MessageReceived.Subscribe handleMessage
        
        let containsState state = gamesStates.ContainsKey (state)
        let getState state =  gamesStates[state]

        let rec waitForGameState (state: GameState) attempts =  
            async {
                let timeToWait = (10 - attempts) * 1000 * 2
                do! Async.Sleep timeToWait

                let gameStateExists = containsState GameState.transaction_complete 

                match gameStateExists with 
                | true -> 
                    return Ok
                | _ -> 
                    match attempts with 
                    | 0 -> 
                        return Error 
                    | _ -> 
                        return! waitForGameState state (attempts - 1)
            }
        let rec waitForTransaction transactionId attempts =  
            async {
                let timeToWait = (10 - attempts) * 1000 * 2
                do! Async.Sleep timeToWait

                let transactionExists = containsState GameState.transaction_complete 

                match transactionExists with 
                | true -> 
                    let completedTransaction = getState GameState.transaction_complete 
                    let previousTtransaction = completedTransaction.["trx_info"] |> bindTransaction

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
                        return! waitForTransaction transactionId (attempts - 1)
            }

        do 
            client.ReconnectTimeout <- TimeSpan.FromMinutes 5
            
        member this.Start () =
            async {
                do! client.Start() |> Async.AwaitTask

                authenticate client username accessToken
            }

        member this.WaitForGamesState state = 
            async {
                return! waitForGameState state 10
            }

        member this.WaitForTransaction transactionId = 
            async {
                return waitForTransaction transactionId 10
            }

        member this.GetState state =
            getState state

        interface IDisposable with 
            member this.Dispose () = 
                messageRecivedeHandler.Dispose()
                client.Dispose ()
                gamesStates.Clear()
