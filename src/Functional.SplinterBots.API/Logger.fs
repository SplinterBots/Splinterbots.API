namespace Functional.SplinterBots.API

module Logger = 

    type Message = string

    type Logger<'Message> = 'Message -> Async<unit>
