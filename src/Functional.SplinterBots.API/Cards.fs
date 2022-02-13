namespace Functional.SplinterBots.API

module Cards =
    open System
    open API


    type Card =
        {
           card_detail_id: int
           gold: bool
           edition: int
           xp: int
        }

    type PlayerCardCollection = 
        {
            player: string
            cards: Card array
        }

    let getPlayerCollection playerName =
        async {
            let uri = getCardsApiUri $"collection/{playerName}"
            let! cards = executeApiCall<PlayerCardCollection> uri

            return cards.cards
        }

    let getCardList () =
        async {
            let uri = getCardsApiUri "get_details"
            let! cards = executeApiCall<PlayerCardCollection> uri

            return cards.cards
        }
