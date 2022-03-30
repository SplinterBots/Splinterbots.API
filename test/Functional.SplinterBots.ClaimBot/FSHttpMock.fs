module FSHttpMock 

open System.Net
open System.Net.Http
open System.Threading.Tasks
open System.Text.RegularExpressions

type UriMatch = string 
type Content = string

type DelegationHandlerStub (handlers: Map<UriMatch, Content>) as self = 
    inherit DelegatingHandler ()

    //let mutable handlers = Map.empty<UriMatch, Content>

    let getContent (uri: string) =
        let content = 
            let matchingKey = 
                handlers 
                |> Map.findKey (fun pattern item -> uri.Contains pattern)
            handlers.[matchingKey]
        content

    do
        self.InnerHandler <- new HttpClientHandler()

    override this.Send (request, cancellationToken) =
        let requestUrl = request.RequestUri
        let content = getContent requestUrl.PathAndQuery
        let response = new HttpResponseMessage(HttpStatusCode.OK)
        response.Content <- new StringContent(content)
        response

    override this.SendAsync (request, cancellationToken) = 
        let result = this.Send (request, cancellationToken)
        Task.FromResult(result)

module FsHttp =
    let definitions = 
        [
            "/settings", "API.Settings.json"
            "/settings", "API.Settings.json"
            "/players/leaderboard", "API.Leaderboard.json"
            "/players/leaderboard", "API.Leaderboard.json"
            "/players/login", "API.Login.json"
            "/players/messages?token=ABCDEF1234", "API.Messages.ABCDEF1234.json"
            "/players/messages?token=NOTVALID", "API.Messages.NOTVALID.json"
            "/battle/history?player=test_player", "API.Battle.json"
            "/repos/functional-solutions/Splinterbots/releases", "API.GitHubReleases.json"
            //"/cards/get_details", "Core.Cards.Cards.json"
        ] 
    let initialiseMocking () =
        let handlers = 
            definitions
            |> Map.ofList
            |> Map.map (fun key item -> ResourceLoader.embeddedResource item)
        let delegationHandler = new DelegationHandlerStub(handlers)

        let httpClient = new HttpClient(delegationHandler)
        FsHttp.Config.setDefaultConfig(fun config -> 
            { config with httpClientFactory = Some (fun () -> httpClient) })
