namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader
open Functional.SplinterBots.API.Battle

module BattleFixture =

    [<Fact>]
    let ``Should compile all monsters properly`` () =
        async {
            FsHttp.initialiseMocking ()
            
            Cards.cardsList
            |> Seq.length
            |> should equal 450
        }

    [<Fact>]
    let ``Can hash team properly`` () =
        let summonerId = "C7-437-2RKXGKQGBK"
        let monsters = ["C1-21-X0ZLANHTDC";"C7-384-743030E5M8";"C7-387-016FOC2QK0";"C3-351-5ZLO7F2U0G";"G3-337-LUYJHI78KG"]
        let secret = "WzkHcJwuJQ"

        let hash = hashSelectedTeam summonerId monsters secret 

        hash |> should equal "3d4c998cecd04874da50d77758f09c4e"
