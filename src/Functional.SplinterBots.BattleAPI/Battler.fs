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
    type GetTeam = MatchDetails -> Async<Team>
    type FightLog = 
        | CheckingForExistingBattle
        | BattleAlreadyStarted
        | StartNewMatch
        | MatchFound
        | GettingTeam
        | TeamSelected
        | SubmitingTeam
        | TeamSubmited
        | RevealTeam
        | FinishedBattle

    
    let private startNewMatch username postingkey =
        async {
            let customJson = 
                sprintf "{\"match_type\":\"Ranked\",\"app\":\"%s\",\"n\":\"%s\"}"
                |> Hive.createCustomJsonPostingKey username "sm_find_match"
            let! response = 
                Hive.createTransaction customJson postingkey
                |> Generator.getStringForSplinterlandsAPI
                |> Http.executeApiPostCall<Transaction> Urls.battleUrl 
            return response
        }
              
    let private submitTeam username postingKey (transaction: Transaction) (team: Team) = 
        async {
            let custom_Json = 
                sprintf "{\"trx_id\":\"%s\",\"team_hash\":\"%s\",\"app\":\"%s\",\"n\":\"%s\"}"
                    transaction.id
                    team.TeamHash
                |> Hive.createCustomJsonPostingKey username "sm_submit_team"
            let! response = 
                Hive.createTransaction custom_Json postingKey
                |> Generator.getStringForSplinterlandsAPI
                |> Http.executeApiPostCall<Transaction> Urls.battleUrl
            return response
        }
    
    let private revealTeam username postingKey (transaction: Transaction) (team: Team) = 
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
    
            let custom_Json = Hive.createCustomJsonPostingKey username "sm_team_reveal" json
            let postData = 
                Hive.createTransaction custom_Json postingKey
                |> Generator.getStringForSplinterlandsAPI 
                   
            do! Http.executeApiPostCall<Transaction> Urls.battleUrl postData |> Async.Ignore
        }

    let fight
            username 
            postingKey
            (webSocket: WebSocketListener)
            (logger: Logger.Logger<FightLog>)
            (getTeam: GetTeam) =
        async {
            do! logger FightLog.CheckingForExistingBattle
            let! matchInfo  = OutstandingMatch.getOutstandingMatch username

            match matchInfo.IsSome && matchInfo.Value.team_hash <> "" with
            | true -> do! logger FightLog.BattleAlreadyStarted
            | _ ->
                do! logger FightLog.StartNewMatch
                let! transaction = startNewMatch username postingKey
                do! webSocket.WaitForTransaction transaction.id
                do! webSocket.WaitForGamesState GameState.match_found 

                do! logger FightLog.MatchFound
                let matchDetails = webSocket.GetState GameState.match_found |> MatchDetails.bind

                do! logger FightLog.GettingTeam
                let! team = getTeam matchDetails
                do! logger FightLog.TeamSelected

                do! logger FightLog.SubmitingTeam
                let! submitedTeam = submitTeam username postingKey transaction team

                let _ = webSocket.WaitForTransaction submitedTeam.id
                do! logger FightLog.TeamSubmited


                do! logger FightLog.RevealTeam
                do! revealTeam username postingKey transaction team
            
                do! webSocket.WaitForGamesState GameState.opponent_submit_team
                do! logger FightLog.FinishedBattle            
        }
