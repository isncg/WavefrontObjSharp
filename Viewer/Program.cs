using System.Collections.Generic;
using WavefrontObjSharp;
using static OpenGL.Gl;

namespace Viewer
{
    class Program : OglApplication
    {
        Camera camera = new Camera();
        CameraFirstPersonController fpController = new CameraFirstPersonController();
        CameraFirstPersonController.InputHandler fpInput = null;
        ObjModel model = null;
        Texture texture = null;
        OglProgram program = null;
        List<OglVertexArray> vertexArrays = null;

        public override void Init()
        {
            base.Init();
            glEnable(GL_DEPTH_TEST);
            glClearColor(0.2f, 0.2f, 0.2f, 0.0f);

            program = new OglProgram("./triangle.vert", "./triangle.frag");
            model = Parser.CreateDefault().Run("/data/teapot.obj");
            vertexArrays = new List<Mesh>(model.meshDict.Values).ConvertAll(mesh => create(mesh));

            camera.controller = fpController;//.param.position = new Vector3f(2, 3, 5);
            camera.Update();
            fpInput = new CameraFirstPersonController.InputHandler { controller = fpController, camera = camera };
            fpInput.EnableMouseLook(true);
            //SetRandomColor(program);
            program.SetUniform("mvp", camera.MVP);

            texture = Texture.Create("/data/remilia.jpg");
            texture.Activated();
            program.SetUniform("tex", texture);
        }

        public override void Render()
        {
            base.Render();

            fpInput.Update();
            program.SetUniform("mvp", camera.MVP);
            program.Use();
            // Draw the triangle.
            foreach (var vertexArray in vertexArrays)
                vertexArray.Draw();
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

        OglVertexArray CreateQuad()
        {
            return new OglVertexArray().Init(
                option => option.SetAttribute(OglVertexAttributeType.Float3, OglVertexAttributeType.Float3, OglVertexAttributeType.Float2),
                builder =>
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
            Program app = new Program();
            app.Start();
        }
    }
}
