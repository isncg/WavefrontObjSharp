﻿using GLFW;
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
        //OglProgram progTeapot = null;
        //OglProgram progRect = null;
        //OglProgram progDeferred = null;
        ShaderManager shaders = new ShaderManager();
       

        List<OglVertexArray> teapotVertexArrays = null;
        List<OglVertexArray> viewRects = new List<OglVertexArray>();
        GBuffer gBuffer = null;
        public override void Init()
        {
            base.Init();
            glEnable(GL_DEPTH_TEST);
            glClearColor(0.2f, 0.2f, 0.2f, 0.0f);
            shaders.Init();
            //progTeapot = new OglProgram("./teapot.vert", "./teapot.frag");
            teapotModel = Parser.CreateDefault().Run("/data/teapot.obj");
            teapotVertexArrays = new List<Mesh>(teapotModel.meshDict.Values).ConvertAll(mesh => create(mesh));

            texRemilia = Texture.Create("/data/remilia.jpg");

            gBuffer = new GBuffer();
            gBuffer.Init(1920, 1080);
            gBuffer.clearOption.color.Set(0.0f, 0.0f, 0.0f, 0.0f);

            viewRects.Add(CreateRect(-1, -0.02f, 0.02f, 1));
            viewRects.Add(CreateRect(0.02f, 1, 0.02f, 1));
            viewRects.Add(CreateRect(-1, -0.02f, -1, -0.02f));
            viewRects.Add(CreateRect(0.02f, 1, -1, -0.02f));
            //progRect = new OglProgram("./rect.vert", "rect.frag");
            //progDeferred = new OglProgram("./deferred.vert", "./deferred.frag");
            Input.RegisterHandler(new CameraFirstPersonInputController(camera, true)); //new CameraFirstPersonController.InputHandler(controller: fpController, camera: camera, mouseLookEnable: true));
        }

        public override void Render(Window window)
        {
            base.Render(window);

            gBuffer.Use(clear: true);
            shaders.deferred.Use((uniforms) =>
            {
                uniforms._mvp = camera.MVP;
                uniforms._tex = texRemilia;
            });
            // Draw the triangle.
            foreach (var va in teapotVertexArrays)
                va.Draw();

            FrameBuffer.UseDefault(window, clear: true);
            shaders.rect.Use(uniforms => uniforms._tex = gBuffer.TexColor); viewRects[0].Draw();
            shaders.rect.Use(uniforms => uniforms._tex = gBuffer.TexNormal); viewRects[1].Draw();
            shaders.rect.Use(uniforms => uniforms._tex = gBuffer.TexPosition); viewRects[2].Draw();
            shaders.rect.Use(uniforms => uniforms._tex = gBuffer.TexTexcoord); viewRects[3].Draw();
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
