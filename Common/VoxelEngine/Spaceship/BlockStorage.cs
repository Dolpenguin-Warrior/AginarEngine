using System;
using System.Collections.Generic;
using System.Text;

namespace Common.VoxelEngine.Spaceship
{
    public class BlockStorage
    {
        private readonly int _size;
        private BitBuffer _data;
        private PaletteEntry[] palette;
        private int paletteCount;
        private int indicesLength;

        public BlockStorage(int size)
        {
            this._size = size;
            this.indicesLength = 1;
            this.paletteCount = 0;
            this.palette = new PaletteEntry[1 << indicesLength];
            this._data = new BitBuffer(size * indicesLength);
            palette[0] = new PaletteEntry(size, 0);
        }

        // Setting the wrong bit :eyesBlur:
        public void SetBlock(int index, int blockType)
        {
            // Get the type of block at that position
            int paletteIndex = (int)_data.GetBits(index * indicesLength, indicesLength).Data;
            PaletteEntry current = palette[paletteIndex];

            // That block at the position no longer exists, so one less reference of the block in the palette
            current.refcount -= 1;

            // Is the block type already in the palette
            int replace = -1;
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i] != null && palette[i].type == blockType)
                {
                    replace = i;
                    break;
                }
            }
            if (replace != -1)
            {
                _data.SetBits(index * indicesLength, indicesLength, new BitArray32((uint)replace));
                palette[replace].refcount += 1;
                return;
            }

            // Otherwise if we can overwrite the current palette entry
            if (current.refcount == 0)
            {
                current.type = blockType;
                current.refcount = 1;
                return;
            }

            // A new palette entry is needed!
            int newEntry = NewPaletteEntry();

            palette[newEntry] = new PaletteEntry(1, blockType);
            _data.SetBits(index * indicesLength, indicesLength, new BitArray32((uint)newEntry));
            paletteCount += 1;
        }

        public int GetBlock(int index)
        {
            int palettePos = (int)_data.GetBits(index * indicesLength, indicesLength).Data;
            return palette[palettePos].type;

        }

        private int NewPaletteEntry()
        {
            int firstFree = -1;
            // Get the first null or refCount = 0 entry
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i] == null || palette[i].refcount == 0)
                {
                    firstFree = i;
                    break;
                }
            }

            if (firstFree != -1)
                return firstFree;

            GrowPalette();

            return NewPaletteEntry();
        }

        private void GrowPalette()
        {
            // Decode the indices
            int[] indices = new int[_size];
            for (int i = 0; i < _size; i++)
            {
                indices[i] = (int)_data.GetBits(i * indicesLength, indicesLength).Data;
            }

            // Create a new palette, double the size
            indicesLength = indicesLength << 1;

            PaletteEntry[] newPalette = new PaletteEntry[(int)MathF.Pow(2, indicesLength)];
            int k = 0;
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i] != null && palette[i].refcount > 0)
                {
                    newPalette[k++] = palette[i];
                }
            }
            palette = newPalette;

            _data = new BitBuffer(_size * indicesLength);

            for (int i = 0; i < indices.Length; i++)
            {
                _data.SetBits(i * indicesLength, indicesLength, new BitArray32((uint)indices[i]));

            }
        }

        // Shrink the palette because it may be too big (old entries removed)
        private void FitPalette()
        {
            for (int i = 0; i < palette.Length; i++)
            {
                // Remove old entries
                if (palette[i] != null && palette[i].refcount == 0)
                {
                    palette[i] = null;
                    paletteCount -= 1;
                }
            }


            if (paletteCount > PowerOfTwo(paletteCount) / 2)
            {
                // Cannot shrink palette
                return;
            }

            int[] indices = new int[_size];
            for (int i = 0; i < _size; i++)
            {
                indices[i] = (int)_data.GetBits(i * indicesLength, indicesLength).Data;
            }

            indicesLength = indicesLength >> 1;
            PaletteEntry[] newPalette = new PaletteEntry[(int)MathF.Pow(2, indicesLength)];

            int paletteCounter = 0;
            for (int pi = 0; pi < palette.Length; pi++, paletteCounter++)
            {
                PaletteEntry entry = newPalette[paletteCounter] = palette[pi];

                for (int di = 0, fc = 0; di < indicesLength && fc < entry.refcount; di++)
                {
                    if (pi == indices[di])
                    {
                        indices[di] = paletteCounter;
                        fc += 1;
                    }
                }
            }

            // allocate a new bitbuffer
            _data = new BitBuffer(_size * indicesLength);

            // Reencode indices
            for (int i = 0; i < indices.Length; i++)
            {
                _data.SetBits(i * indicesLength, indicesLength, new BitArray32((uint)indices[i]));
            }
        }

        //https://www.geeksforgeeks.org/smallest-power-of-2-greater-than-or-equal-to-n/
        private int PowerOfTwo(int n)
        {
            int count = 0;

            // First n in the below  
            // condition is for the 
            // case where n is 0 
            if (n > 0 && (n & (n - 1)) == 0)
                return n;

            while (n != 0)
            {
                n >>= 1;
                count += 1;
            }

            return 1 << count;
        }

        private class PaletteEntry
        {
            public int type;
            public int refcount = 0;

            public PaletteEntry(int index, int refcount = 0) => (this.type, this.refcount) = (index, refcount);
        }
    }
    
}

