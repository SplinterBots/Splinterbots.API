namespace Functional.SplinterBots.API

module Http =

    open System
    open System.Text.Json
    open FsHttp
    open FsHttp.DslCE
    open FsHttp.Response

    let executeApiCallToString url  =
        async {
            let! response =
                httpAsync {
                    GET url
                    CacheControl "no-cache"
                    UserAgent "SplinterBots"
                }
            return response |> Response.toString Int32.MaxValue
        }

    let executeApiCall<'T> url  =
        async {
            let! response =
                httpAsync {                   
                    GET url
                    CacheControl "no-cache"
                    UserAgent "SplinterBots"
                }
                
            let! responseStream =  response |> toStreamAsync
            return! JsonSerializer.DeserializeAsync<'T>(responseStream).AsTask() |> Async.AwaitTask   
        }

    let executeApiPostCall<'T> url payload = 
        async {
            let! response =
                httpAsync {                   
                    POST url
                    UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0"
                    ContentType "application/x-www-form-urlencoded"                                      
                    body 
                    json payload
                }
            
            let! responseStream =  response |> toStreamAsync
            return! JsonSerializer.DeserializeAsync<'T>(responseStream).AsTask() |> Async.AwaitTask 
        }
