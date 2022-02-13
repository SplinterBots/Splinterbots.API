namespace  Functional.SplinterBots.Tests

module SplinterBotsMathFixture =

    open Xunit
    open FsUnit.Xunit
    open Functional.SplinterBots

    let roundValues : obj[] seq =
        seq {
            yield [| 100M; 100M |]
            yield [| 100.123; 100.123 |]
            yield [| 100.12356789; 100.123 |]
            yield [| 0.01001; 0.010 |]
            yield [| 0.99099; 0.990 |]
        }
    [<Theory>]
    [<MemberData("roundValues")>]
    let ``Given decimal amount When round Then number is always nmo smaller than 3 numbers after dot `` (number: decimal) (expectedResult: decimal) =
        number
        |> SplinterBotsMath.round
        |> should equal expectedResult

    let devValues : obj[] seq =
        seq {
            yield [| 0.01; None  |]
            yield [| 12; Some 0.6M |]
            yield [| 40; Some 2M |]
            yield [| 50; Some 2.0M |]
            yield [| 55; Some 2.2M |]
            yield [| 65; Some 2.6M |]
            yield [| 85; Some 3.4M |]
            yield [| 100; Some 3M |]
            yield [| 105; Some 3.15M |]
            yield [| 155; Some 3.1M |]
            yield [| 205; Some 2.05M |]
            yield [| 255; Some 2.55M |]
            yield [| 305; Some 3.05M |]
        }
    [<Theory>]
    [<MemberData("devValues")>]
    let ``Given DEC Amout when calculate donation level THEN value  is progressive`` (number: decimal) (expectedResult: decimal option) =
        number
        |> SplinterBotsMath.calculateDecDonation
        |> should equal expectedResult

    let spsValues : obj[] seq =
        seq {
            yield [| 0.09; None |]
            yield [| 0.1; Some 0.05M |]
            yield [| 0.5; Some 0.05M |]
            yield [| 1; Some 0.05M |]
            yield [| 4; Some 0.2M|]
            yield [| 8; Some 0.4M|]
            yield [| 12; Some 0.6M|]
            yield [| 16; Some 0.8M|]
            yield [| 21; Some 1.05M|]
            yield [| 30; Some 1.5M|]

        }
    [<Theory>]
    [<MemberData("spsValues")>]
    let ``Given SPS Amout when calculate donation level THEN value  is progressive`` (number: decimal) (expectedResult: decimal option) =
        number
        |> SplinterBotsMath.calcualteSPSDonation
        |> should equal expectedResult
