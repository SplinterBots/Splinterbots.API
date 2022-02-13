namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
    
module Quest = 
    [<Fact>]
    let ``Can deserialize Quest`` () = 
        let questRewards = 
            "[{\"type\":\"potion\",\"quantity\":1,\"potion_type\":\"gold\"},{\"type\":\"dec\",\"quantity\":4}]"
        let result = Rewards.convertToList questRewards

        Seq.length result |> should equal 2
    
    [<Fact>]
    let ``First item is Potion `` () = 
        let questRewards = 
            "[{\"type\":\"potion\",\"quantity\":1,\"potion_type\":\"gold\"},{\"type\":\"dec\",\"quantity\":4}]"
        let result = Rewards.convertToList questRewards

        let item =
            result 
            |> Seq.item 0 
        item.``type`` |> should equal "potion"
