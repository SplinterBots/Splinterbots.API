namespace Functional.SplinterBots.API

module Logger = 

    type Message = string

    type Logger = string -> Async<unit>

    let logStatus (logger: Logger) item = 
        item.ToString() |> logger
