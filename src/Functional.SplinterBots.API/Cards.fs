namespace Functional.SplinterBots.API

module CardTypes = 
    type Card =
        {
           card_detail_id: int
           gold: bool
           level: int
           card_long_id: string
           player: string
        }
    type RawCard = 
        {
            player: string 
            uid: string 
            card_detail_id: int 
            xp: int 
            gold: bool 
            delegated_to: string option  
            level: int 
        }
    type PlayersCardCollection = 
        {
            player: string
            cards: RawCard seq
        }

module Cards =

    open System
    open API
    open CardTypes


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


    let private  allowPlayableCards username (card: RawCard) =
        let isOwner = card.player = username
        let isDelegatedTo = 
            match card.delegated_to with
            | Some x -> x = username
            | None -> false
        isOwner || isDelegatedTo

    let private  convertToCards (card: RawCard): Card = 
        {
           card_detail_id = card.card_detail_id
           gold = card.gold
           level = card.level
           card_long_id = card.uid
           player = card.player
        }
    let private getGhostCards () =
        let ghostCardIds = [ "157"; "158"; "159"; "160"; "395"; "396"; "397"; "398"; "399"; "161"; "162"; "163"; "167"; "400"; "401"; "402"; "403"; "440"; "168"; "169"; "170"; "171"; "381"; "382"; "383"; "384"; "385"; "172"; "173"; "174"; "178"; "386"; "387"; "388"; "389"; "437"; "179"; "180"; "181"; "182"; "367"; "368"; "369"; "370"; "371"; "183"; "184"; "185"; "189"; "372"; "373"; "374"; "375"; "439"; "146"; "147"; "148"; "149"; "409"; "410"; "411"; "412"; "413"; "150"; "151"; "152"; "156"; "414"; "415"; "416"; "417"; "135"; "135"; "136"; "137"; "138"; "353"; "354"; "355"; "356"; "357"; "139"; "140"; "141"; "145"; "358"; "359"; "360"; "361"; "438"; "224"; "190"; "191"; "192"; "157"; "423"; "424"; "425"; "426"; "194"; "195"; "196"; "427"; "428"; "429"; ] 
        let createCardFromId cardId: Card = 
            {
                card_detail_id = cardId
                gold = false
                level = 1
                card_long_id = $"starter-{cardId}-{Randomizer.generateRandomString 5}"
                player = ""
             }
        ghostCardIds 
        |> Seq.map Int32.Parse
        |> Seq.distinct
        |> Seq.sort
        |> Seq.map createCardFromId

    let getPlayerCards username = 
        async {
            let cardsUri = $"{api2Uri}/cards/collection/{username}"
            let! cards  = executeApiCall<PlayersCardCollection> cardsUri
            let playerCards = 
                cards.cards
                |> Seq.filter (allowPlayableCards username)
                |> Seq.map convertToCards
            let ghostCards = getGhostCards ()
            return Seq.concat [playerCards; ghostCards;]
        }
