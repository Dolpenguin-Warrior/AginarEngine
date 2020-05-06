using OpenToolkit.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Aginar.Core.Meshes
{
    public struct VertexAttribute
    {
        public readonly int size;
        public readonly int byteSize;
        public readonly string attributeName;
        public readonly VertexAttribPointerType type;
        public readonly bool isInteger;
        public readonly bool isNormalised;

        public VertexAttribute(int Size, int ByteSize, string AttributeName, VertexAttribPointerType vertexAttribPointerType, bool normalized, bool isInteger) 
            => (size, byteSize, attributeName, this.type, this.isInteger, isNormalised) = (Size, ByteSize, AttributeName, vertexAttribPointerType, isInteger, normalized);

        public unsafe static VertexAttribute GenerateAttribute<T>(int size, string attributeName, VertexAttribPointerType vertexAttribPointerType, bool normalised = false, bool integer = false) where T : unmanaged
        {
            return new VertexAttribute(size, size * sizeof(T), attributeName, vertexAttribPointerType, normalised, integer);
        }
    }
}
