
#ifndef __APP_H__
#define __APP_H__

#include <stdint.h>

#if defined(_WIN32)
#   define FEROP_IMPORT       __declspec(dllimport)
#   define FEROP_EXPORT       __declspec(dllexport)
#   define FEROP_DECL         __cdecl
#elif defined(__GNUC__)
#   define FEROP_EXPORT       __attribute__((visibility("default")))
#   define FEROP_IMPORT
#   define FEROP_DECL         __attribute__((cdecl))
#else
#   error Compiler not supported.
#endif


#include <stdio.h>
#if defined(__GNUC__)
#   include "SDL.h"
#   include "SDL_opengl.h"
#else
#   include "SDL.h"
#   include <GL/glew.h>
#   include <GL/wglew.h>
#endif



typedef struct {
void* Window;
void* GLContext;

} App_Application;


FEROP_EXPORT App_Application FEROP_DECL App_init (int8_t*, int32_t, int32_t);


FEROP_EXPORT int32_t FEROP_DECL App_exit (App_Application);


FEROP_EXPORT void FEROP_DECL App_clearColor (App_Application);


FEROP_EXPORT void FEROP_DECL App_swap (App_Application);


FEROP_EXPORT int32_t FEROP_DECL App_shouldQuit (App_Application);



#endif
