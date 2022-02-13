namespace Functional.SplinterBots.BattleAPI

module UltimateBattleAPI = 
    type BattleApiQuery = 
        {
            mana: int  
            rules: string 
            splinters: string[]
            //cards: Card[] 
            //quest: JToken
            //questLessDetails: JToken 
            username: string 
            secondTry: bool
        }
