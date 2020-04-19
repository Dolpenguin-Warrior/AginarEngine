using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Aginar.VoxelEngine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Block
    {
        public int BlockType { get; private set; }
        public int LightLevel { get; private set; }
        public Direction Rotation { get; private set; }
        public bool HasData { get; private set; }


    }
}
