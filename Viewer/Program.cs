using GLFW;
using OpenGL;
using System;
using System.Collections.Generic;
using WavefrontObjSharp;
using static OpenGL.Gl;

namespace Viewer
{
    class Program : OglApplication
    {
        Camera camera = new Camera();
        //CameraFirstPersonController fpController = new CameraFirstPersonController();
        ObjModel teapotModel = null;
        Texture texRemilia = null;
        OglProgram progTeapot = null;
        OglProgram progRect = null;
        OglProgram progDeferred = null;
        List<OglVertexArray> teapotVertexArrays = null;
        List<OglVertexArray> viewRects = new List<OglVertexArray>();
        FrameBuffer frameBuffer = null;
        public override void Init()
        {
            base.Init();
            glEnable(GL_DEPTH_TEST);
            glClearColor(0.2f, 0.2f, 0.2f, 0.0f);

            progTeapot = new OglProgram("./triangle.vert", "./triangle.frag");
            teapotModel = Parser.CreateDefault().Run("/data/teapot.obj");
            teapotVertexArrays = new List<Mesh>(teapotModel.meshDict.Values).ConvertAll(mesh => create(mesh));

            texRemilia = Texture.Create("/data/remilia.jpg");

            frameBuffer = new FrameBuffer();
            frameBuffer.Init(4, true, 1920, 1080);
            frameBuffer.clearOption.color.Set(0.0f, 0.0f, 0.0f, 0.0f);

            viewRects.Add(CreateRect(-1,    -0.02f,   0.02f,   1));
            viewRects.Add(CreateRect(0.02f, 1,        0.02f,   1));
            viewRects.Add(CreateRect(-1,    -0.02f,   -1,      -0.02f));
            viewRects.Add(CreateRect(0.02f, 1,        -1,      -0.02f));
            progRect = new OglProgram("./rect.vert", "rect.frag");
            progDeferred = new OglProgram("./deferred.vert", "./deferred.frag");
            Input.RegisterHandler(new CameraFirstPersonInputController(camera, true)); //new CameraFirstPersonController.InputHandler(controller: fpController, camera: camera, mouseLookEnable: true));
        }

        public override void Render(Window window)
        {
            base.Render(window);

            frameBuffer.Use(clear:true);
            progDeferred.Use((config) => {
                config.SetUniform(OglProgram.UniformID._mvp, camera.MVP);
                config.SetUniform(OglProgram.UniformID._tex, texRemilia);
            });
            // Draw the triangle.
            foreach (var va in teapotVertexArrays)
                va.Draw();

            FrameBuffer.UseDefault(window, clear:true);
            progRect.Use(config => config.SetUniform(OglProgram.UniformID._tex, frameBuffer.colors[0])); viewRects[0].Draw();
            progRect.Use(config => config.SetUniform(OglProgram.UniformID._tex, frameBuffer.colors[1])); viewRects[1].Draw();
            progRect.Use(config => config.SetUniform(OglProgram.UniformID._tex, frameBuffer.colors[2])); viewRects[2].Draw();
            progRect.Use(config => config.SetUniform(OglProgram.UniformID._tex, frameBuffer.colors[3])); viewRects[3].Draw();
            //progRect.Use(config => config.SetUniform(OglProgram.UniformID._tex, frameBuffer.depth)); viewRects[1].Draw();
        }

        OglVertexArray create(Mesh mesh)
        {
            var meshVertices = mesh.Select("v", "vn", "vt");
            var triangleIndices = mesh.GetTriangleIndices(string.Empty);
            var vertexArray = new OglVertexArray().Init(
               option => option.SetAttribute(OglVertexAttributeType.Float3, OglVertexAttributeType.Float3, OglVertexAttributeType.Float2),
               builder =>
               {
                   foreach (var v in meshVertices)
                       builder.AddVertex(new List<ParamVector>(v).ConvertAll(e => (object)e.values));
                   builder.AddIndices(OglVertexArray.Primitive.Type.Triangles, triangleIndices);
               }
            );
            return vertexArray;
        }

        OglVertexArray CreateRect(float xMin, float xMax, float yMin, float yMax)
        {
            return new OglVertexArray().Init(
                option => option.SetAttribute(OglVertexAttributeType.Float2, OglVertexAttributeType.Float2),
                builder =>
                {
                    builder.AddVertex(new float[] { xMin, yMin }, new float[] { 0, 0 });
                    builder.AddVertex(new float[] { xMax, yMin }, new float[] { 1, 0 });
                    builder.AddVertex(new float[] { xMax, yMax }, new float[] { 1, 1 });
                    builder.AddVertex(new float[] { xMin, yMax }, new float[] { 0, 1 });

                    builder.AddIndices(OglVertexArray.Primitive.Type.Triangles, new uint[] { 0, 1, 2, 0, 2, 3 });
                });
        }

        static void Main(string[] args)
        {
            Program app = new Program();
            app.Start();
        }
    }
}
