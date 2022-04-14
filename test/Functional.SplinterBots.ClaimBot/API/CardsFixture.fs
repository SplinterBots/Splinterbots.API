namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader
open Functional.SplinterBots.API.Battle

module CardsFixture =

    [<Fact>]
    let ``Can get battle result`` () =
        async {
            FsHttp.initialiseMocking ()
            
            Cards.cardsList
            |> Seq.length
            |> should equal 450
        }

    [<Fact>]
    let `` Can get starter cards`` () =
        async {
            FsHttp.initialiseMocking ()
        
            Cards.getStarterCards ()
            |> Seq.length
            |> should equal 30
        }

    [<Fact>]
    let ``Starter cards have the started card id`` () =
        async {
            FsHttp.initialiseMocking ()
            
            let startedCard =
                Cards.getStarterCards ()
                |> Seq.head

            startedCard.uid
            |> should startWith "starter-"
        }

    [<Fact>]
    let ``Players cards have the non started card id`` () =
        async {
            FsHttp.initialiseMocking ()
        
            let! startedCard = Cards.getPlayerCards "ThePlayer"
            let startedCard = startedCard |> Seq.head

            startedCard.uid
            |> should not' (startWith "starter-")
        }

    [<Fact>]
    let ``Can merge all available cards`` () =
        async {
            FsHttp.initialiseMocking ()
            
            let! cards = Cards.getAvailableCardsForPlayer "ThePlayer"

            cards
            |> Seq.length
            |> should equal 65
        }
