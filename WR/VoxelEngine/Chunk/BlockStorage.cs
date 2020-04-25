using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;

namespace Aginar.VoxelEngine.ChunkData
{
    // https://www.reddit.com/r/VoxelGameDev/comments/9yu8qy/palettebased_compression_for_chunked_discrete/
    public class BlockStorage
    {
        private int size;
        private BitArray data;
        private PaletteEntry[] palette;
        private int paletteCount;
        private int indicesLength;

        public BlockStorage(int size)
        {
            this.size = size;
            this.indicesLength = 1;
            this.paletteCount = 0;
            this.palette = new PaletteEntry[1 << indicesLength];
            this.data = new BitArray(size * indicesLength); // the length is in bits, not bytes!
        }

        public void setBlock(int index, BlockType type)
        {
            BitVector32 pIndexVector = GetData(index);
            int paletteIndex = pIndexVector.Data;

            PaletteEntry current = palette[paletteIndex];

            // Whatever block is there will cease to exist in but a moment...

            current.refcount -= 1;

            // The following steps/choices *must* be ordered like they are.

            // --- Is the block-type already in the palette?
            int replace = SearchPalette(type);
            if (replace != -1)
            {
                // YES: Use the existing palette entry.
                SetData(index, replace);
                palette[replace].refcount += 1;
                return;
            }

            // --- Can we overwrite the current palette entry?
            if (current.refcount == 0)
            {
                // YES, we can!
                current.type = type;
                current.refcount = 1;
                return;
            }

            // --- A new palette entry is needed!

            // Get the first free palette entry, possibly growing the palette!
            int newEntry = newPaletteEntry();

            palette[newEntry] = new PaletteEntry(1, type);
            SetData(index, newEntry);
            paletteCount += 1;
        }

        private int SearchPalette(BlockType type)
        {
            int replace = -1;
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i].type.GetType() == type.GetType())
                {
                    replace = i;
                    break;
                }
            }

            return replace;
        }
        private int SearchPalette(int refcount)
        {
            int replace = -1;
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i].refcount == refcount)
                {
                    replace = i;
                    break;
                }
            }

            return replace;
        }

        private BitVector32 GetData(int index)
        {
            BitVector32 pIndexVector = new BitVector32();
            for (int i = 0; i < indicesLength; i++)
            {
                pIndexVector[i] = data[i + indicesLength * index];
            }

            return pIndexVector;
        }

        private void SetData(int index, int newEntry)
        {
            for (int i = 0; i < indicesLength; i++)
            {
                data[i + indicesLength * index] = ((newEntry >> i) & 1) == 1;
            }
        }

        public BlockType getBlock(int index)
        {
            int paletteIndex = GetData(index).Data;
            return palette[paletteIndex].type;
        }

        private int newPaletteEntry()
        {
            int firstFree = SearchPalette(0);

            if (firstFree != -1)
            {
                return firstFree;
            }

            // No free entry?
            // Grow the palette, and thus the BitBuffer
            growPalette();

            // Just try again now!
            return newPaletteEntry();
        }

        private void growPalette()
        {
            // decode the indices
            int[] indices = new int[size];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = GetData(i).Data;
            }

            // Create new palette, doubling it in size
            indicesLength = indicesLength << 1;
            PaletteEntry[] newPalette = new PaletteEntry[1 << indicesLength];
            palette.CopyTo(newPalette, 0);
            palette = newPalette;

            // Allocate new BitBuffer
            data = new BitArray(size * indicesLength); // the length is in bits, not bytes!

            // Encode the indices
            for (int i = 0; i < indices.Length; i++)
            {
                SetData(i*indicesLength, indices[i]);
            }
        }

        // Shrink the palette (and thus the BitBuffer) every now and then.
        // You may need to apply heuristics to determine when to do this.
        public void FitPalette()
        {
            // Remove old entries...
            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i].refcount == 0)
                {
                    palette[i] = null;
                    paletteCount -= 1;
                }
            }

            // Is the palette less than half of its closest power-of-two?
            if (paletteCount > PowerOfTwo(paletteCount) / 2)
            {
                // NO: The palette cannot be shrunk!
                return;
            }
            
            // decode all indices
            int[] indices = new int[size];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = GetData(i * indicesLength).Data;
            }

            // Create new palette, halfing it in size
            indicesLength = indicesLength >> 1;
            PaletteEntry[] newPalette = new PaletteEntry[1 << indicesLength];

            // We gotta compress the palette entries!
            int paletteCounter = 0;
            for (int pi = 0; pi < palette.Length; pi++, paletteCounter++)
            {
                PaletteEntry entry = newPalette[paletteCounter] = palette[pi];

                // Re-encode the indices (find and replace; with limit)
                for (int di = 0, fc = 0; di < indices.Length && fc < entry.refcount; di++)
                {
                    if (pi == indices[di])
                    {
                        indices[di] = paletteCounter;
                        fc += 1;
                    }
                }
            }

            // Allocate new BitBuffer
            data = new BitArray(size * indicesLength); // the length is in bits, not bytes!

            // Encode the indices
            for (int i = 0; i < indices.Length; i++)
            {
                SetData(i * indicesLength, indices[i]);
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
            // TODO: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.intrinsics.x86.lzcnt.leadingzerocount?view=netcore-3.0
        }
    }
}