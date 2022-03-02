namespace Functional.SplinterBots.API

module Market =

    open API

    type CardGroup =
        {
           card_detail_id: int
           gold: bool
           edition: int
           qty: int
           low_price: decimal
           low_price_bcx: decimal
           high_price: decimal
           level: int
           mana: int
        }

    type CardOnMarket =
        {
            market_id: string
            fee_percent: decimal
            uid: string
            seller: string
            card_detail_id: int
            xp: int
            gold: bool
            edition: int
            buy_price: string
            currency: string
            desc: string
            ``type``: string
        }

    type Currency =
        | DEC
        | CREDITS

    type MarketAction =
        | Rent
        | Buy

    let getCardsGroupFromMarket marketAction =
        let sortByLowPriceDescending (cards: CardGroup seq) =
            cards |> Seq.sortBy (fun x -> x.low_price)
        async {
            let uri =
                let action =
                    match marketAction with
                    | Rent -> "for_rent_grouped"
                    | Buy -> "for_sale_grouped"
                getMarketUri action
            let! items = executeApiCall<CardGroup seq> uri
            let sortedItems =  items |> sortByLowPriceDescending
            return sortedItems
        }

    let getCardsDetailsFromMarket marketAction (group: CardGroup) =
        let sortByLowPriceDescending (cards: CardOnMarket seq) =
            cards |> Seq.sortBy (fun card -> float(card.buy_price))
        let uri =
            let action =
                let actionName =
                    match marketAction with
                    | Rent -> "for_rent_by_card"
                    | Buy -> "for_sale_by_card"
                sprintf "%s?card_detail_id=%i&gold=%b&edition=%i"
                    actionName
                    group.card_detail_id
                    group.gold
                    group.edition
            getMarketUri action
        async {
            let! items = executeApiCall<CardOnMarket seq> uri
            let sortedItems =
                items
                |> sortByLowPriceDescending
            return sortedItems
        }

    let buyCard cardDetails currency playerName activeKey =
        let transactionPayload  =
            let buyPrice = float(cardDetails.buy_price)
            sprintf "{\"items\":[\"%s\"],\"price\":%s,\"currency\":\"%A\",\"app\":\"%s\",\"n\":\"%s\"}"
                cardDetails.market_id
                (buyPrice.ToString("0.000"))
                currency
        let operations = API.createCustomJsonActiveKey playerName "sm_market_purchase" transactionPayload
        let txid = API.hive.broadcast_transaction([| operations |] , [| activeKey |])
        API.waitForTransaction playerName txid

    let rentCards items currency days playerName activeKey =
        let transactionPayload  =
            sprintf "{\"items\":[%s],\"currency\":\"%A\",\"days\":%i,\"app\":\"%s\",\"n\":\"%s\"}"
                items
                currency
                days
        let operations = API.createCustomJsonActiveKey playerName "sm_market_rent" transactionPayload
        let txid = API.hive.broadcast_transaction([| operations |] , [| activeKey |])
        API.waitForTransaction playerName txid
