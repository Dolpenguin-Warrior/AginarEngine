using OpenToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class Grass : BlockType
    {
        public override Vector3 emitsLight => new Vector3(0, 0, 0);

        public override Item[] itemDrop => new Item[] { Item.DirtBlock };
    }

    public class Dirt : BlockType
    {
        public override Item[] itemDrop => new Item[] { Item.DirtBlock };

        public override Vector3 emitsLight => new Vector3(0, 0, 0);

    }

    public class Stone : BlockType
    {
        public override Vector3 emitsLight => new Vector3(0, 0, 0);
        public override Item[] itemDrop => new Item[] { Item.StoneBlock };
    }

    public class Lamp : BlockType
    {
        public override Item[] itemDrop => new Item[] { Item.LampBlock };

        public override Vector3 emitsLight => new Vector3(1, 1, 1);
    }
}
