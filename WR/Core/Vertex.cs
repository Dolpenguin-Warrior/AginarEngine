using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Aginar.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public float x, y, z;
        public float uvX, uvY;

        public Vertex(float x, float y, float z, float uvX, float uvY)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.uvX = uvX;
            this.uvY = uvY;
        }

    }
}
