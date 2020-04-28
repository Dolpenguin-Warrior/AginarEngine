//#if DEBUG
namespace Aginar.VoxelEngine
{
    internal struct uint3
    {
        public int x;
        public int y;
        public int z;

        public uint3(int v1, int v2, int v3)
        {
            this.x = v1;
            this.y = v2;
            this.z = v3;
        }
    }
}