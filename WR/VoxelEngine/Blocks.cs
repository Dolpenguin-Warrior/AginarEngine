using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class Grass : BlockType
    {
        public override Vector3 emitsLight => new Vector3(0, 0, 0);

        public override Item[] itemDrop => new Item[] { Item.DirtBlock };

        public override string domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";
    }

    public class Dirt : BlockType
    {
        public override Item[] itemDrop => new Item[] { Item.DirtBlock };

        public override Vector3 emitsLight => new Vector3(0, 0, 0);

        public override string domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";
    }

    public class Stone : BlockType
    {
        public override Vector3 emitsLight => new Vector3(0, 0, 0);
        public override Item[] itemDrop => new Item[] { Item.StoneBlock };

        public override string domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";
    }

    public class Lamp : BlockType
    {
        public override Item[] itemDrop => new Item[] { Item.LampBlock };

        public override Vector3 emitsLight => new Vector3(1, 1, 1);

        public override string domain => $"Base{TypeDescriptor.GetClassName(this.GetType())}";
    }
}
