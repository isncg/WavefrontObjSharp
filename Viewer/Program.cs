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
        static Camera camera = new Camera();
        static CameraFirstPersonController fpController = new CameraFirstPersonController();
        static OglVertexArray create(Mesh mesh)
        {
            var meshVertices = mesh.Select("v", "vn", "vt");
            var triangleIndices = mesh.GetTriangleIndices(string.Empty);
            var vertexArray = new OglVertexArray().Init(
               (option) =>
               {
                   option.SetAttribute(
                       OglVertexAttributeType.Float3,
                       OglVertexAttributeType.Float3,
                       OglVertexAttributeType.Float2);
               },
               (builder) =>
               {
                   foreach (var v in meshVertices)
                   {
                       var vertexAttributeValuesList = new List<ParamVector>(v).ConvertAll(e => (object)e.values);
                       builder.AddVertex(vertexAttributeValuesList);
                   }
                   builder.AddIndices(OglVertexArray.Primitive.Type.Triangles, triangleIndices);
               }
           );
            return vertexArray;
        }

        static OglVertexArray CreateQuad()
        {
            return new OglVertexArray().Init(
                (option) =>
                {
                    option.SetAttribute(
                       OglVertexAttributeType.Float3,
                       OglVertexAttributeType.Float3,
                       OglVertexAttributeType.Float2);
                },
                (builder) =>
                {
                    builder.AddVertex(new float[] { 0, 0, 0 }, new float[] { 0, 0, 1 }, new float[] { 0, 0 });
                    builder.AddVertex(new float[] { 1, 0, 0 }, new float[] { 0, 0, 1 }, new float[] { 1, 0 });
                    builder.AddVertex(new float[] { 1, 1, 0 }, new float[] { 0, 0, 1 }, new float[] { 1, 1 });
                    builder.AddVertex(new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 }, new float[] { 0, 1 });

                    builder.AddIndices(OglVertexArray.Primitive.Type.Triangles, new uint[] { 0, 1, 2, 0, 2, 3 });
                });
        }

        static void Main(string[] args)
        {
            var model = Parser.CreateDefault().Run("/data/teapot.obj");
            // Set context creation hints
            PrepareContext();
            // Create a window and shader program
            var window = CreateWindow(800, 600);
            glEnable(GL_DEPTH_TEST);
            //glEnable(GL_TEXTURE);
            glClearColor(0.2f, 0.2f, 0.2f, 0.0f);


            var keyState = new GLFWKeyState();
            keyState.Init(window);
            var mouseState = new GLFWMouseState();
            mouseState.Init(window);
            var program = new OglProgram("./triangle.vert", "./triangle.frag");

            var vertexArrays = new List<Mesh>(model.meshDict.Values).ConvertAll(mesh=>create(mesh));

            camera.controller = fpController;//.param.position = new Vector3f(2, 3, 5);
            camera.Update();
            var fpInput = new CameraFirstPersonController.InputHandler { controller = fpController, camera = camera};
            fpInput.EnableMouseLook(true);
            SetRandomColor(program);
            program.SetUniform("mvp", camera.MVP);

            var texture = Texture.Create("/data/remilia.jpg");
            texture.Activated();
            program.SetUniform("tex", texture);

            long n = 0;
            
            while (!Glfw.WindowShouldClose(window))
            {
                // Swap fore/back framebuffers, and poll for operating system events.
                Glfw.SwapBuffers(window);
                Glfw.PollEvents();

                // Clear the framebuffer to defined background color
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                if (n++ % 60 == 0)
                    SetRandomColor(program);


                fpInput.Update();
                program.SetUniform("mvp", camera.MVP);
                program.Use();
                // Draw the triangle.
                foreach (var vertexArray in vertexArrays)
                    vertexArray.Draw();
                KeyState.current.FrameClear();
                Time.frameStatics.Update();
                Console.Write(string.Format("\rFPS: {0}         \r", Time.FilteredFps));
            }

            Glfw.Terminate();
        }

        static void SetRandomColor(OglProgram program)
        {
            double t = Time.RunningTime;
           
            uniformValue.X = (float)(Math.Sin(t) * 0.5 + 0.5);
            uniformValue.Y = (float)(Math.Sin(t+Math.PI*2/3) * 0.5 + 0.5);
            uniformValue.Z = (float)(Math.Sin(t+Math.PI*4/3) * 0.5 + 0.5);
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
    }
}
