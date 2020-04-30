using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Aginar.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkVertex
    {
        public uint BlockData; // 18 bits position, 12 bits lighting
        public uint BlockType; // 16 bits blocktype, 3 bits normal, 4 bits rotation, 2 bits uv, 3 bits face (23 bits, 9 spare)
    }
}
