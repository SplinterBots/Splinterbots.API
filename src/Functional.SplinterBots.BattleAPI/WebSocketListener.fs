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

        do 
            client.ReconnectTimeout <- TimeSpan.FromMinutes 5
            
        member this.Start () =
            async {
                do! client.Start() |> Async.AwaitTask

                authenticate client username accessToken
            }

        member this.ContainsState state =
            gamesStates.ContainsKey (state)

        member this.GetState state = 
            gamesStates[state]

        interface IDisposable with 
            member this.Dispose () = 
                messageRecivedeHandler.Dispose()
                client.Dispose ()
                gamesStates.Clear()
