
using System.Collections.Generic;

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
                for (int i = 0; i < values.Length && i < 3; i++)
                {
                    float.TryParse(values[i], out bufferF[i]);
                }
                AddBytesF(sizeof(float) * 3);
                return this;
            }

            public unsafe InitOption AddAttribute(uint index, int size, int type, bool normalized, int stride, int offset)
            {
                parent.attributes.Add(new OglVertexAttribute { index = index, size = size, type = type, normalized = normalized, stride = stride, offset = offset });
                return this;
            }
        }
    }
}