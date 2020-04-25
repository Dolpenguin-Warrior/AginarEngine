using OpenToolkit.Mathematics;

namespace Aginar.VoxelEngine
{
    public abstract class BlockType
    {
        public abstract string domain { get; }
        public abstract Item[] itemDrop { get; }
        public abstract Vector3 emitsLight { get; }
    }
}