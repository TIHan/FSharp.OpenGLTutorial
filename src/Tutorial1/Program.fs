open Tutorial1

[<EntryPoint>]
let main argv = 
    let app = App.init ("Tutorial1".ToCharArray() |> Array.map (fun x -> sbyte x)) 1024 768

    App.clearColor app

    while App.shouldQuit app = 0 do
        App.swap app

    App.exit (app)

