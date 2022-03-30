namespace Functional.SplinterBots.API

module Splinters = 

    let private splinters = 
        [ 
            ("red", "fire")
            ("blue", "water")
            ("green", "earth")
            ("white", "life")
            ("black", "death")
            ("gold", "dragon")
        ]
        
    let getAllowedSplinters (inactiveSplinters: string) = 
        //let inactiveSplinters = inactiveSplinters.Split(',')
        let result = 
            splinters 
            |> Seq.filter (fun (color, _) -> not (inactiveSplinters.Contains(color)))
            |> Seq.map (fun (_, name) -> name)
            |> Array.ofSeq
        result

