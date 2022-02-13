namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots.API
open System
    
module SignatureFixture = 
    let signKey = "5HzkDhZJ34xiH5HxsH5EZxm3NvMhsXzUvi8LZhj5M2RhpkP3zMB"
    let user = "test" 

    [<Fact>]
    let ``Return proper timestamp`` () =
        let (ts, _) = Signature.sign user signKey

        let timestampAsDate =
            let unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            unixTime.AddMilliseconds(float(ts))
            
        timestampAsDate.Date
        |> should equal (DateTime.Now.Date)
        

    [<Fact>]
    let ``Can sign message`` () = 
        let (_, signature) = Signature.sign user signKey

        signature.Length
        |> should equal 130
