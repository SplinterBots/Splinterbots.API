namespace Functional.SplinterBots.API

open System
open System.Text.Json

module Rewards =
    type RewardCard = 
        {
            uid: string
            card_detail_id: int
            xp : int
            gold: bool
            edition: int
        }
    type Rewards =
        {
            ``type``: string 
            quantity: int
            potion_type: string
            card: RewardCard
        }

    let toString reward = 
        match reward.``type`` with 
        | "potion" -> $"{reward.potion_type} {reward.``type``} x {reward.quantity}"
        | _ -> $"{reward.``type``} x {reward.quantity}"

    let convertToList (rewardArray: string) = 
        let rewards = JsonSerializer.Deserialize<Rewards array>(rewardArray)
        rewards

module Quest =
    let private uri name = 
        Urls.getPlayerUri "quests" $"username={name}"
    type PlayerQuests = 
        {
            id: string
            player: string
            created_date: DateTime option 
            created_block: int
            name: string
            total_items: int
            completed_items: int
            claim_date: DateTime option 
            reward_qty: int
            rewards: string
        }
    
    type QuestProgress = int * int
    type DailyReward = 
        | InProgress of QuestProgress
        | ReadyToClaim
        | Claimed of string seq
        | Unknown
    module PlayerQuests =
        let empty = 
            {
                id = ""
                player = ""
                created_date = None
                created_block = 0
                name = ""
                total_items = 0
                completed_items = 0
                claim_date = None
                reward_qty = 0
                rewards = ""
            }
    let getClaimedDailyRewards questDetails = 
        (Rewards.convertToList questDetails.rewards 
        |> Seq.map Rewards.toString)

    let getDailyReward (details: PlayerQuests) =
        match details with 
        | { claim_date = y } when y.IsSome -> 
            DailyReward.Claimed (getClaimedDailyRewards details)
        | x when x.completed_items = x.total_items -> DailyReward.ReadyToClaim
        | x when x.completed_items < x.total_items -> DailyReward.InProgress (x.completed_items, x.total_items)
        | _ -> DailyReward.Unknown

    let getQuest playerName =
        async {
            let  uri = uri playerName
            let! quests = Http.executeApiCall<PlayerQuests array> uri
            return 
                match quests.Length with
                | 0 -> None 
                | _ -> quests |> Array.item 0 |> Some
        }

    let claimQuest questId playerName postingKey = 
        async {
            let randomNumber = Randomizer.generateRandomString 10
            let transactionPayload  = 
                sprintf "{\"type\":\"quest\",\"quest_id\":\"%s\",\"app\":\"%s\",\"n\":\"%s\"}" 
                    questId 
            let operations = Hive.createCustomJsonPostingKey playerName "sm_claim_reward" transactionPayload
            let txid = Hive.brodcastTransaction operations postingKey
            Hive.waitForTransaction playerName txid |> ignore
        }
    let startNewQuest playerName postingKey = 
        async {
            let randomNumber = Randomizer.generateRandomString 10
            let transactionPayload  = sprintf "{\"type\":\"daily\",\"app\":\"%s\",\"n\":\"%s\"}" 
            let operations = Hive.createCustomJsonPostingKey playerName "sm_start_quest" transactionPayload
            Hive.brodcastTransaction operations postingKey |> ignore
        }

module LastSeasonRewards = 
    let private uri name = 
        Urls.getPlayerUri "last_season_rewards" $"username={name}"

    type SeasonRewards = 
        {
            success: bool
            rewards: Rewards.Rewards array
            claim_date: DateTime option
            error: string
        }
    module SeasonRewards =
        let empty = 
            {
                success = false
                rewards = [||]
                claim_date = None
                error = ""
            }

    let getLastSeasonRewards playerName =
        async {
            let  uri = uri playerName
            return! Http.executeApiCall<SeasonRewards> uri
        }
    let claimSeason seasonId playerName postingKey = 
        async {
            let randomNumber = Randomizer.generateRandomString 10
            let transactionPayload  = 
                sprintf "{\"type\":\"league_season\",\"season\":%i,\"app\":\"%s\",\"n\":\"%s\"}"
                    seasonId 
            let operations = Hive.createCustomJsonPostingKey playerName "sm_claim_reward" transactionPayload
            let txid = Hive.brodcastTransaction operations postingKey
            Hive.waitForTransaction playerName txid |> ignore
        }

