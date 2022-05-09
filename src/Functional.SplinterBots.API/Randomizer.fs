namespace Functional.SplinterBots.API


module Randomizer =
    
    open System

    let generateRandomString numebrOfCharacters = 
        let randomizer = Random()
        let chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray()
        let sz = Array.length chars in
        String(Array.init numebrOfCharacters (fun _ -> chars.[randomizer.Next sz])).ToString()
