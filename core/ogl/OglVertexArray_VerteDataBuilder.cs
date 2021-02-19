using System;
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

            public void AddVertex(List<string[]> vertexAttributeValuesList)
            {
                if (vertexAttributeValuesList.Count == parent.attributes.Count)
                {
                    var bytes = new Bytes();
                    for (int i = 0; i < vertexAttributeValuesList.Count; i++)
                    {
                        var size = parent.attributes[i].size;
                        var type = parent.attributes[i].type;
                        var strList = new List<string>(vertexAttributeValuesList[i]);
                        while (strList.Count < size)
                        {
                            strList.Add("0");
                        }
                        try
                        {
                            if (type == GL_FLOAT)
                            {
                                bytes.AddRange(strList.ConvertAll(v => float.Parse(v)));
                            }
                            else
                            {
                                bytes.AddRange(strList.ConvertAll(v => int.Parse(v)));
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    int count = bytes.Count;
                    for (int i = 0; i < parent.vertexByteSize; i++)
                    {
                        if (i < count)
                            buildResult.Add(bytes[i]);
                        else
                            buildResult.Add(0);
                    }
                }
            }

            public void AddVertex(params object[] vertexAttributeValuesList)
            {
                AddVertex(new List<object>(vertexAttributeValuesList));
            }


            public void AddVertex(List<object> vertexAttributeValuesList)
            {
                if (vertexAttributeValuesList.Count == parent.attributes.Count)
                {
                    var bytes = new Bytes();
                    for (int i = 0; i < vertexAttributeValuesList.Count; i++)
                    {
                        var size = parent.attributes[i].size;
                        var type = parent.attributes[i].type;

                        if (vertexAttributeValuesList[i] is IEnumerable<string>)
                        {
                            var strList =  new List<string>(vertexAttributeValuesList[i] as IEnumerable<string>);
                            while (strList.Count < size)
                                strList.Add("0");
                            try
                            {
                                if (type == GL_FLOAT)
                                    bytes.AddRange(strList.ConvertAll(v => float.Parse(v)));
                                else
                                    bytes.AddRange(strList.ConvertAll(v => int.Parse(v)));
                            }
                            catch (Exception e)
                            {
                            }
                        }

                        else if (vertexAttributeValuesList[i] is IEnumerable<float>)
                        {
                            var floatList = new List<float>(vertexAttributeValuesList[i] as IEnumerable<float>);
                            while (floatList.Count < size)
                                floatList.Add(0);
                            try
                            {
                                if (type == GL_FLOAT)
                                    bytes.AddRange(floatList);
                                else
                                    bytes.AddRange(floatList.ConvertAll(v =>(int)v));
                            }
                            catch (Exception e)
                            {
                            }
                        }

                        else if (vertexAttributeValuesList[i] is IEnumerable<int>)
                        {
                            var intList = new List<int>(vertexAttributeValuesList[i] as IEnumerable<int>);
                            while (intList.Count < size)
                                intList.Add(0);
                            try
                            {
                                if (type == GL_FLOAT)
                                    bytes.AddRange(intList.ConvertAll(v=>(float)v));
                                else
                                    bytes.AddRange(intList);
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("Unknown datatype: {0}", vertexAttributeValuesList[i].GetType()));
                        }
                    }
                    int count = bytes.Count;
                    for (int i = 0; i < parent.vertexByteSize; i++)
                    {
                        if (i < count)
                            buildResult.Add(bytes[i]);
                        else
                            buildResult.Add(0);
                    }
                }
            }


            public void AddIndices(Primitive.Type type, uint[] indices)
            {
                parent.primitives.Add(new Primitive { type = type, indices = indices });
            }
        }
    }
}