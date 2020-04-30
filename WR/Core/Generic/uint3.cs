//#if DEBUG
using System.Runtime.InteropServices;

namespace Aginar.Core.Generic
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct uint3
    {
        public uint x;
        public uint y;
        public uint z;

        public uint3(uint v1, uint v2, uint v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }
    }
}