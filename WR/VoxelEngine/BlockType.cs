using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Aginar.VoxelEngine
{
    public abstract class BlockType
    {
        public abstract Item[] itemDrop { get; }
        public abstract Vector3 emitsLight { get; }
    }
}
