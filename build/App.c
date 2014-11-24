#include "App.h" 


FEROP_EXPORT App_Application FEROP_DECL App_init (int8_t* title, int32_t screenWidth, int32_t screenHeight)
{

SDL_Init (SDL_INIT_VIDEO);

App_Application app;

app.Window = 
    SDL_CreateWindow(
        (const char*)title,
        SDL_WINDOWPOS_UNDEFINED,
        SDL_WINDOWPOS_UNDEFINED,
        screenWidth, screenHeight,
        SDL_WINDOW_OPENGL|SDL_WINDOW_RESIZABLE);

SDL_GL_SetAttribute (SDL_GL_CONTEXT_MAJOR_VERSION, 3);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_MINOR_VERSION, 2);
SDL_GL_SetAttribute (SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

app.GLContext = SDL_GL_CreateContext ((SDL_Window*)app.Window);
return app;
         
}


FEROP_EXPORT int32_t FEROP_DECL App_exit (App_Application app)
{

SDL_GL_DeleteContext (app.GLContext);
SDL_DestroyWindow (app.Window);
SDL_Quit ();
return 0;
         
}


FEROP_EXPORT void FEROP_DECL App_clearColor (App_Application app)
{

// Dark blue background
glClearColor (0.0f, 0.0f, 0.4f, 0.0f);
         
}


FEROP_EXPORT void FEROP_DECL App_swap (App_Application app)
{

SDL_GL_SwapWindow (app.Window);
         
}


FEROP_EXPORT int32_t FEROP_DECL App_shouldQuit (App_Application app)
{

SDL_Event e;
SDL_PollEvent (&e);

switch (e.key.keysym.sym)
{
case SDLK_ESCAPE: return GL_TRUE;
}

return e.type == SDL_QUIT;
         
}
