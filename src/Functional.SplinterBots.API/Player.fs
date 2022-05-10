namespace Functional.SplinterBots.API

module Player =
    
    open API 
    open UnionTools

    module Balance =
        type TokenBalance = 
            {
                player: string
                token: string
                balance: decimal
            }
        type Token = 
            | DEC
            | VOUCHER
            | SPS
            | CREDITS
            | SPSP
            | GOLD
            | QUEST
            | LEGENDARY
            | ECR
            | UNTAMED
            | ORB
            | BETA
            | ALFA
        type PlayerBalance =
            {
                dec: decimal
                voucher: decimal 
                sps: decimal
                credits: decimal
                spsp: decimal
                gold: decimal
                quest: decimal
                legendary: decimal
                ecr: decimal
                untamed: decimal
                orb: decimal
                beta: decimal
                alfa: decimal
            }
        module PlayerBalance =
            let empty = 
                {
                    dec = 0.0M
                    voucher = 0.0M
                    sps = 0.0M
                    credits = 0.0M
                    spsp = 0.0M
                    gold = 0.0M
                    quest = 0.0M
                    legendary = 0.0M
                    ecr = 0.0M
                    untamed = 0.0M
                    orb = 0.0M
                    beta = 0.0M
                    alfa = 0.0M
                }
        let private getPlayerBalanceApiUrl name = 
            Urls.getPlayerUri "balances" $"username={name}"

        let getBalance playerName =
            let getBalance (items: TokenBalance seq) (token: Token) = 
                let item =
                    items 
                    |> Seq.tryFind (fun item -> token =~ item.token)
                match item with 
                | Some x ->x.balance
                | None -> 0M
            async {
                let uri = getPlayerBalanceApiUrl playerName
                let! items = executeApiCall<TokenBalance array> uri
                return 
                    {
                        dec = getBalance items Token.DEC
                        voucher = getBalance items Token.VOUCHER
                        sps = getBalance items Token.SPS
                        credits = getBalance items Token.CREDITS
                        spsp = getBalance items Token.SPSP
                        gold = getBalance items Token.GOLD
                        quest = getBalance items Token.QUEST
                        legendary = getBalance items Token.LEGENDARY
                        ecr = getBalance items Token.ECR
                        untamed = getBalance items Token.UNTAMED
                        orb = getBalance items Token.ORB
                        beta = getBalance items Token.BETA
                        alfa = getBalance items Token.ALFA
                    }
            }
    module Details =
        let private uri name = 
            Urls.getPlayerUri "details" $"name={name}"
        type PlayerDetails = 
            {
                name: string
                join_date: string
                rating: int
                battles: int
                wins: int
                current_streak: int
                longest_streak: int
                max_rating:int
                max_rank:int
                champion_points:int
                capture_rate:int 
                last_reward_block:int
                collection_power: int
                league: int
            }
        module PlayerDetails = 
            let empty = 
                {
                    name = ""
                    join_date = ""
                    rating = 0
                    battles = 0
                    wins = 0
                    current_streak = 0
                    longest_streak = 0
                    max_rating = 0
                    max_rank = 0
                    champion_points = 0
                    capture_rate = 0
                    last_reward_block = 0
                    collection_power = 0
                    league = 0
                }

        let getDetails playerName =
            async {
                let uri = uri playerName
                return! executeApiCall<PlayerDetails> uri
            }
