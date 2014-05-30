open Ferop
open Ferop.Code
open Tutorial1

#if DEBUG
type Native = FeropProvider<"Tutorial1.Native", "bin/Debug", Platform.Auto>
#else
type Native = FeropProvider<"Tutorial1.Native", "bin/Release", Platform.Auto>
#endif

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial1", 1024, 768)

    Native.App.clearColor app

    while not <| Native.App.shouldQuit app do
        Native.App.swap app

    Native.App.exit (app)

