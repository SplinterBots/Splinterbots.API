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
        
    type StartFight = unit -> Async<Transaction>
    type GetTeam = MatchDetails -> Async<Team>
    type SubmitTeam = Transaction -> Team -> Async<Transaction>
    type RevealTeam = Transaction -> Team -> Async<unit> 

    let fight
            (startFight: StartFight) 
            (getTeam: GetTeam) 
            (submitTeam: SubmitTeam) 
            (revealTeam: RevealTeam)
            (webSocket: WebSocketListener) =
        async {
            let! transaction = startFight ()
            do! webSocket.WaitForTransaction transaction.id
            do! webSocket.WaitForGamesState GameState.match_found 

            let matchDetails = webSocket.GetState GameState.match_found |> MatchDetails.bind
            let! team = getTeam matchDetails

            do! (Task.Delay 5000 |> Async.AwaitTask)

            let! submitedTeam = submitTeam transaction team
            let _ = webSocket.WaitForTransaction submitedTeam.id

            do! revealTeam transaction team

            do! webSocket.WaitForGamesState GameState.opponent_submit_team
        }

    let startNewMatch username postingkey =
        fun () -> 
            async {
                let customJson = 
                    sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"
                    |> API.createCustomJsonPostingKey username "sm_find_match"
                let! response = 
                    API.hive.create_transaction ([| customJson |], [| postingkey |])
                    |> Generator.getStringForSplinterlandsAPI
                    |> API.executeApiPostCall<Transaction> API.battleUrl 
                return response
            }
          
    let submitTeam username postingKey (transaction: Transaction) (team: Team) = 
        async {
            let custom_Json = 
                sprintf "{\"trx_id\":\"%s\",\"team_hash\":\"%s\",\"app\":\"%s\",\"n\":\"%s\"}"
                    transaction.id
                    team.TeamHash
                |> API.createCustomJsonPostingKey username "sm_submit_team"
            let! response = 
                API.hive.create_transaction([| custom_Json |], [| postingKey |])
                |> Generator.getStringForSplinterlandsAPI
                |> API.executeApiPostCall<Transaction> API.battleUrl
            return response
        }

    let revealTeam username postingKey (transaction: Transaction) (team: Team) = 
        async {
            let monsters = 
                team.Team
                |> Seq.map (fun monster -> monster.card_long_id)
                |> Seq.map (sprintf "\"%s\"")
                |> String.concat ","
            let json = 
                sprintf "{\"trx_id\":\"%s\",\"summoner\":\"%s\",\"monsters\":[%s],\"secret\":\"%s\",\"app\":\"%s\",\"n\":\"%s\"}"
                    transaction.id
                    team.Summoner.card_long_id
                    monsters
                    team.Secret

            let custom_Json = API.createCustomJsonPostingKey username "sm_team_reveal" json
            let postData = 
                API.hive.create_transaction([| custom_Json |], [| postingKey |])
                |> Generator.getStringForSplinterlandsAPI 
               
            do! API.executeApiPostCall<Transaction> API.battleUrl postData |> Async.Ignore
        }
