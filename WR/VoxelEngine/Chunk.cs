using Aginar.VoxelEngine.ChunkData;
using OpenToolkit.Mathematics;
using System;
using System.Dynamic;

namespace Aginar.VoxelEngine
{
    public class Chunk
    {
        private BlockStorage blocks = new BlockStorage(World.CHUNK_SIZE_CUBED);

        public Lights lights { get; private set; } = new Lights();

        public readonly World World;

        public Chunk[] chunks = new Chunk[6]; // N, S, E, W, U, D

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
                lights.SetLight(index, ((uint)MathF.Round(World.blocks[value].Light.Z) << 8) | ((uint)MathF.Round(World.blocks[value].Light.Y) << 4) | ((uint)MathF.Round(World.blocks[value].Light.X)));
                Lighting.LightStorage.PropogateLight(index, this);
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
                lights.SetLight(index, ((uint)MathF.Round(World.blocks[value].Light.Z) << 8) | ((uint)MathF.Round(World.blocks[value].Light.Y) << 4) | ((uint)MathF.Round(World.blocks[value].Light.X)));
                Lighting.LightStorage.PropogateLight(index, this);
            }
        }
    }
}