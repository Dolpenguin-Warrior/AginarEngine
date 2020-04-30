using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Aginar.VoxelEngine.ChunkData.Mesh;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aginar.Core
{
    public class ChunkMesh : IDisposable
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;
        private Texture _texture;

        private int _triangleCount;

        public ChunkMesh()
        {
            _vertexBufferObject = GL.GenBuffer();
            _elementBufferObject = GL.GenBuffer();

            SetShader("Core/Shaders/chunk.vert", "Core/Shaders/chunk.frag");
            SetTexture("Core/Textures/default.png");

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            int vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.UnsignedInt, false, 0, 0);
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

        public void SetData(ChunkVertex[] vertices, uint[] indices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 2 * sizeof(int), vertices, BufferUsageHint.StaticDraw);

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
