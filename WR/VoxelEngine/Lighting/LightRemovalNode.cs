using System;
using System.Collections.Generic;
using System.Text;

namespace Aginar.VoxelEngine.Lighting
{
    internal struct LightRemovalNode
    {
        public Chunk chunk;
        public int index;
        public uint value;
    }
}
