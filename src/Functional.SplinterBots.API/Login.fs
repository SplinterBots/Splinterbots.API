namespace Functional.SplinterBots.API

module Login =

    open API         

    let private getUri name key = 
        let (ts, signature) = Signature.sign name key 
        let bid = "bid_" + Randomizer.generateRandomString 20
        let sid = "sid_" + Randomizer.generateRandomString 20
        let parameters = 
            $"name={name}&ref=&browser_id={bid}&session_id={sid}&sig={signature}&ts={ts}"
        getPlayerUri "login" parameters

    type Login =
        {
            timestamp: int64
            name: string
            token: string
        }

    let getToken name postingKey =
        async {
            let uri = getUri name postingKey
            let! login = executeApiCall<Login> uri

            return login.token
        }

    let valiadteToken playerName token =
        async {
            let paramteres =
                sprintf "token=%s&username=%s"
                    token
                    playerName
            let uri = getPlayerUri "messages" paramteres
            let! callResponse  = executeApiCallToString uri
            let doesContainsErrorMessage = callResponse.StartsWith("{\"error")

            let isValid = doesContainsErrorMessage = false
            return isValid
        }
