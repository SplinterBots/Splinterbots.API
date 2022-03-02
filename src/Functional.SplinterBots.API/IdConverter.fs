namespace Functional.SplinterBots.API

module IdConverter =
    let bindMultipleCardsId ids =
        let concatenatedIDds =
            ids
            |> Seq.map (fun id -> $"\"{id}\"")
            |> String.concat ","
        $"{concatenatedIDds}"

    let bindSingleId id = 
        $"\"{id}\""
