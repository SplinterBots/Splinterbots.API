namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots.API
open System
open FSHttpMock
    
module LoginFixture = 
    let signKey = "5HzkDhZJ34xiH5HxsH5EZxm3NvMhsXzUvi8LZhj5M2RhpkP3zMB"
    let user = "test" 

    [<Fact>]
    let ``Can obtain token`` () =
        async {
            FsHttp.initialiseMocking ()
            let! token = Login.getToken user signKey
            
            token
            |> should equal "ABCDEF1234"
        }

    [<Fact>]
    let ``Ensure that token is valid`` () =
        async {
            FsHttp.initialiseMocking ()
            let! isValid = Login.valiadteToken user "ABCDEF1234"
            
            isValid
            |> should equal true
        }

    [<Fact>]
    let ``Ensure that token is not valid`` () =
        async {
            FsHttp.initialiseMocking ()
            let! isValid = Login.valiadteToken user "NOTVALID"
            
            isValid
            |> should equal false
        }
