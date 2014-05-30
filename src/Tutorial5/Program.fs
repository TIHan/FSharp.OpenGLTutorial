open FSharp.Game.Math
open Ferop
open Tutorial5

open System.Runtime.InteropServices
open System.IO
open System.Text

#if DEBUG
type Native = FeropProvider<"Tutorial5.Native", "bin/Debug">
type Common = FeropProvider<"Common", "bin/Debug">
#else
type Native = FeropProvider<"Tutorial5.Native", "bin/Release">
type Common = FeropProvider<"Common", "bin/Release">
#endif

let loadShaders () =
    let vertexSource = Encoding.ASCII.GetBytes (File.ReadAllText "TransformVertexShader.vertexshader")
    let fragmentSource = Encoding.ASCII.GetBytes (File.ReadAllText "TextureFragmentShader.fragmentshader")

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

let generateTexture width height (data: byte[]) =
    let dh = GCHandle.Alloc (data, GCHandleType.Pinned)
    let dp = dh.AddrOfPinnedObject ()

    let vbo = Native.App.generateTexture (width, height, dp)

    dh.Free ()
    vbo

let enableUniformMVP (mvpId: uint32) (mvp: mat4) =
    let dh = GCHandle.Alloc (mvp, GCHandleType.Pinned)
    let dp = dh.AddrOfPinnedObject ()

    Native.App.enableUniformMVP (mvpId, dp)

    dh.Free ()    

let triangleData = 
    [|
    -1.0f;-1.0f;-1.0f; // triangle 1 : begin
    -1.0f;-1.0f; 1.0f;
    -1.0f; 1.0f; 1.0f; // triangle 1 : end
    1.0f; 1.0f;-1.0f; // triangle 2 : begin
    -1.0f;-1.0f;-1.0f;
    -1.0f; 1.0f;-1.0f; // triangle 2 : end
    1.0f;-1.0f; 1.0f;
    -1.0f;-1.0f;-1.0f;
    1.0f;-1.0f;-1.0f;
    1.0f; 1.0f;-1.0f;
    1.0f;-1.0f;-1.0f;
    -1.0f;-1.0f;-1.0f;
    -1.0f;-1.0f;-1.0f;
    -1.0f; 1.0f; 1.0f;
    -1.0f; 1.0f;-1.0f;
    1.0f;-1.0f; 1.0f;
    -1.0f;-1.0f; 1.0f;
    -1.0f;-1.0f;-1.0f;
    -1.0f; 1.0f; 1.0f;
    -1.0f;-1.0f; 1.0f;
    1.0f;-1.0f; 1.0f;
    1.0f; 1.0f; 1.0f;
    1.0f;-1.0f;-1.0f;
    1.0f; 1.0f;-1.0f;
    1.0f;-1.0f;-1.0f;
    1.0f; 1.0f; 1.0f;
    1.0f;-1.0f; 1.0f;
    1.0f; 1.0f; 1.0f;
    1.0f; 1.0f;-1.0f;
    -1.0f; 1.0f;-1.0f;
    1.0f; 1.0f; 1.0f;
    -1.0f; 1.0f;-1.0f;
    -1.0f; 1.0f; 1.0f;
    1.0f; 1.0f; 1.0f;
    -1.0f; 1.0f; 1.0f;
    1.0f;-1.0f; 1.0f|]

let uvTextureData = 
    [|0.000059f; 0.000004f;
    0.000103f; 0.336048f;
    0.335973f; 0.335903f;
    1.000023f; 0.000013f;
    0.667979f; 0.335851f;
    0.999958f; 0.336064f;
    0.667979f; 0.335851f;
    0.336024f; 0.671877f;
    0.667969f; 0.671889f;
    1.000023f; 0.000013f;
    0.668104f; 0.000013f;
    0.667979f; 0.335851f;
    0.000059f; 0.000004f;
    0.335973f; 0.335903f;
    0.336098f; 0.000071f;
    0.667979f; 0.335851f;
    0.335973f; 0.335903f;
    0.336024f; 0.671877f;
    1.000004f; 0.671847f;
    0.999958f; 0.336064f;
    0.667979f; 0.335851f;
    0.668104f; 0.000013f;
    0.335973f; 0.335903f;
    0.667979f; 0.335851f;
    0.335973f; 0.335903f;
    0.668104f; 0.000013f;
    0.336098f; 0.000071f;
    0.000103f; 0.336048f;
    0.000004f; 0.671870f;
    0.336024f; 0.671877f;
    0.000103f; 0.336048f;
    0.336024f; 0.671877f;
    0.335973f; 0.335903f;
    0.667969f; 0.671889f;
    1.000004f; 0.671847f;
    0.667979f; 0.335851f|]

let elementData = [|0us; 1us; 2us|]

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial5 - Textured Cube", 1024, 768)

    Native.App.clearColor app

    let vao = Native.App.generateVAO app
    let vbo = generateVBO triangleData app
    let uv = generateVBO uvTextureData app

    let bmp = new Gdk.Pixbuf ("uvtemplate.tga")
    let textureId = Native.App.generateTexture (bmp.Width, bmp.Height, bmp.Pixels)

    let programId = loadShaders ()

    let mvpId = Native.App.uniformMVP programId
    let myTextureSamplerId = Native.App.uniform (programId, "myTextureSampler")

    Native.App.useProgram (programId, app)

    let projection = Mat4.createPerspective 45.f<deg> (4.f / 3.f) 0.1f 100.f |> Mat4.transpose 
    let view = Mat4.lookAt (vec3 (4.f, 3.f, 3.f)) (vec3 0.f) (vec3 (0.f, 1.f, 0.f)) |> Mat4.transpose 
    let model = Mat4.identity
    let mvp = projection * view * model |> Mat4.transpose

    while not <| Native.App.shouldQuit app do
        Native.App.clear app
        Native.App.depthTest ()

        enableUniformMVP mvpId mvp
        Native.App.bindTexture (textureId, myTextureSamplerId)
        Native.App.drawUV (uv, app)
        Native.App.drawVBO (vbo, app)
        Native.App.disableVertexAttribs ()

        Native.App.swap app

    Native.App.exit (app)




    