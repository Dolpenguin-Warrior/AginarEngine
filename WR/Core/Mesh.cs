using Aginar.Core.Meshes;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using System;

namespace Aginar.Core
{
    public class Mesh : IDisposable
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private Texture _texture;

        private int _triangleCount;

        private int totalVertexSize = 0;

        public Mesh(VertexAttribute[] vertexAttributes)
        {
            _vertexBufferObject = GL.GenBuffer();
            _elementBufferObject = GL.GenBuffer();

            SetShader("Core/Shaders/default.vert", "Core/Shaders/default.frag");
            SetTexture("Core/Textures/default.png");

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            int stride = 0;
            if (vertexAttributes.Length > 1)
            {
                foreach (var item in vertexAttributes)
                {
                    stride += item.byteSize;
                }
            }
            int offset = 0;
            for (int i = 0; i < vertexAttributes.Length; i++)
            {
                int vertexLocation = _shader.GetAttribLocation(vertexAttributes[i].attributeName);
                GL.EnableVertexAttribArray(vertexLocation);

                if (vertexAttributes[i].isInteger)
                    GL.VertexAttribIPointer(vertexLocation, vertexAttributes[i].size, VertexAttribIntegerType.UnsignedInt, stride, new IntPtr(offset));
                else
                    GL.VertexAttribPointer(vertexLocation, vertexAttributes[i].size, vertexAttributes[i].type, vertexAttributes[i].isNormalised, stride, new IntPtr(offset));

                offset += vertexAttributes[i].byteSize;
                totalVertexSize += vertexAttributes[i].byteSize;
            }
        }

        public void SetTexture(string texturePath)
        {
            try
            {
                _texture = new Texture(texturePath);
                _texture.Use();
            }
            catch
            {
                Console.WriteLine($"Texture initialisation failed!\n Texture Path \"{texturePath}\"");
            }
        }

        public void SetShader(string vertexPath, string fragmentPath)
        {
            try
            {
                _shader = new Shader(vertexPath, fragmentPath);
                _shader.Use();
            }
            catch
            {
                Console.WriteLine($"Shader initialisation failed!\n Vertex Path: \"{vertexPath}\" \n Fragment Path \"{fragmentPath}\"");
            }
        }

        public void SetData<T>(T[] vertices, uint[] indices) where T : unmanaged
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * totalVertexSize, vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _triangleCount = indices.Length;
        }

        public void UpdateView(Matrix4 viewMatrix)
            => _shader.SetMatrix4("view", viewMatrix);

        public void UpdateProjection(Matrix4 projectionMatrix)
            => _shader.SetMatrix4("projection", projectionMatrix);

        public void UpdateModel(Matrix4 modelMatrix)
            => _shader.SetMatrix4("model", modelMatrix);

        public void Draw()
        {
            GL.BindVertexArray(_vertexArrayObject);

            _shader.Use();
            _texture.Use();

            GL.DrawElements(PrimitiveType.Triangles, _triangleCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            _shader.Dispose();
            _texture.Dispose();
        }
    }
}