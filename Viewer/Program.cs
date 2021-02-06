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
        //static Matrix4x4 lookat = Matrix.LookAt(new Vector3f(2, 3, 5), new Vector3f(0, 0, 0), new Vector3f(0, 1, 0));
        //static Matrix4x4 perspective = Matrix.Perspective(60, 1.333f, 0.1f, 100);
        static Camera camera = new Camera();
        static CameraFirstPersonController fpController = new CameraFirstPersonController();
        static OglVertexArray create(Mesh mesh)
        {
            var meshVertices = mesh.Select(new string[] { "v", "vn" });
            var triangleIndices = mesh.GetTriangleIndices(string.Empty);
            var vertexArray = new OglVertexArray().Init(
               (option) =>
               {
                   option.SetAttribute(
                       OglVertexAttributeType.Float3,
                       OglVertexAttributeType.Float3);
               },
               (builder) =>
               {
                   foreach (var v in meshVertices)
                   {
                       var vertexAttributeValuesList = new List<ParamVector>(v).ConvertAll(e => e.values);
                       builder.AddVertex(vertexAttributeValuesList);
                   }
                   builder.AddIndices(OglVertexArray.Primitive.Type.Triangles, triangleIndices);
               }
           );
            return vertexArray;
        }


        static void Main(string[] args)
        {
            var reader = Utils.GetStreamReader("/data/teapot.obj");
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
            // Set context creation hints
            PrepareContext();
            // Create a window and shader program
            var window = CreateWindow(800, 600);
            var keyState = new GLFWKeyState();
            keyState.Init(window);
            var mouseState = new GLFWMouseState();
            mouseState.Init(window);
            var program = new OglProgram().Init((option) =>
            {
                option.AddShader(OglProgram.ShaderType.Vertex, "./triangle.vert");
                option.AddShader(OglProgram.ShaderType.Fragment, "./triangle.frag");
                option.CompileNow();
            });

            var vertexArrays = new List<Mesh>(model.meshDict.Values).ConvertAll(mesh=>create(mesh));

            rand = new Random();
            camera.controller = fpController;//.param.position = new Vector3f(2, 3, 5);
            camera.Update();
            var fpInput = new CameraFirstPersonController.InputHandler { controller = fpController, camera = camera};
            fpInput.EnableMouseLook(true);
            Matrix4x4 mvp = new Matrix4x4();
            Matrix4x4.Mul(camera.perspective, camera.lookat, mvp);
            //var mvp = camera.perspective * camera.lookat; //Matrix4x4.I();
            SetRandomColor(program);
            program.SetUniform("mvp", mvp);

            var err = GetError();
            Console.WriteLine(err);
            long n = 0;
            glEnable(GL_DEPTH_TEST);
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
                //camera.Update();
                Matrix4x4.Mul(camera.perspective, camera.lookat, mvp);
                program.SetUniform("mvp", mvp);
                program.Use();
                // Draw the triangle.
                foreach (var vertexArray in vertexArrays)
                    vertexArray.Draw();
                KeyState.current.FrameClear();
            }

            Glfw.Terminate();
        }
        static double radR = 0;
        static double radG = Math.PI*2/3;
        static double radB = Math.PI*4/3;
        static void SetRandomColor(OglProgram program)
        {
            radR += rand.NextDouble();
            radG += rand.NextDouble();
            radB += rand.NextDouble();

            uniformValue.X = (float)(Math.Sin(radR*0.1) * 0.5 + 0.5);
            uniformValue.Y = (float)(Math.Sin(radG*0.1) * 0.5 + 0.5);
            uniformValue.Z = (float)(Math.Sin(radB*0.1) * 0.5 + 0.5);
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
