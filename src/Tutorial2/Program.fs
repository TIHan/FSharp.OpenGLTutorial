open System.Runtime.InteropServices
open System.IO
open System.Text

open FSharp.Game.Math

open Ferop
open Common.Ferop

[<Ferop>]
[<ClangOsx (
    "-DGL_GLEXT_PROTOTYPES -I/Library/Frameworks/SDL2.framework/Headers",
    "-F/Library/Frameworks -framework Cocoa -framework OpenGL -framework IOKit -framework SDL2"
)>]
[<GccLinux ("-I../../include/SDL2", "-lSDL2")>]
#if __64BIT__
[<MsvcWin (""" /I ..\..\include\SDL2 /I ..\..\include ..\..\lib\win\x64\SDL2.lib ..\..\lib\win\x64\SDL2main.lib ..\..\lib\win\x64\glew32.lib opengl32.lib """)>]
#else
[<MsvcWin (""" /I ..\..\include\SDL2 /I ..\..\include ..\..\lib\win\x86\SDL2.lib ..\..\lib\win\x86\SDL2main.lib ..\..\lib\win\x86\glew32.lib opengl32.lib """)>]
#endif
[<Header ("""
#include <stdio.h>
#if defined(__GNUC__)
#   include "SDL.h"
#   include "SDL_opengl.h"
#else
#   include "SDL.h"
#   include <GL/glew.h>
#   include <GL/wglew.h>
#endif
""")>]
module Tutorial2Native =
    [<Import; MI (MIO.NoInlining)>]
    let generateVAO (app: Application) : uint32 =
        C """
GLuint VertexArrayID;
glGenVertexArrays (1, &VertexArrayID);
glBindVertexArray (VertexArrayID);
return VertexArrayID;
        """

    [<Import; MI (MIO.NoInlining)>]
    let generateVBO (size: int) (data: single []) (app: Application) : uint32 =
        C """
GLuint vbo;
glGenBuffers (1, &vbo);
glBindBuffer (GL_ARRAY_BUFFER, vbo);
glBufferData (GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);
return vbo;
        """

    [<Import; MI (MIO.NoInlining)>]
    let clear (app: Application) : unit =
        C """
// Clear the screen
glClear( GL_COLOR_BUFFER_BIT );
        """

    [<Import; MI (MIO.NoInlining)>]
    let useProgram (programId: uint32) (app: Application) : unit =
        C """
// Use our shader
glUseProgram(programId);
        """

    [<Import; MI (MIO.NoInlining)>]
    let drawVBO (vbo: uint32) (app: Application) : unit =
        C """
// 1rst attribute buffer : vertices
glEnableVertexAttribArray(0);
glBindBuffer(GL_ARRAY_BUFFER, vbo);
glVertexAttribPointer(
    0,                  // attribute 0. No particular reason for 0, but must match the layout in the shader.
    3,                  // size
    GL_FLOAT,           // type
    GL_FALSE,           // normalized?
    0,                  // stride
    (void*)0            // array buffer offset
);

// Draw the triangle !
glDrawArrays(GL_TRIANGLES, 0, 3); // 3 indices starting at 0 -> 1 triangle

glDisableVertexAttribArray(0);
        """

let loadShaders () =
    let vertexSource = Encoding.ASCII.GetBytes (File.ReadAllText "SimpleVertexShader.vertexshader")
    let fragmentSource = Encoding.ASCII.GetBytes (File.ReadAllText "SimpleFragmentShader.fragmentshader")

    // Create and compile our GLSL program from the shaders
    Shader.loadShaders vertexSource fragmentSource

let generateVBO (data: single[]) (app: Application) =
    Tutorial2Native.generateVBO (sizeof<single> * data.Length) data app

let triangleData = 
    [|
    -1.0f; -1.0f; 0.0f;
     1.0f; -1.0f; 0.0f;
     0.0f;  1.0f; 0.0f|]

[<EntryPoint>]
let main argv = 
    let app = App.init ("Tutorial2 - Red Triangle".ToCharArray() |> Array.map (fun x -> sbyte x)) 1024 768

    App.clearColor app

    let vao = Tutorial2Native.generateVAO app
    let vbo = generateVBO triangleData app

    let programId = loadShaders ()
    Tutorial2Native.useProgram programId app

    while App.shouldQuit app = 0 do
        Tutorial2Native.clear app
        Tutorial2Native.drawVBO vbo app

        App.swap app

    App.exit (app)


