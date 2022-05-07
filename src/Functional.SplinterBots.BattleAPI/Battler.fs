namespace Functional.SplinterBots.BattleAPI

open Websocket.Client
open System
open System.Collections.Generic
open Newtonsoft.Json.Linq
open Functional.SplinterBots.API
open Functional.SplinterBots.BattleAPI.WebSocket
open System.Threading.Tasks

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

    type MatchDetails =
        {
            id: string
        }
    module MatchDetails =
        let bind (token: JToken) =
            {
                id = ""
            }
   
    type Team =
        {
            id: string
        }
   
    type StartFight = Username -> PostingKey -> Async<Transaction>
    type GetTeam = MatchDetails -> Async<Team>
    type SubmitTeam = Team -> Async<Transaction>
    type RevealTeam = Team -> Async<Transaction> 

    let fight 
            (startFight: StartFight) 
            (getTeam: GetTeam) 
            (submitTeam: SubmitTeam) 
            (revealTeam: RevealTeam)
            (webSocket: WebSocketListener)
            (username: Username)
            (postingKey: PostingKey) =
        async {
            let! transaction = startFight username postingKey
            let! _ = webSocket.WaitForTransaction transaction.id
            let! _ = webSocket.WaitForGamesState GameState.match_found 

            let matchDetails = webSocket.GetState GameState.match_found |> MatchDetails.bind
            let! team = getTeam matchDetails

            do! (Task.Delay 8000 |> Async.AwaitTask)

            let! submitedTeam = submitTeam team
            let! _ = webSocket.WaitForTransaction submitedTeam.id

            let! revealedTeam = revealTeam team
            ()
        }

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


