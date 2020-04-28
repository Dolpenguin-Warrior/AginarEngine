﻿#version 330 core

layout(location = 0) in int2 aPosition;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    
    
    texCoord = aTexCoord;

    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}