﻿using OpenGL;
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
        OglProgram progQuad = null;
        List<OglVertexArray> teapotVertexArrays = null;
        OglVertexArray quadVertexArray = null;
        FrameBuffer frameBuffer = null;
        public override void Init()
        {
            base.Init();
            glEnable(GL_DEPTH_TEST);
            glClearColor(0.2f, 0.2f, 0.2f, 0.0f);

            progTeapot = new OglProgram("./triangle.vert", "./triangle.frag");
            teapotModel = Parser.CreateDefault().Run("/data/teapot.obj");
            teapotVertexArrays = new List<Mesh>(teapotModel.meshDict.Values).ConvertAll(mesh => create(mesh));

            progQuad = new OglProgram("./quad.vert", "./quad.frag");

            quadVertexArray = CreateQuad();

            

            Input.RegisterHandler(new CameraFirstPersonInputController(camera, true)); //new CameraFirstPersonController.InputHandler(controller: fpController, camera: camera, mouseLookEnable: true));
            //SetRandomColor(program);
      
            texRemilia = Texture.Create("/data/remilia.jpg");
            texRemilia.Activated();

            progTeapot.Use((config)=> {
                config.SetUniform("mvp", camera.MVP);
                config.SetUniform("tex", texRemilia);
            });


            frameBuffer = new FrameBuffer();
            frameBuffer.Init(1, true, 1920, 1080);
            frameBuffer.clearOption.color.Set(1.0f, 0.0f, 0.0f, 1.0f);
            frameBuffer.colors[0].Activated();

            progQuad.Use((config)=> {
                config.SetUniform("tex", frameBuffer.colors[0]);
            });
        }

        public override void Render()
        {
            base.Render();

            frameBuffer.Use(clear:true);
            progTeapot.Use((config) => {
                config.SetUniform("mvp", camera.MVP);
            });
            // Draw the triangle.
            foreach (var va in teapotVertexArrays)
                va.Draw();

            FrameBuffer.UseDefault(clear:true);
            progQuad.Use((config) => {
                config.SetUniform("tex", frameBuffer.colors[0]);
            });
            quadVertexArray.Draw();
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
