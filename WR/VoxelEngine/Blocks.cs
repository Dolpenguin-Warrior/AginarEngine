using Aginar.Core;
using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class Air : BlockType
    {
        public override Vector3 Light => new Vector3(0, 0, 0);

        public override Item[] Items => new Item[] {  };

        public override string Domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";

        public override int Index => -1;

        public override MeshType meshType => MeshType.Cube;
    }

    public class Grass : BlockType
    {
        public override Vector3 Light => new Vector3(0, 0, 0);

        public override Item[] Items => new Item[] { Item.DirtBlock };

        public override string Domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";

        public override int Index => 1;

        public override MeshType meshType => MeshType.Cube;
    }

    public class Dirt : BlockType
    {
        public override Item[] Items => new Item[] { Item.DirtBlock };

        public override Vector3 Light => new Vector3(0, 0, 0);

        public override string Domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";

        public override int Index => 3;

        public override MeshType meshType => MeshType.Cube;
    }

    public class Stone : BlockType
    {
        public override Vector3 Light => new Vector3(0, 0, 0);
        public override Item[] Items => new Item[] { Item.StoneBlock };

        public override string Domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";

        public override int Index => 3;

        public override MeshType meshType => MeshType.Cube;
    }

    public class Lamp : BlockType
    {
        public override Item[] Items => new Item[] { Item.LampBlock };

        public override Vector3 Light => new Vector3(1, 1, 1);

        public override string Domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";

        public override int Index => 4;
        public override MeshType meshType => MeshType.Cube;
    }
}
