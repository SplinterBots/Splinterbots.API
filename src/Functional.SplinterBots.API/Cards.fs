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
        
        
    let private starterCardsList = 
        [157; 158; 159; 160; 395; 396; 397; 398; 399; 161; 162; 163; 167; 400; 401; 402; 403; 440; 168; 169; 170; 171; 381; 382; 383; 384; 385; 172; 173; 174; 178; 386; 387; 388; 389; 437; 179; 180; 181; 182; 367; 368; 369; 370; 371; 183; 184; 185; 189; 372; 373; 374; 375; 439; 146; 147; 148; 149; 409; 410; 411; 412; 413; 150; 151; 152; 156; 414; 415; 416; 417; 135; 135; 136; 137; 138; 353; 354; 355; 356; 357; 139; 140; 141; 145; 358; 359; 360; 361; 438; 224; 190; 191; 192; 157; 423; 424; 425; 426; 194; 195; 196; 427; 428; 429;]

    let getStarterCards () =
        let getStarterCardId cardId =
            sprintf "starter-%i-%s" cardId (API.generateRandomString 5)

        starterCardsList
        |> Seq.map (fun cardId -> {card_detail_id = cardId; gold = false; level = 1; player = "starter"; uid = (getStarterCardId cardId)})
    
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

