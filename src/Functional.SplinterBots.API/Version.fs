module Version

open System.Reflection
open System.IO
open System.Text.RegularExpressions

[<Literal>]
let versionFileName = "SplinterBots.version.info"

type Version = 
    {
        year: int
        week: int
        build: int
    }

let bind input = 
    let validationPattern = "\\d{4}\\.\\d{2}.\\d*"
    let isValid = Regex.IsMatch(input, validationPattern)
    match isValid with 
    | true -> 
        let segments = input.Split('.')
        {
            year = int(segments.[0])
            week = int(segments.[1])
            build = int(segments.[2])
        }
    | _ -> 
        {
            year = 0001
            week = 01
            build = 01
        }

let isFirstVersionLarger first second = 
    let asSingleNumner version = 
        version.year * 10000 + version.week * 100 + version.build

    let firstNumber = asSingleNumner first
    let secondUnmber =asSingleNumner second

    firstNumber > secondUnmber

let toString version =
    $"{version.year}.{version.week}.{version.build}"

let getVersion () = 
    let info = File.ReadAllText "version.info"
    info.Trim () |> bind
