namespace Functional.SplinterBots.API

module Settings =
    
    open API
    open System

    type Setting = 
        {
            season: {| id: int; ends: DateTime |}
        }
    module Setting =
        let empty = 
            {
                season = {| id = 0; ends = DateTime.MinValue |}
            }

    let getSettings () = 
        async {
            let uri = Urls.gameApiUri "settings"
            return! executeApiCall<Setting> uri
        }
