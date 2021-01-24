using System;
using System.Collections.Generic;
using System.IO;
using GLFW;
using WavefrontObjSharp;
using static OpenGL.Gl;


namespace Viewer
{
    class Program
    {
        static Vector3f uniformValue = new Vector3f();
        static void Main(string[] args)
        {
            var reader = Utils.GetStreamReader("/data/test.obj");
            if (reader == null)
                return;

            Parser parser = new Parser();
            parser.Configure(option =>
            {
                option.AddCommand("v", new ObjCommand_v("v"));
                option.AddCommand("vn", new ObjCommand_v("vn"));
                option.AddCommand("vt", new ObjCommand_v("vt"));
                option.AddCommand("f", new ObjCommand_f());
                option.AddCommand("o", new ObjCommand_o());
                if (reader != null)
                    option.SetInput(reader.ReadLine);
            });

            var model = parser.Run();
            var mesh = model.CurrentMesh;
            var vectorArrayList = mesh.CreateVectorArrayList(new string[] { "v" });
            var triangleIndices = mesh.GetTriangleIndices(string.Empty);
            // Set context creation hints
            PrepareContext();
            // Create a window and shader program
            var window = CreateWindow(1024, 800);
            var program = new OglProgram().Init((option) =>
            {
                option.AddShader(OglProgram.ShaderType.Vertex, "./triangle.vert");
                option.AddShader(OglProgram.ShaderType.Fragment, "./triangle.frag");
                option.CompileNow();
            }); //CreateProgram();

            var vertexArray = new OglVertexArray().Init((option) => {
                foreach(var v in vectorArrayList)
                {
                    option.Add3F(v[0].v[0], v[0].v[1], v[0].v[2]);
                }
                
                option.AddAttribute(0, 3, GL_FLOAT, false, 3 * sizeof(float));
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
                //glDrawArrays(GL_TRIANGLES, 0, 3);
                glDrawElements(GL_TRIANGLES, triangleIndices);
            }

            Glfw.Terminate();
        }

        static void SetRandomColor(OglProgram program)
        {
            uniformValue.X = (float)rand.NextDouble();
            uniformValue.Y = (float)rand.NextDouble();
            uniformValue.Z = (float)rand.NextDouble();
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
