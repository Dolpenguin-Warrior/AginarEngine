#version 330

out vec4 outputColor;

in vec2 texCoord;
flat in vec4 lighting;

uniform sampler2D texture0;

void main()
{
    outputColor = texture(texture0, texCoord) * (vec4(0.2f, 0.2f, 0.2f, 0.2f) + (lighting / 16));
    if (outputColor.w == 0)
    {
        discard;
    }
}