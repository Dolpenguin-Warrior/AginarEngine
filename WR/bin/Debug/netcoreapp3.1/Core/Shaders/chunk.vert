#version 330 core

layout(location = 0) in uvec2 aPosition;

out vec2 texCoord;
flat out vec4 lighting;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

vec2 texCoords[4] = vec2[4](
    vec2(0.0f, 1.0f),
    vec2(1.0f, 1.0f),
    vec2(1.0f, 0.0f),
    vec2(0.0f, 0.0f)
);


void main(void)
{
    vec4 position;

    position.x = aPosition.x & 0x3Fu;
    position.y = ((aPosition.x & 0xFC0u) >> 6u);
    position.z = ((aPosition.x & 0x3F000u) >> 12u);
    position.w = 1;

    lighting = vec4(1.0f, 1.0f, 1.0f, 1.0f);

    uint blockType = aPosition.y  & 65535u;

    uint rotation = (aPosition.y >> 19u) & 15u;

    uint normal  = (aPosition.y >> 16u) & 7u;

    uint verta = (aPosition.y & 0x1800000u) >> 23u;
    

    texCoord = vec2(texCoords[verta]);

    gl_Position = position * model * view * projection;
}