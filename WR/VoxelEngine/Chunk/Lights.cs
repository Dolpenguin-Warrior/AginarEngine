using Aginar.VoxelEngine.ChunkData;
using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aginar.VoxelEngine.ChunkData
{
    public class Lights
    {
        const int LIGHTING_BITS = 4;
        const int INDEX_SIZE = LIGHTING_BITS * 3;
        private BitBuffer bitBuffer;

        public Lights()
        {
            bitBuffer = new BitBuffer(World.CHUNK_SIZE_CUBED * INDEX_SIZE);

        }

        public Vector3i GetLight(int index)
        {
            return new Vector3i(
                (int)bitBuffer.GetBits(index * INDEX_SIZE, LIGHTING_BITS).Data,
                (int)bitBuffer.GetBits(index * INDEX_SIZE + LIGHTING_BITS, LIGHTING_BITS).Data,
                (int)bitBuffer.GetBits(index * INDEX_SIZE + (2 * LIGHTING_BITS), LIGHTING_BITS).Data
                );
        }

        public uint GetLightUInt(int index)
        {
            return bitBuffer.GetBits(index * INDEX_SIZE, INDEX_SIZE).Data;
                
        }

        public void SetLight(int index, uint lighting)
        {
            bitBuffer.SetBits(index * INDEX_SIZE, INDEX_SIZE, new BitArray32(lighting));
        }

    }
}
