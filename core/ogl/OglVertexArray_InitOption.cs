
using System.Collections.Generic;
using OpenGL;

namespace Viewer
{
    public partial class OglVertexArray
    {
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
                fixed (float* arr = bufferF)
                {
                    byte* bytes = (byte*)arr;
                    for (int i = 0; i < bytecount; i++)
                    {
                        dataList.Add(bytes[i]);
                    }
                }
            }

            //public InitOption Add3F(float a, float b, float c)
            //{
            //    bufferF[0] = a;
            //    bufferF[1] = b;
            //    bufferF[2] = c;
            //    AddBytesF(sizeof(float) * 3);
            //    return this;
            //}

            //public InitOption Add3F(string[] values)
            //{
            //    bufferF[0] = 0;
            //    bufferF[1] = 0;
            //    bufferF[2] = 0;
            //    for (int i = 0; i < values.Length && i < 3; i++)
            //    {
            //        float.TryParse(values[i], out bufferF[i]);
            //    }
            //    AddBytesF(sizeof(float) * 3);
            //    return this;
            //}

            //public unsafe InitOption AddAttribute(uint index, int size, int type, bool normalized, int stride, int offset)
            //{
            //    parent.attributes.Add(new OglVertexAttribute { index = index, size = size, type = type, normalized = normalized, stride = stride, offset = offset });
            //    return this;
            //}

            public unsafe InitOption SetAttribute(params OglVertexAttributeType[] attributeTypes)
            {
                int vertexSize = 0;
                int offset = 0;
                parent.attributes.Clear();
                for(int i = 0; i < attributeTypes.Length; i++)
                {
                    int size = 0;
                    int type = 0;
                    int bytesize = 0;
                    switch (attributeTypes[i])
                    {
                        case OglVertexAttributeType.Float3:
                            size = 3;
                            type = Gl.GL_FLOAT;
                            bytesize = 3 * sizeof(float);
                            break;
                        case OglVertexAttributeType.Float4:
                            size = 4;
                            type = Gl.GL_FLOAT;
                            bytesize = 4 * sizeof(float);
                            break;
                    }
                    var attr = new OglVertexAttribute { index = (uint)i, size = size, type = type, normalized = false, offset = offset };
                    offset += bytesize;
                    parent.attributes.Add(attr);
                }

                foreach(var attr in parent.attributes)
                {
                    attr.stride = offset;
                }
                return this;
            }
        }
    }
}