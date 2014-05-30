open FSharp.Game.Math
open Ferop
open Ferop.Code
open Tutorial2

open System.Runtime.InteropServices
open System.IO
open System.Text

#if DEBUG
type Native = FeropProvider<"Tutorial2.Native", "bin/Debug", Platform.Auto>
type Common = FeropProvider<"Common", "bin/Debug", Platform.Auto>
#else
type Native = FeropProvider<"Tutorial2.Native", "bin/Release", Platform.Auto>
type Common = FeropProvider<"Common", "bin/Release", Platform.Auto>
#endif

let loadShaders () =
    let vertexSource = Encoding.ASCII.GetBytes (File.ReadAllText "SimpleVertexShader.vertexshader")
    let fragmentSource = Encoding.ASCII.GetBytes (File.ReadAllText "SimpleFragmentShader.fragmentshader")

    let vh = GCHandle.Alloc (vertexSource, GCHandleType.Pinned)
    let fh = GCHandle.Alloc (fragmentSource, GCHandleType.Pinned)

    let vp = vh.AddrOfPinnedObject ()
    let fp = fh.AddrOfPinnedObject ()

    // Create and compile our GLSL program from the shaders
    let programId = Common.Shader.loadShaders (vp, fp)

    vh.Free ()
    fh.Free ()

    programId

let generateVBO (data: single[]) (app: Application) =
    let dh = GCHandle.Alloc (data, GCHandleType.Pinned)
    let dp = dh.AddrOfPinnedObject ()

    let vbo = Native.App.generateVBO (sizeof<single> * data.Length, dp, app)

    dh.Free ()
    vbo

let triangleData = 
    [|
    -1.0f; -1.0f; 0.0f;
     1.0f; -1.0f; 0.0f;
     0.0f;  1.0f; 0.0f|]

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial2 - Red Triangle", 1024, 768)

    Native.App.clearColor app

    let vao = Native.App.generateVAO app
    let vbo = generateVBO triangleData app

    let programId = loadShaders ()
    Native.App.useProgram (programId, app)

    while not <| Native.App.shouldQuit app do
        Native.App.clear app
        Native.App.drawVBO (vbo, app)

        Native.App.swap app

    Native.App.exit (app)


