using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Aginar.VoxelEngine
{
    public class World
    {
        public const int LOG_CHUNK_SIZE = 5; // The number which is chunk size log. So 2^logchunksize is the chunk size.
        public const int CHUNK_SIZE = 1 << LOG_CHUNK_SIZE; // The size of a chunk
        public const int CHUNK_SIZE_SQUARE = CHUNK_SIZE * CHUNK_SIZE; // The amount of blocks in two dimensions of a chunk
        public const int CHUNK_SIZE_CUBED = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;  // The amount of blocks in a chunk

        public const int CHUNK_MASK = CHUNK_SIZE - 1; // The bitmask for replacing % 32 with magic bit operators

        public Dictionary<Vector3i, Chunk> _chunks = new Dictionary<Vector3i, Chunk>();

        public static readonly BlockType[] blocks = new BlockType[] { new Air(), new Grass(), new Stone(), new Dirt(), new Lamp(), new PurpleLamp() };

        public World()
        {
            _chunks.Add(new Vector3i(), new Chunk(this));
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    float height = PerlinNoise.Fbm(x / 40.12412f, z / 40.12412f, 8) * 10 + 10;
                    bool lamp = Math.Abs(PerlinNoise.Noise(x, z)) > 0.5f;
                    for (int y = 0; y < CHUNK_SIZE; y++)
                    {
                        int i = Vector3IntToIndex(x, y, z);
                        _chunks[new Vector3i()][i] = ((y > height) ? 0 : (y > height - 1) ? 1 : (y > height - 3) ? 3 : 2);

                    }
                }
            }

             //_chunks[new Vector3i()][1, 12, 5] = 4;
             //_chunks[new Vector3i()][10, 12, 5] = 4;
             _chunks[new Vector3i()][4, 12, 6] = 5;
             _chunks[new Vector3i()][8, 12, 4] = 5;


        }

        public BlockType this[int x, int y, int z]
        {
            get
            {
                return blocks[0];
            }
            set { }
        }

        /// <summary>
        /// Takes the local position of a block in a chunk and returns the 1d index of that block (to be used in flat arrays)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Vector3IntToIndex(int localX, int localY, int localZ)
        {
            return (localZ << (2* LOG_CHUNK_SIZE)) | (localY << LOG_CHUNK_SIZE) | localX;
        }

        /// <summary>
        /// Takes the local position of a block in a chunk and returns the 1d index of that block (to be used in flat arrays)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Vector3IntToIndex(Vector3i localPos)
        {
            return (localPos.Z << (2 * LOG_CHUNK_SIZE)) | (localPos.Y << LOG_CHUNK_SIZE) | localPos.X;
        }

        /// <summary>
        /// Converts a 1d index of a block in a chunk to the local 3d position of that block. index is ZZZZZ_YYYYY_XXXXX so therefore Z is moved 10 to the right, Y is moved 5 right and Z removed, X just has Y and Z removed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3i IndexToLocalPos(int index)
        {
            return new Vector3i(index & (CHUNK_MASK), (index >> LOG_CHUNK_SIZE) & CHUNK_MASK, index >> (2 * LOG_CHUNK_SIZE));
        }
    }
}