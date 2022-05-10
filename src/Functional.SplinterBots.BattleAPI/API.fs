namespace Functional.SplinterBots.BattleAPI

module Urls =
    let battleUrl = "https://battle.splinterlands.com/battle/battle_tx"

module MD5 = 
    let generateMD5Hash (input: string) =
        use md5 = System.Security.Cryptography.MD5.Create()
        let inputBytes = System.Text.Encoding.ASCII.GetBytes input
        let hash = md5.ComputeHash inputBytes

        hash
        |> Seq.map (fun byte -> byte.ToString("x2"))
        |> String.concat ""
