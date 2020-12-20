using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;


namespace Viewer
{
    class Program
    {
        static OglProgram.UniformValueFloat3 uniformValue = new OglProgram.UniformValueFloat3();
        static void Main(string[] args)
        {
            // Set context creation hints
            PrepareContext();
            // Create a window and shader program
            var window = CreateWindow(1024, 800);
            var program = new OglProgram().Init((option) =>
            {
                option.AddShader(OglProgram.ShaderType.Vertex, "./triangle.vert");
                option.AddShader(OglProgram.ShaderType.Fragment, "./triangle.frag");
                option.AddUniform(OglProgram.Uniform.Type.Float3, "color");
                option.CompileNow();
            }); //CreateProgram();

            var vertexArray = new OglVertexArray().Init((option) => {
                option
                .Add3F(-0.5f, -0.5f, 0.0f)
                .Add3F(0.5f, -0.5f, 0.0f)
                .Add3F(0.0f, 0.5f, 0.0f)
                
                .AddAttribute(0, 3, GL_FLOAT, false, 3 * sizeof(float));
            });
            vertexArray.Bind();
            rand = new Random();
            SetRandomColor(program);
            long n = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                // Swap fore/back framebuffers, and poll for operating system events.
                Glfw.SwapBuffers(window);
                Glfw.PollEvents();

                // Clear the framebuffer to defined background color
                glClear(GL_COLOR_BUFFER_BIT);

                if (n++ % 60 == 0)
                    SetRandomColor(program);

                // Draw the triangle.
                glDrawArrays(GL_TRIANGLES, 0, 3);
            }

            Glfw.Terminate();
        }

        static void SetRandomColor(OglProgram program)
        {
            uniformValue.v0 = (float)rand.NextDouble();
            uniformValue.v1 = (float)rand.NextDouble();
            uniformValue.v2 = (float)rand.NextDouble();
            program.SetUniform("color", uniformValue);
        }

        private static void PrepareContext()
        {
            // Set some common hints for the OpenGL profile creation
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Doublebuffer, true);
            Glfw.WindowHint(Hint.Decorated, true);
        }
        private static Window CreateWindow(int width, int height)
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
        private static Random rand;
    }
}
