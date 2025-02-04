﻿namespace Functional.SplinterBots.API

module Login =

    open Http         

    let private getUri name key = 
        let (ts, signature) = Signature.sign name key 
        let bid = "bid_" + Randomizer.generateRandomString 20
        let sid = "sid_" + Randomizer.generateRandomString 20
        let parameters = 
            $"name={name}&ref=&browser_id={bid}&session_id={sid}&sig={signature}&ts={ts}"
        Urls.getPlayerUri "login" parameters

    type Login =
        {
            timestamp: int64
            name: string
            token: string
        }

    let getToken name postingKey =
        async {
            let uri = getUri name postingKey
            let! login = Http.executeApiCall<Login> uri

            return login.token
        }

    let valiadteToken playerName token =
        async {
            let paramteres =
                sprintf "token=%s&username=%s"
                    token
                    playerName
            let uri = Urls.getPlayerUri "messages" paramteres
            let! callResponse  = Http.executeApiCallToString uri
            let doesContainsErrorMessage = callResponse.StartsWith("{\"error")

            let isValid = doesContainsErrorMessage = false
            return isValid
        }
