namespace Common.Ferop

open Ferop

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
[<Source ("""
char VertexShaderErrorMessage[65536];
char FragmentShaderErrorMessage[65536];
char ProgramErrorMessage[65536];
""")>]
module Shader =
    [<Import; MI (MIO.NoInlining)>]
    let loadShaders (vertexSource: byte []) (fragmentSource: byte []) : uint32 =
        C """
// Create the shaders
GLuint VertexShaderID = glCreateShader(GL_VERTEX_SHADER);
GLuint FragmentShaderID = glCreateShader(GL_FRAGMENT_SHADER);

GLint Result = GL_FALSE;
int InfoLogLength;



// Compile Vertex Shader
glShaderSource(VertexShaderID, 1, &vertexSource, NULL);
glCompileShader(VertexShaderID);

// Check Vertex Shader
glGetShaderiv(VertexShaderID, GL_COMPILE_STATUS, &Result);
glGetShaderiv(VertexShaderID, GL_INFO_LOG_LENGTH, &InfoLogLength);
if ( InfoLogLength > 0 ){
    glGetShaderInfoLog(VertexShaderID, InfoLogLength, NULL, &VertexShaderErrorMessage[0]);
    printf("%s\n", &VertexShaderErrorMessage[0]);
    for (int i = 0; i < 65536; ++i) { VertexShaderErrorMessage[i] = '\0'; }
}



// Compile Fragment Shader
glShaderSource(FragmentShaderID, 1, &fragmentSource, NULL);
glCompileShader(FragmentShaderID);

// Check Fragment Shader
glGetShaderiv(FragmentShaderID, GL_COMPILE_STATUS, &Result);
glGetShaderiv(FragmentShaderID, GL_INFO_LOG_LENGTH, &InfoLogLength);
if ( InfoLogLength > 0 ){
    glGetShaderInfoLog(FragmentShaderID, InfoLogLength, NULL, &FragmentShaderErrorMessage[0]);
    printf("%s\n", &FragmentShaderErrorMessage[0]);
    for (int i = 0; i < 65536; ++i) { FragmentShaderErrorMessage[i] = '\0'; }
}



// Link the program
printf("Linking program\n");
GLuint ProgramID = glCreateProgram();
glAttachShader(ProgramID, VertexShaderID);
glAttachShader(ProgramID, FragmentShaderID);
glLinkProgram(ProgramID);

// Check the program
glGetProgramiv(ProgramID, GL_LINK_STATUS, &Result);
glGetProgramiv(ProgramID, GL_INFO_LOG_LENGTH, &InfoLogLength);
if ( InfoLogLength > 0 ){
    glGetProgramInfoLog(ProgramID, InfoLogLength, NULL, &ProgramErrorMessage[0]);
    printf("%s\n", &ProgramErrorMessage[0]);
    for (int i = 0; i < 65536; ++i) { ProgramErrorMessage[i] = '\0'; }
}

glDeleteShader(VertexShaderID);
glDeleteShader(FragmentShaderID);

return ProgramID;
        """

[<Struct>]
type Application =
    val Window : nativeint
    val GLContext : nativeint

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
module App =
    [<Import; MI (MIO.NoInlining)>]
    let init (title: sbyte []) (screenWidth: int) (screenHeight: int) : Application =
        C """
SDL_Init (SDL_INIT_VIDEO);

App_Application app;

app.Window = 
    SDL_CreateWindow(
        "Ferop.Sample",
        SDL_WINDOWPOS_UNDEFINED,
        SDL_WINDOWPOS_UNDEFINED,
        900, 900,
        SDL_WINDOW_OPENGL);

SDL_GL_SetAttribute (SDL_GL_CONTEXT_MAJOR_VERSION, 3);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_MINOR_VERSION, 2);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

app.GLContext = SDL_GL_CreateContext ((SDL_Window*)app.Window);
SDL_GL_SetSwapInterval (0);

#if defined(__GNUC__)
#else
glewExperimental = GL_TRUE;
glewInit ();
#endif

return app;
        """

    [<Import; MI (MIO.NoInlining)>]
    let exit (app: Application) : int =
        C """
SDL_GL_DeleteContext (app.GLContext);
SDL_DestroyWindow (app.Window);
SDL_Quit ();
return 0;
        """

    [<Import; MI (MIO.NoInlining)>]
    let clearColor (app: Application) : unit =
        C """
// Dark blue background
glClearColor (0.0f, 0.0f, 0.4f, 0.0f);
        """

    [<Import; MI (MIO.NoInlining)>]
    let swap (app: Application) : unit = 
        C """
SDL_GL_SwapWindow (app.Window);
        """

    [<Import; MI (MIO.NoInlining)>]
    let shouldQuit (app: Application) : int =
        C """
SDL_Event e;
SDL_PollEvent (&e);

switch (e.key.keysym.sym)
{
case SDLK_ESCAPE: return GL_TRUE;
}

return e.type == SDL_QUIT;
        """
