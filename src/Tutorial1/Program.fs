open Ferop
open Tutorial1

#if DEBUG
type Native = FeropProvider<"Tutorial1.Native", "bin/Debug">
#else
type Native = FeropProvider<"Tutorial1.Native", "bin/Release">
#endif

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial1", 1024, 768)

    Native.App.clearColor app

    while not <| Native.App.shouldQuit app do
        Native.App.swap app

    Native.App.exit (app)

