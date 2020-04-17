using Agniar.Core;
using OpenToolkit.Graphics.ES11;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;
using System;

namespace Aginar.Core
{
    public class Game : GameWindow
    {
        private Mesh _mesh;
        private Camera _camera;

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

            const float cameraSpeed = 1.5f;
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

            float deltaX =  MousePosition.X - Bounds.HalfSize.X;
            float deltaY = MousePosition.Y - Bounds.HalfSize.Y ;
            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            MousePosition = new Vector2(Bounds.HalfSize.X, Bounds.HalfSize.Y);
            _mesh.UpdateView(_camera.GetViewMatrix());
            base.OnUpdateFrame(e);
        }

        protected override void OnLoad()
        {
            IsFocused = true;
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            

            _mesh = new Mesh();
            _mesh.SetData(
                new Vertex[] 
                {
                    new Vertex(-0.5f, -0.5f, 0.0f,  0.0f, 1.0f),
                    new Vertex(0.5f, -0.5f, 0.0f,  1.0f, 1.0f),
                    new Vertex(0.5f,  0.5f, 0.0f, 1.0f, 0.0f),
                    new Vertex(-0.5f,  0.5f, 0.0f,  0.0f, 0.0f),
                }, 
                new uint[] 
                {
                    0, 1, 2,// clockwise order (tr, br, tl)
                    2, 3, 0
                }
            );

            _camera = new Camera(Vector3.UnitZ * 3, 800/600);
            _mesh.UpdateProjection(_camera.GetProjectionMatrix());
            _mesh.UpdateView(_camera.GetViewMatrix());
            _mesh.UpdateModel(OpenToolkit.Mathematics.Matrix4.Identity);

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
            GL.Viewport(0, 0, e.Width, e.Height);
            if (_camera != null)
            _camera.Fov = e.Width / (float)e.Height;
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            _mesh.Dispose();
            base.OnUnload();
        }
    }
}
