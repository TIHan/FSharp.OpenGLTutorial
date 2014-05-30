open FSharp.Game.Math
open Ferop
open Tutorial4

open System.Runtime.InteropServices
open System.IO
open System.Text

#if DEBUG
type Native = FeropProvider<"Tutorial4.Native", "bin/Debug">
type Common = FeropProvider<"Common", "bin/Debug">
#else
type Native = FeropProvider<"Tutorial4.Native", "bin/Release">
type Common = FeropProvider<"Common", "bin/Release">
#endif

let loadShaders () =
    let vertexSource = Encoding.ASCII.GetBytes (File.ReadAllText "TransformVertexShader.vertexshader")
    let fragmentSource = Encoding.ASCII.GetBytes (File.ReadAllText "ColorFragmentShader.fragmentshader")

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

let colorData =
    [|0.583f;  0.771f;  0.014f;
    0.609f;  0.115f;  0.436f;
    0.327f;  0.483f;  0.844f;
    0.822f;  0.569f;  0.201f;
    0.435f;  0.602f;  0.223f;
    0.310f;  0.747f;  0.185f;
    0.597f;  0.770f;  0.761f;
    0.559f;  0.436f;  0.730f;
    0.359f;  0.583f;  0.152f;
    0.483f;  0.596f;  0.789f;
    0.559f;  0.861f;  0.639f;
    0.195f;  0.548f;  0.859f;
    0.014f;  0.184f;  0.576f;
    0.771f;  0.328f;  0.970f;
    0.406f;  0.615f;  0.116f;
    0.676f;  0.977f;  0.133f;
    0.971f;  0.572f;  0.833f;
    0.140f;  0.616f;  0.489f;
    0.997f;  0.513f;  0.064f;
    0.945f;  0.719f;  0.592f;
    0.543f;  0.021f;  0.978f;
    0.279f;  0.317f;  0.505f;
    0.167f;  0.620f;  0.077f;
    0.347f;  0.857f;  0.137f;
    0.055f;  0.953f;  0.042f;
    0.714f;  0.505f;  0.345f;
    0.783f;  0.290f;  0.734f;
    0.722f;  0.645f;  0.174f;
    0.302f;  0.455f;  0.848f;
    0.225f;  0.587f;  0.040f;
    0.517f;  0.713f;  0.338f;
    0.053f;  0.959f;  0.120f;
    0.393f;  0.621f;  0.362f;
    0.673f;  0.211f;  0.457f;
    0.820f;  0.883f;  0.371f;
    0.982f;  0.099f;  0.879f|]

let elementData = [|0us; 1us; 2us|]

[<EntryPoint>]
let main argv = 
    let app = Native.App.init ("Tutorial4 - Colored Cube", 1024, 768)

    Native.App.clearColor app

    let vao = Native.App.generateVAO app
    let vbo = generateVBO triangleData app
    let colorVbo = generateVBO colorData app

    let programId = loadShaders ()

    let mvpId = Native.App.uniformMVP programId

    Native.App.useProgram (programId, app)

    let projection = Mat4.createPerspective 45.f<deg> (4.f / 3.f) 0.1f 100.f |> Mat4.transpose 
    let view = Mat4.lookAt (vec3 (4.f, 3.f, -3.f)) (vec3 0.f) (vec3 (0.f, 1.f, 0.f)) |> Mat4.transpose 
    let model = Mat4.identity
    let mvp = projection * view * model |> Mat4.transpose

    while not <| Native.App.shouldQuit app do
        Native.App.clear app
        Native.App.depthTest ()

        enableUniformMVP mvpId mvp
        Native.App.drawColorVBO (colorVbo, app)
        Native.App.drawVBO (vbo, app)
        Native.App.disableVertexAttribs ()

        Native.App.swap app

    Native.App.exit (app)




