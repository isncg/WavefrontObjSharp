using System;
using System.Collections.Generic;
using System.Text;
using GLFW;
using static OpenGL.Gl;


namespace Viewer
{
    public partial class OglVertexArray
    {
        byte[] data = null;
        uint vao = 0;
        uint vbo = 0;

        public class Primitive
        {
            public enum Type: int
            {
                Triangles = GL_TRIANGLES
            }
            public Type type;
            public uint[] indices;
        }
        public List<Primitive> primitives = new List<Primitive>();

        public OglVertexArray Init(Action<InitOption> option, Action<VertexDataBuilder> builder)
        {
            if (option != null)
            {
                var initOption = new InitOption(this);
                option(initOption);
                //data = initOption.databytes;
                vao = 0;
                vbo = 0;
            }

            if (builder != null)
            {
                var vartexDataBuilder = new VertexDataBuilder(this);
                builder(vartexDataBuilder);
                data = vartexDataBuilder.buildResult.ToArray();
            }

            return this;
        }

        public void Bind()
        {
            if (vao <= 0)
                CreateBuffer();
            else
                glBindVertexArray(vao);
        }

        unsafe void CreateBuffer()
        {
            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);

            glBindBuffer(GL_ARRAY_BUFFER, vbo);
            fixed (byte* v = &data[0])
            {
                glBufferData(GL_ARRAY_BUFFER, data.Length, v, GL_STATIC_DRAW);
            }
            foreach(var attr in attributes)
            {
                glEnableVertexAttribArray(attr.index);
                glVertexAttribPointer(attr.index, attr.size, attr.type, attr.normalized, attr.stride, (void*)attr.offset);
            }
        }
        private List<OglVertexAttribute> attributes = new List<OglVertexAttribute>();
        private int vertexByteSize = 0;
     
        public void Draw()
        {
            Bind();
            foreach (var primitive in primitives)
                glDrawElements((int)primitive.type, primitive.indices);
        }
    }
}
