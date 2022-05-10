namespace Functional.SplinterBots.API

module Transfer =

    type Token =
        | SPS
        | DEC 

    let transferCurrency token amount destinationPlayerName playerName activeKey = 
        async {
            let transactionPayload  = 
                sprintf "{\"token\":\"%A\",\"qty\":%M,\"to\":\"%s\",\"memo\":\"%s\",\"app\":\"%s\",\"n\":\"%s\"}"
                    token
                    amount
                    destinationPlayerName
                    destinationPlayerName

            let operations = Hive.createCustomJsonActiveKey  playerName "sm_token_transfer" transactionPayload
            let txid = Hive.brodcastTransaction operations activeKey
            Hive.waitForTransaction playerName txid |> ignore
        }


    type Data =
        {
            from: string 
            ``to``: string 
            token: string 
            qty: double 
            platform: string 
            address: string 
            ts: int64 
            ``sig``: string 
        }

    type ClaimResult = 
        {
            success: bool
            tx: {| id: string |}
            data : Data
        }

    let claimSPS playerName (ts: int64) signature token = 
        async {
            let uri =
                let parameters = 
                    sprintf "platform=hive&address=%s&sig=%s&ts=%i&token=%s&v=%s&username=%s"
                        playerName
                        signature
                        ts
                        token
                        token
                        playerName
                Urls.getECUri "claim_sps_airdrop" parameters
            let! claimData = Http.executeApiCall<ClaimResult> uri
            ()
        }
