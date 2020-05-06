using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VoxelEngine.Spaceship.SmallShip
{
    public class SmallShipComponent
    {
        public const int LOG_CHUNK_SIZE = 5;
        public const int CHUNK_SIZE = 1 << LOG_CHUNK_SIZE;
        public const int CHUNK_SIZE_MASK = CHUNK_SIZE - 1;
        public const int CHUNK_SIZE_SQUARE = CHUNK_SIZE * CHUNK_SIZE;
        public const int CHUNK_SIZE_CUBE = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;
    }
}
