namespace Aginar.VoxelEngine.ChunkData
{
    internal class PaletteEntry
    {
        public int refcount = 0;
        public int type;

        public PaletteEntry(int refcount, int blockType) => (this.refcount, this.type) = (refcount, blockType);
    }
}