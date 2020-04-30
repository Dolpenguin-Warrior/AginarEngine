#version 330 core

layout(location = 0) in uvec2 aPosition;

out vec2 texCoord;
flat out ivec4 lighting;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    vec4 position;

    position.x = aPosition.x & 63u;
    position.y = (aPosition.x >> 6u) & 63u;
    position.z = ((aPosition.x >> 12u) & 63u);
    position.w = 1;

    lighting = ivec4((aPosition.x >> 19u) & 15u, (aPosition.x >> 23u) & 15u, (aPosition.x >> 27u) & 15u, 1u);

    uint blockType = aPosition.y  & 65535u;

    uint rotation = (aPosition.y >> 19u) & 15u;

    uint normal  = (aPosition.y >> 16u) & 7u;
    

    texCoord = vec2(0, 0);

    gl_Position = position * model * view * projection;
}