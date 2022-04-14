namespace Functional.SplinterBots.API

module Cards =
    open System
    open API


    type Card =
        {
           card_detail_id: int
           gold: bool
           level: int
           player: string
           uid: string
        }

    let filterCardsByOwningPlayer playerName (cards: Card seq) = 
        cards
        |> Seq.filter (fun card -> card.player = playerName)
        
    let sentCards cardIds destinationPlayerName playerName activeKey =
        let transactionPayload  =
            sprintf "{\"to\":\"%s\",\"cards\":[%s],\"app\":\"%s\",\"n\":\"%s\"}"
                destinationPlayerName
                cardIds
        let operations = API.createCustomJsonActiveKey playerName "sm_gift_cards" transactionPayload
        let txid = API.hive.broadcast_transaction([| operations |] , [| activeKey |])
        API.waitForTransaction playerName txid

    type CardType =
        | Monster = 1
        | Splinter = 2

    type CardColour =
        | Red = 1
        | Blue = 2
        | Green = 3
        | White = 4
        | Black = 5
        | Gold = 6
        | Gray = 7

    [<Flags>]
    type CardRarity  =
        | Common = 1
        | Rare = 2
        | Epic = 3
        | Legendary = 4

    type CardListItem =
        {
            id: int
            name: string
            ``type``: string
            color: string
            rarity: CardRarity
            is_starter: bool
        }

    let cardsList = 
        let cardsUri = $"{api2Uri}/cards/get_details"
        let rawCardsData = executeApiCall<CardListItem seq> cardsUri |> Async.RunSynchronously
        rawCardsData

    let getStarterCards () =
        let getStarterCardId cardId =
            sprintf "starter-%i-%s" cardId (API.generateRandomString 5)
        cardsList 
        |> Seq.filter (fun card -> card.is_starter)
        |> Seq.map (fun card -> {card_detail_id = card.id; gold = false; level = 1; player = "starter"; uid = (getStarterCardId card.id)})
    
    let getPlayerCards playerName =
        async {
            let uri = getCardsUri $"collection/{playerName}"
            let! cards = executeApiCall<{|player: string;cards: Card seq; |}> uri
    
            return cards.cards
        }

    let getAvailableCardsForPlayer playerName =
        async {
            let starterCards = getStarterCards ()
            let! playerCards = getPlayerCards playerName

            let playableCards = 
                playerCards
                |> Seq.append starterCards
                |> Seq.distinctBy (fun card -> card.card_detail_id)

            return playableCards
        }

