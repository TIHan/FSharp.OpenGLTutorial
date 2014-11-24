#include "Shader.h" 

char VertexShaderErrorMessage[65536];
char FragmentShaderErrorMessage[65536];
char ProgramErrorMessage[65536];


FEROP_EXPORT uint32_t FEROP_DECL Shader_loadShaders (uint8_t* vertexSource, uint8_t* fragmentSource)
{

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
         
}
