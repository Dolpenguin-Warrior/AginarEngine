using OpenToolkit.Mathematics;

namespace Aginar.VoxelEngine
{
    public class Chunk
    {
        private int[] blocks = new int[World.CHUNK_SIZE_CUBED];

        public readonly World World;

        public Chunk(World world) => World = world;

        public Vector3i position { get; private set; }

        internal int[] GetBlocks() => blocks;

        public void ResetChunk(Vector3i position)
        {
            this.position = position;
        }

        public int this[int index]
        {
            get
            {
                if (index > -1 && index < World.CHUNK_SIZE_CUBED)
                    return blocks[index];
                return 0;
            }
            set
            {
                if (index > -1 && index < World.CHUNK_SIZE_CUBED)
                    blocks[index] = value;
            }
        }

        public int this[int x, int y, int z]
        {
            get
            {
                int index = World.Vector3IntToIndex(x, y, z);
                if (index > -1 && index < World.CHUNK_SIZE_CUBED)
                    return blocks[index];
                return 0;
            }
            set
            {
                int index = World.Vector3IntToIndex(x, y, z);
                if (index > -1 && index < World.CHUNK_SIZE_CUBED)
                    blocks[index] = value;
            }
        }
    }
}