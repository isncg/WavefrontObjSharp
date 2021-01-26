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

        public OglVertexArray Init(Action<InitOption> option, Action<VertexDataBuilder> builder)
        {
            if (option != null)
            {
                var initOption = new InitOption(this);
                option(initOption);
                data = initOption.databytes;
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
                glVertexAttribPointer(attr.index, attr.size, attr.type, attr.normalized, attr.stride, NULL);
            }
        }
        private List<OglVertexAttribute> attributes = new List<OglVertexAttribute>();
     
    }
}
