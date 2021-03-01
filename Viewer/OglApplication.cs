using GLFW;
using static OpenGL.Gl;
using System;
using System.Collections.Generic;
using System.Text;
using WavefrontObjSharp;

namespace Viewer
{
    public abstract class OglApplication
    {
        private void PrepareContext()
        {
            // Set some common hints for the OpenGL profile creation
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Doublebuffer, true);
            Glfw.WindowHint(Hint.Decorated, true);
        }

        private Window CreateWindow(int width, int height)
        {
            // Create window, make the OpenGL context current on the thread, and import graphics functions
            var window = Glfw.CreateWindow(width, height, "Viewer", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);

            // Center window
            var screen = Glfw.PrimaryMonitor.WorkArea;
            var x = (screen.Width - width) / 2;
            var y = (screen.Height - height) / 2;
            Glfw.SetWindowPosition(window, x, y);

            return window;
        }

        public virtual void Init()
        {

        }

        public virtual void Render()
        {

        }

        public void Start()
        {
            var window = CreateWindow(800, 600);
            var keyState = new GLFWKeyState();
            keyState.Init(window);
            var mouseState = new GLFWMouseState();
            mouseState.Init(window);

            Init();

            while (!Glfw.WindowShouldClose(window))
            {
                // Swap fore/back framebuffers, and poll for operating system events.
                Glfw.SwapBuffers(window);
                Glfw.PollEvents();

                // Clear the framebuffer to defined background color
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                Render();

                Input.AllHandlersFrameUpdate();
                //foreach (var handler in Input.handlers)
                //    if (null != handler) handler.FrameUpdate();
                KeyState.current.FrameClear();
                Time.frameStatics.Update();
                Console.Write(string.Format("\rFPS: {0}         \r", Time.FilteredFps));
            }
        }
    }
}
