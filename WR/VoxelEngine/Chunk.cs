using Aginar.VoxelEngine.ChunkData;
using OpenToolkit.Mathematics;

namespace Aginar.VoxelEngine
{
    public class Chunk
    {
        private BlockStorage blocks = new BlockStorage(World.CHUNK_SIZE_CUBED);

        public readonly World World;

        public Chunk(World world) => World = world;

        public Vector3i position { get; private set; }

        internal BlockStorage GetBlocks() => blocks;

        public void ResetChunk(Vector3i position)
        {
            this.position = position;
        }

        public int this[int index]
        {
            get
            {
                index = index & (World.CHUNK_SIZE_CUBED - 1);
                return blocks.GetBlock(index);
            }
            set
            {
                index = index & (World.CHUNK_SIZE_CUBED - 1);
                blocks.SetBlock(index, value);
            }
        }

        public int this[int x, int y, int z]
        {
            get
            {
                int index = World.Vector3IntToIndex(x, y, z) & (World.CHUNK_SIZE_CUBED - 1);
                return blocks.GetBlock(index);
            }
            set
            {
                int index = World.Vector3IntToIndex(x, y, z) & (World.CHUNK_SIZE_CUBED - 1);
                blocks.SetBlock(index, value);
            }
        }
    }
}