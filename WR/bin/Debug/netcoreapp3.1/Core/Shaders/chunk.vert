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

const uint yShift = 6u;
const uint zShift = 12u;

const uint rLightShift = 18u;
const uint gLightShift = 22u;
const uint bLightShift = 26u;

const uint rotationShift = 19u;
const uint normalShift = 16u;
const uint uvShift = 23u;

void main(void)
{
    vec4 position;

    position.x = aPosition.x & 0x3Fu;
    position.y = ((aPosition.x & 0xFC0u) >> yShift);
    position.z = ((aPosition.x & 0x3F000u) >> zShift);
    position.w = 1;

    lighting = vec4((aPosition.x >> rLightShift) & 15u, (aPosition.x >> gLightShift) & 15u, (aPosition.x >> bLightShift) & 15u, 1);

    uint blockType = aPosition.y  & 0xFFFFu;

    uint rotation = (aPosition.y >> rotationShift) & 15u;

    uint normal  = (aPosition.y & 0x70000u) >> normalShift;

    uint UV = (aPosition.y & 0x1800000u) >> uvShift;
    

    texCoord = texCoords[UV] + vec2(0.125f * normal, 0.125f * (blockType));

    gl_Position = position * model * view * projection;
}