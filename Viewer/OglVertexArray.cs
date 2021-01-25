using System;
using System.Collections.Generic;
using System.Text;
using GLFW;
using static OpenGL.Gl;


namespace Viewer
{
    class OglVertexArray
    {
        byte[] data = null;
        uint vao = 0;
        uint vbo = 0;

        public OglVertexArray Init(Action<InitOption> callback)
        {
            if (callback != null)
            {
                var option = new InitOption(this);
                callback(option);
                data = option.databytes;
                vao = 0;
                vbo = 0;
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

        class VertexAttribute
        {
            public uint index;
            public int size;
            public int type;
            public bool normalized;
            public int stride;
        }
        
        private List<VertexAttribute> attributes = new List<VertexAttribute>();
        public class InitOption
        {
            private OglVertexArray parent;
            private float[] bufferF = new float[32];            
            private List<byte> dataList = new List<byte>();
            public byte[] databytes { get { return dataList.ToArray(); } }
            public InitOption(OglVertexArray parent)
            {
                this.parent = parent;
            }
            private unsafe void AddBytesF(int bytecount)
            {
                fixed(float* arr = bufferF)
                {
                    byte* bytes = (byte*)arr;
                    for(int i = 0; i < bytecount; i++)
                    {
                        dataList.Add(bytes[i]);
                    }
                }
            }

            public InitOption Add3F(float a, float b, float c)
            {
                bufferF[0] = a;
                bufferF[1] = b;
                bufferF[2] = c;
                AddBytesF(sizeof(float) * 3);
                return this;
            }

            public InitOption Add3F(string[] values)
            {
                bufferF[0] = 0;
                bufferF[1] = 0;
                bufferF[2] = 0;
                for(int i=0;i<values.Length && i < 3; i++)
                {
                    float.TryParse(values[i], out bufferF[i]);
                }
                AddBytesF(sizeof(float) * 3);
                return this;
            }

            public unsafe InitOption AddAttribute(uint index, int size, int type, bool normalized, int stride)
            {
                parent.attributes.Add(new VertexAttribute { index = index, size = size, type = type, normalized = normalized, stride = stride});
                return this;
            }
        }

     
    }
}
