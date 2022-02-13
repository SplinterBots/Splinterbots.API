namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.Cards
open FSHttpMock
    
module CardsFixture = 
    

    [<Fact>]
    let ``Get all cards`` () = 
        async {
            FsHttp.initialiseMocking ()
            do! Cards.ensureCardsListIsDownloaded () 

            let! cards = Cards.getCards ()
            
            cards
            |> Seq.length
            |> should equal 442
        }

    [<Fact>]
    let ``Get rare cards`` () = 
        async {
            FsHttp.initialiseMocking ()
            do! Cards.ensureCardsListIsDownloaded () 
            let! cards = Cards.getCards ()
            
            let rareCards = cards |> Cards.filterCardsByRarity Cards.CardRarity.Rare

            rareCards
            |> Seq.length
            |> should equal 132
        }
