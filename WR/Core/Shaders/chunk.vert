#version 330 core

layout(location = 0) in uvec2 aPosition;

out vec2 texCoord;
flat out vec4 lighting;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

vec2 texCoords[4] = vec2[4](
    vec2(0.0f, 1f),
    vec2(0.125f, 1f),
    vec2(0.125f, 0.875f),
    vec2(0.0f, 0.875f)
);



void main(void)
{
    vec4 position;

    position.x = aPosition.x & 0x3Fu;
    position.y = ((aPosition.x & 0xFC0u) >> 6u);
    position.z = ((aPosition.x & 0x3F000u) >> 12u);
    position.w = 1;

    lighting = vec4((aPosition.x >> 18u) & 15u, (aPosition.x >> 22u) & 15u, (aPosition.x >> 26u) & 15u, 1);

    uint blockType = aPosition.y  & 0xFFFFu;

    uint rotation = (aPosition.y >> 19u) & 15u;

    uint normal  = (aPosition.y & 0x70000u) >> 16u;

    uint verta = (aPosition.y & 0x1800000u) >> 23u;
    

    texCoord = texCoords[verta] + vec2(0.125f * normal, 0.125f * (blockType-1u));

    gl_Position = position * model * view * projection;
}