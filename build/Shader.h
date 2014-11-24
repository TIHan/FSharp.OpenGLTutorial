
#ifndef __SHADER_H__
#define __SHADER_H__

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



FEROP_EXPORT uint32_t FEROP_DECL Shader_loadShaders (uint8_t*, uint8_t*);



#endif
