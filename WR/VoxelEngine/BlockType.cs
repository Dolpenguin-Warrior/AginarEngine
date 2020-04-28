using OpenToolkit.Mathematics;

namespace Aginar.VoxelEngine
{
    public abstract class BlockType
    {
        public abstract string Domain { get; }
        public abstract Item[] Items { get; }
        public abstract Vector3 Light { get; }
        public abstract int Index { get; }
        public abstract MeshType meshType { get; }
    }
}