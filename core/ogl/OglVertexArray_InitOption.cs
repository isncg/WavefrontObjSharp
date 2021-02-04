
using System.Collections.Generic;
using OpenGL;

namespace Viewer
{
    public partial class OglVertexArray
    {
        public class InitOption
        {
            private OglVertexArray parent;
            public InitOption(OglVertexArray parent)
            {
                this.parent = parent;
            }
          
            public unsafe InitOption SetAttribute(params OglVertexAttributeType[] attributeTypes)
            {
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
                parent.vertexByteSize = offset;
                return this;
            }
        }
    }
}