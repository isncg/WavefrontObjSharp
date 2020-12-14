using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class Face {
        public int[] vertexIndices;
    }


    public class Mesh
    {
        
        public List<List<Vector>> data;

        public List<Vertex> corners;
        public ulong vertexVaiidFlag = 0;


        public List<Face> faces;

        public int GetCornerIndex(Vertex vertex)
        {
            for(int i = 0; i < corners.Count; i++)
            {
                if (corners[i].Equals(vertex))
                    return i;
            }
            corners.Add(vertex);
            return corners.Count - 1;
        }

        public bool VerifyVertex(Vertex vertex, ulong flag = 0){
            if(flag == 0)
                flag = vertexVaiidFlag;
            if(vertex.GetValidFlag()!=flag)
                return false;
            for(int i=0;i<(int)Vertex.Component.Count;i++)
                if(vertex.compIndex[i]>=data[i].Count)
                    return false;
            return true;
        }
        public bool AddFace(List<Vertex> vertices){
            if(vertices.Count>0){
                ulong flag = vertexVaiidFlag;
                if(flag == 0)
                     flag = vertices[0].GetValidFlag();
                if(flag == 0)
                    return false;
                foreach(var v in vertices)
                    if(!VerifyVertex(v, flag))
                        return false;
                faces.Add(new Face{
                    vertexIndices = vertices.ConvertAll(vertex=>GetCornerIndex(vertex)).ToArray()
                });
                if(vertexVaiidFlag == 0)
                    vertexVaiidFlag = flag;
                return true;
            }
            return false;
        }


        public Mesh()
        {
            data = new List<List<Vector>>((int)Vertex.Component.Count);
            for(int i = 0; i < (int)Vertex.Component.Count; i++)
            {
                data.Add(new List<Vector>());
            }
            corners = new List<Vertex>();
            faces = new List<Face>();
        }
    }

    public class ModelBuilder
    {
    }
}
