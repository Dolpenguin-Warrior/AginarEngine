using OpenToolkit.Graphics.ES11;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;
using System;

namespace Aginar.Core
{
    public class Game : GameWindow
    {
        private Mesh _mesh;
        public Game(string windowTitle, int width, int height, int renderFrequency, int updateFrequency, GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            Title = windowTitle;
            Size = new OpenToolkit.Mathematics.Vector2i(width, height);
            UpdateFrequency = updateFrequency;
            RenderFrequency = renderFrequency;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(Key.Escape))
            {
                Close();
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            
            _mesh = new Mesh();
            _mesh.SetData(
                new Vertex[] 
                {
                    new Vertex(0.5f,  0.5f, 0.0f, 1.0f, 1.0f), // top right
                    new Vertex(0.5f, -0.5f, 0.0f, 1.0f, 0.0f), // bottom right
                    new Vertex(-0.5f, -0.5f, 0.0f, 0.0f, 0.0f), // bottom left
                    new Vertex(-0.5f,  0.5f, 0.0f, 0.0f, 1.0f), // top left
                }, 
                new uint[] 
                {
                    0, 1, 3,// clockwise order (tr, br, tl)
                    1, 2, 3
                }
                
            
            );
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _mesh.Draw();
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            _mesh.Dispose();
            base.OnUnload();
        }
    }
}
