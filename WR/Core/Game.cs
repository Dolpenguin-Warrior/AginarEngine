using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace Aginar.Core
{
    using Aginar.SmallShip;
    using VoxelEngine;

    public class Game : GameWindow
    {
        private ChunkMesh _mesh;
        private Mesh mesh;
        private Camera _camera;
        private World _world;

        public Game(string windowTitle, int width, int height, int renderFrequency, int updateFrequency, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            Title = windowTitle;
            Size = new OpenToolkit.Mathematics.Vector2i(width, height);
            UpdateFrequency = updateFrequency;
            RenderFrequency = renderFrequency;
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
            mesh.UpdateView(_camera.GetViewMatrix());

            base.OnUpdateFrame(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);

            mesh = new Mesh(new Meshes.VertexAttribute[] { Meshes.VertexAttribute.GenerateAttribute<float>(3, "aPosition", VertexAttribPointerType.Float), Meshes.VertexAttribute.GenerateAttribute<float>(2, "aTexCoord", VertexAttribPointerType.Float) });
            _mesh = new ChunkMesh();

            _camera = new Camera(Vector3.UnitZ * 3, (float)Size.X / Size.Y);

            mesh.UpdateProjection(_camera.GetProjectionMatrix());
            mesh.UpdateView(_camera.GetViewMatrix());
            mesh.UpdateModel(Matrix4.Identity);

            mesh.SetData(new float[] 
            {
                0, 0, -2, 0, 0,
                1, 0, -2, 1, 0,
                1, 1, -2, 1, 1,
                0, 1, -2, 0, 1,
            },
            new uint[]
            {
                0, 1, 2,
                0, 2, 3
            }
            );

            _mesh.UpdateProjection(_camera.GetProjectionMatrix());
            _mesh.UpdateView(_camera.GetViewMatrix());
            _mesh.UpdateModel(OpenToolkit.Mathematics.Matrix4.Identity);
            _mesh.SetTexture("Core/Textures/tilemap.png");


            _world = new World();

            double timer = 0;
            ChunkMeshGenerator.GenerateMesh(_mesh, _world._chunks[new Vector3i()], ref timer);


            Common.VoxelEngine.Spaceship.SmallShip.Chunk chunk = new Common.VoxelEngine.Spaceship.SmallShip.Chunk();

            for (int i = 0; i < Common.VoxelEngine.Spaceship.SmallShip.SmallShipComponent.CHUNK_SIZE_CUBE; i++)
            {
                if (i % 2 == 0)
                    chunk.blocks[i] = 1;
            }

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _mesh.Draw();
            mesh.Draw();

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
            mesh.Dispose();
            base.OnUnload();
        }
    }
}