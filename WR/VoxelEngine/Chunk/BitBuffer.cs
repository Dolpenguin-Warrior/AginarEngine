using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Aginar.VoxelEngine.ChunkData
{
    public class BitBuffer
    {
        private BitArray bits;

        /// <summary>
        /// Create a new bit buffer
        /// </summary>
        /// <param name="size">The size of the buffer (in bits)</param>
        public BitBuffer(int size)
        {
            bits = new BitArray(size);
        }

        public BitArray32 GetBits(int bit, int range)
        {
            BitArray32 output = new BitArray32();
            for (int i = 0; i < range; i++)
            {
                output[i] = bits.Get(bit + i);
            }
            return output;
        }

        public void SetBits(int bit, int range, BitArray32 data)
        {
            
            for (int i = 0; i < range; i++)
            {
                bits.Set(bit +  i, data[i]);
            }
        }

        public bool GetBit (int bit)
        {
            return bits[bit];
        }

        public void SetBit(int bit, bool value)
        {
            bits[bit] = value;
        }
    }
}
