namespace Tutorial4

open Ferop.Code

[<Struct>]
type Application =
    val Window : nativeint
    val GLContext : nativeint

[<Ferop>]
[<ClangFlagsOsx ("-DGL_GLEXT_PROTOTYPES")>]
[<ClangLibsOsx ("-framework Cocoa -framework OpenGL -framework IOKit -framework SDL2")>]
[<Include ("<stdio.h>")>]
[<Include ("<SDL2/SDL.h>")>]
[<Include ("<SDL2/SDL_opengl.h>")>]
module App =
    let init (title: string) (screenWidth: int) (screenHeight: int) : Application =
        C """
SDL_Init (SDL_INIT_VIDEO);

App_Application app;

app.Window = 
    SDL_CreateWindow(
        title,
        SDL_WINDOWPOS_UNDEFINED,
        SDL_WINDOWPOS_UNDEFINED,
        screenWidth, screenHeight,
        SDL_WINDOW_OPENGL|SDL_WINDOW_RESIZABLE);

SDL_GL_SetAttribute (SDL_GL_CONTEXT_MAJOR_VERSION, 3);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_MINOR_VERSION, 2);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

app.GLContext = SDL_GL_CreateContext ((SDL_Window*)app.Window);
return app;
        """

    let exit (app: Application) : int =
        C """
SDL_GL_DeleteContext (app.GLContext);
SDL_DestroyWindow (app.Window);
SDL_Quit ();
return 0;
        """

    let clearColor (app: Application) : unit =
        C """
// Dark blue background
glClearColor (0.0f, 0.0f, 0.4f, 0.0f);
        """

    let swap (app: Application) : unit = 
        C """
SDL_GL_SwapWindow (app.Window);
        """

    let shouldQuit (app: Application) : bool =
        C """
SDL_Event e;
SDL_PollEvent (&e);

switch (e.key.keysym.sym)
{
case SDLK_ESCAPE: return true;
}

return e.type == SDL_QUIT;
        """

    let generateVAO (app: Application) : uint32 =
        C """
GLuint VertexArrayID;
glGenVertexArrays (1, &VertexArrayID);
glBindVertexArray (VertexArrayID);
return VertexArrayID;
        """

    let generateVBO (size: int) (data: nativeint) (app: Application) : uint32 =
        C """
GLuint vbo;
glGenBuffers(1, &vbo);
glBindBuffer(GL_ARRAY_BUFFER, vbo);
glBufferData(GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);
return vbo;
        """

    let clear (app: Application) : unit =
        C """
// Clear the screen
glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        """

    let depthTest () : unit =
        C """
// Enable depth test
glEnable(GL_DEPTH_TEST);
// Accept fragment if it closer to the camera than the former one
glDepthFunc(GL_LESS); 
    """

    let useProgram (programId: uint32) (app: Application) : unit =
        C """
// Use our shader
glUseProgram(programId);
        """

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
glDrawArrays(GL_TRIANGLES, 0, 12 * 3); // 3 indices starting at 0 -> 1 triangle
        """
    
    let drawColorVBO (vbo: uint32) (app: Application) : unit =
        C """
// 2nd attribute buffer : colors
glEnableVertexAttribArray(1);
glBindBuffer(GL_ARRAY_BUFFER, vbo);
glVertexAttribPointer(
    1,                                // attribute. No particular reason for 1, but must match the layout in the shader.
    3,                                // size
    GL_FLOAT,                         // type
    GL_FALSE,                         // normalized?
    0,                                // stride
    (void*)0                          // array buffer offset
);
        """

    let disableVertexAttribs () : unit =
        C """
glDisableVertexAttribArray(1);
glDisableVertexAttribArray(0);
        """

    let uniformMVP (programId: uint32) : uint32 =
        C """
// Get a handle for our "MVP" uniform
GLuint MatrixID = glGetUniformLocation(programId, "MVP");
return MatrixID;
        """

    let enableUniformMVP (mvpId: uint32) (mvp: nativeint) : unit =
        C """
// in the "MVP" uniform
glUniformMatrix4fv(mvpId, 1, GL_FALSE, mvp);
        """

