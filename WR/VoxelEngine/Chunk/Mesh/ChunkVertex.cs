using System;
using System.Collections.Generic;
using System.Text;

namespace Aginar.VoxelEngine.ChunkData.Mesh
{
    public struct ChunkVertex
    {
        public uint BlockData; // 18 bits position, 12 bits lighting
        public uint BlockType; // 16 bits blocktype, 3 bits normal, 4 bits rotation (23 bits, 9 spare)
    }
}
