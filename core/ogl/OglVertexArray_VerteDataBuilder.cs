using System.Collections.Generic;
using static OpenGL.Gl;
namespace Viewer
{
    public partial class OglVertexArray
    {
        public class VertexDataBuilder
        {
            public List<byte> buildResult = new List<byte>();
            private OglVertexArray parent;
            public VertexDataBuilder(OglVertexArray parent)
            {
                this.parent = parent;
            }

            
            unsafe List<byte> GetBytesF(string[] attribute, int elementCount)
            {
                int len = elementCount * sizeof(float);
                var numbers = new float[elementCount];
                var result = new byte[len];
                for(int i = 0; i < len; i++)
                {
                    result[i] = 0;
                }
                for(int i = 0; i < elementCount; i++)
                {
                    numbers[i] = 0;
                    if (i < attribute.Length)
                    {
                        float.TryParse(attribute[i], out numbers[i]);
                    }
                }
                fixed(float* ptr = &numbers[0])
                {
                    byte* bptr = (byte*)ptr;
                    
                    for (int i = 0; i < len; i++)
                    {
                        result[i] = bptr[i];
                    }
                }
                return new List<byte>(result);
            }
            unsafe List<byte> GetBytesI(string[] attribute, int elementCount)
            {
                int len = elementCount * sizeof(int);
                var numbers = new int[elementCount];
                var result = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    result[i] = 0;
                }
                for (int i = 0; i < elementCount; i++)
                {
                    numbers[i] = 0;
                    if (i < attribute.Length)
                    {
                        int.TryParse(attribute[i], out numbers[i]);
                    }
                }
                fixed (int* ptr = &numbers[0])
                {
                    byte* bptr = (byte*)ptr;

                    for (int i = 0; i < len; i++)
                    {
                        result[i] = bptr[i];
                    }
                }
                return new List<byte>(result);
            }


            public void AddVertex(List<string[]> vertexAttributeValuesList)
            {
                if(vertexAttributeValuesList.Count == parent.attributes.Count)
                {
                    for(int i = 0; i < vertexAttributeValuesList.Count; i++)
                    {
                        var size = parent.attributes[i].size;
                        var type = parent.attributes[i].type;
                        if (type == GL_FLOAT)
                        {
                            buildResult.AddRange(GetBytesF(vertexAttributeValuesList[i], size));
                        }
                        else
                        {
                            buildResult.AddRange(GetBytesI(vertexAttributeValuesList[i], size));
                        }
                    }
                }
            } 

        }
    }
}