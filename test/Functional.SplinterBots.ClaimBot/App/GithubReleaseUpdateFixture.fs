namespace Functional.SplinterBots.Tests.API

open Xunit
open FsUnit.Xunit
open FSHttpMock
    
module GithubReleaseUpdateFixture = 
    [<Fact>]
    let ``Is able to parse version`` () =
        let input = "2021.52.123"
        let version = Version.bind input 

        version.year |> should equal 2021
        version.week |> should equal 52
        version.build |> should equal 123

    [<Fact>]
    let ``Version parsing can cope with small build `` () =
        let input = "2021.52.1"
        let version = Version.bind input 

        version.year |> should equal 2021
        version.week |> should equal 52
        version.build |> should equal 1

    [<Fact>]
    let ``Is able to detect incorect version`` () =
        let input = "1980.I.Will.Fail."
        let version = Version.bind input 

        version.year |> should equal 1
        version.week |> should equal 1

    [<Fact>]
    let ``Get GitHub release info`` () = 
        async {
            FsHttp.initialiseMocking ()

            let! release  = GitHubReleaseUpdate.getLatestGithubVersion ()
            release.Value.name
            |> should not' (equal "empty")
        }

    [<Fact>]
    let ``Can extract asset url`` () = 
        async {
            FsHttp.initialiseMocking ()

            let! release  = GitHubReleaseUpdate.getLatestGithubVersion ()
            let asset = GitHubReleaseUpdate.getProperAssetUrl release.Value
            asset
            |> should not' (equal GitHubReleaseUpdate.notSupported)
        }
