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

        public Mesh()
        {
            _vertexBufferObject = GL.GenBuffer();
            _elementBufferObject = GL.GenBuffer();

            SetShader("Core/Shaders/default.vert", "Core/Shaders/default.frag");
            SetTexture("Core/Textures/default.png");

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
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

        public void SetData(Vertex[] vertices, uint[] indices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 5 * sizeof(float), vertices, BufferUsageHint.StaticDraw);

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