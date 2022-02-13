namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open Functional.SplinterBots
open Functional.SplinterBots.API
open HttpClientMock
open FSHttpMock
open ResourceLoader
open System

module SettingsFixture =
    [<Fact>]
    let ``Can get settings id`` () =
        async {
            FsHttp.initialiseMocking ()
            let! settings = API.Settings.getSettings ()

            settings.season.id 
            |> should equal 77
        }

    [<Fact>]
    let ``Can get settings end date`` () =
        async {
            FsHttp.initialiseMocking ()
            let! settings = API.Settings.getSettings ()

            settings.season.ends 
            |> should equal (DateTime.Parse "2021-12-31T02:00:00.000Z")
        }


