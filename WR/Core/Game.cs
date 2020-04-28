using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace Aginar.Core
{
    using VoxelEngine;

    public class Game : GameWindow
    {
        private Mesh _mesh;
        private Camera _camera;
        private World _world;

        public Game(string windowTitle, int width, int height, int renderFrequency, int updateFrequency, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            Title = windowTitle;
            Size = new OpenToolkit.Mathematics.Vector2i(width, height);
            UpdateFrequency = updateFrequency;
            RenderFrequency = renderFrequency;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused)
                return;
            if (KeyboardState.IsKeyDown(Key.Escape))
            {
                Close();
            }
#if DEBUG
            if (KeyboardState.IsKeyDown(Key.J))
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
#endif
            const float cameraSpeed = 4f;
            const float sensitivity = 0.2f;

            if (KeyboardState.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (KeyboardState.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (KeyboardState.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (KeyboardState.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (KeyboardState.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (KeyboardState.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            float deltaX = MousePosition.X - Bounds.HalfSize.X;
            float deltaY = MousePosition.Y - Bounds.HalfSize.Y;
            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            MousePosition = new Vector2(Bounds.HalfSize.X, Bounds.HalfSize.Y);
            _mesh.UpdateView(_camera.GetViewMatrix());
            base.OnUpdateFrame(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            
            _mesh = new Mesh();

            _camera = new Camera(Vector3.UnitZ * 3, (float)Size.X / Size.Y);
            
            _mesh.UpdateProjection(_camera.GetProjectionMatrix());
            _mesh.UpdateView(_camera.GetViewMatrix());
            _mesh.UpdateModel(OpenToolkit.Mathematics.Matrix4.Identity);
            _mesh.SetTexture("Core/Textures/tilemap.png");

            _world = new World();

            ChunkMeshGenerator.GenerateMesh(_mesh, _world._chunks[new Vector3i()]);

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _mesh.Draw();
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            System.Console.WriteLine("window resize");
            GL.Viewport(0, 0, e.Width, e.Height);
            
            if (_camera != null)
                _camera.AspectRatio = e.Width / (float)e.Height;
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            _mesh.Dispose();
            base.OnUnload();
        }
    }
}