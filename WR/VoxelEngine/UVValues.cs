using System;
using System.Collections.Generic;
using System.Text;

namespace Aginar.VoxelEngine
{
    public class UVValues
    {
        private const int TextureWidth = 128;
        private const int TextureHeight = 128;
        private const int BlockSize = 16;
        public const float BlockUVWidth = BlockSize / (float)TextureWidth;
        public const float TexelWidth = 1 / (float)TextureWidth;
        public const float BlockUVHeight = BlockSize / (float)TextureHeight;
        public const float TexelHeight = 1 / (float)TextureHeight;

        
    }
}
