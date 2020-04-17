using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class World
    {
        public const int LOG_CHUNK_SIZE = 5; // The number which is chunk size log. So 2^logchunksize is the chunk size.
        public const int CHUNK_SIZE = 1 << LOG_CHUNK_SIZE; // The size of a chunk
        public const int CHUNK_SIZE_SQUARE = CHUNK_SIZE * CHUNK_SIZE; // The amount of blocks in two dimensions of a chunk
        public const int CHUNK_SIZE_CUBED = CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE;  // The amount of blocks in a chunk

        public const int CHUNK_MASK = CHUNK_SIZE - 1; // The bitmask for replacing % 32 with magic bit operators


        /// <summary>
        /// Takes the local position of a block in a chunk and returns the 1d index of that block (to be used in flat arrays)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Vector3IntToIndex(int localX, int localY, int localZ)
        {
            return (localZ << 10) | (localY << 5) | localX;
        }

        /// <summary>
        /// Takes the local position of a block in a chunk and returns the 1d index of that block (to be used in flat arrays)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Vector3IntToIndex(Vector3i localPos)
        {
            return (localPos.Z << 10) | (localPos.Y << 5) | localPos.X;
        }

        /// <summary>
        /// Converts a 1d index of a block in a chunk to the local 3d position of that block. index is ZZZZZ_YYYYY_XXXXX so therefore Z is moved 10 to the right, Y is moved 5 right and Z removed, X just has Y and Z removed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3i IndexToLocalPos(int index)
        {
            return new Vector3i(index & (CHUNK_SIZE - 1), (index >> LOG_CHUNK_SIZE) & (CHUNK_SIZE - 1), index >> (2 * LOG_CHUNK_SIZE));
        }
    }
}
