namespace Aginar.VoxelEngine.ChunkData
{
    internal class PaletteEntry
    {
        public int refcount = 0;
        public BlockType type;

        public PaletteEntry(int refcount, BlockType blockType) => (this.refcount, this.type) = (refcount, blockType);
    }
}