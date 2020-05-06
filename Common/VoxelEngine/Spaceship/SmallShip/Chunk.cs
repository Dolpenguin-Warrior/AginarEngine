using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VoxelEngine.Spaceship.SmallShip
{
    public class Chunk
    {
        public ushort[] blocks = new ushort[SmallShipComponent.CHUNK_SIZE_CUBE];

        // N, E, U
        public Chunk[] neighbours;
    }
}
