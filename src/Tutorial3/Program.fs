open FSharp.Game.Math
open Ferop
open Tutorial3

open System.Runtime.InteropServices
open System.IO
open System.Text

#if DEBUG
type Native = FeropProvider<"Tutorial3.Native", "bin/Debug">
type Common = FeropProvider<"Common", "bin/Debug">
#else
type Native = FeropProvider<"Tutorial3.Native", "bin/Release">
type Common = FeropProvider<"Common", "bin/Release">
#endif

let loadShaders () =
    let vertexSource = Encoding.ASCII.GetBytes (File.ReadAllText "SimpleTransform.vertexshader")
    let fragmentSource = Encoding.ASCII.GetBytes (File.ReadAllText "SingleColor.fragmentshader")

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

let enableUniformMVP (mvpId: uint32) (mvp: mat4) =
    let dh = GCHandle.Alloc (mvp, GCHandleType.Pinned)
    let dp = dh.AddrOfPinnedObject ()

    Native.App.enableUniformMVP (mvpId, dp)

    dh.Free ()    

let triangleData = 
    [|
    -1.0f; -1.0f; 0.0f;
     1.0f; -1.0f; 0.0f;
     0.0f;  1.0f; 0.0f|]

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial3 - Matrices", 1024, 768)

    Native.App.clearColor app

    let vao = Native.App.generateVAO app
    let vbo = generateVBO triangleData app

    let programId = loadShaders ()

    let mvpId = Native.App.uniformMVP programId

    Native.App.useProgram (programId, app)

    let projection = Mat4.createPerspective 45.f<deg> (4.f / 3.f) 0.1f 100.f |> Mat4.transpose 
    let view = Mat4.lookAt (vec3 (4.f, 3.f, 3.f)) (vec3 0.f) (vec3 (0.f, 1.f, 0.f)) |> Mat4.transpose 
    let model = Mat4.identity
    let mvp = projection * view * model |> Mat4.transpose

    while not <| Native.App.shouldQuit app do
        Native.App.clear app

        enableUniformMVP mvpId mvp
        Native.App.drawVBO (vbo, app)

        Native.App.swap app

    Native.App.exit (app)



