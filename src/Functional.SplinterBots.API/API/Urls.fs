namespace Functional.SplinterBots.API

module Urls =
    let applicationIdentifier = "splinterlands/0.7.139"
    let hiveApi = "anyx.io"
    let hiveNodeUrl = $"https://{hiveApi}"

    let api2Uri = 
        "https://api2.splinterlands.com"
    let cacheApiUri action queryParameters= 
        $"https://cache-api.splinterlands.com/{action}?{queryParameters}"
    let gameApiUri action =
        $"https://game-api.splinterlands.com/{action}" 
        
    let getPlayerUri action queryParameters =
        $"{api2Uri}/players/{action}?{queryParameters}"
    let getCardsUri action = 
        $"{api2Uri}/cards/{action}"
    let getMarketUri action =
        $"{api2Uri}/market/{action}"
    let getBattleUri action queryParameters = 
        $"{api2Uri}/battle/{action}?{queryParameters}"
    let getECUri action queryParameters =
        $"https://ec-api.splinterlands.com/players/{action}?{queryParameters}"
    let battleUri  = "https://battle.splinterlands.com/battle/battle_tx"
