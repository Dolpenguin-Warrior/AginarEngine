using System;
using System.Collections.Generic;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class Chunk
    {
        private ushort[] blocks = new ushort[World.CHUNK_SIZE_CUBED];
    }
}
