using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class MeshVertexCommand : IObjCommand
    {
        public Vertex.Component Component { get; private set; }
        public MeshVertexCommand(Vertex.Component component)
        {
            Component = component;
        }

        public void Execute(List<string> param, ObjCommandContext context)
        {
            var vec = Vector.Parse(param.ToArray());
            context.currentMesh.data[(int)Component].Add(vec);
        }

        public static MeshVertexCommand Create(string name)
        {
            if (name == "v")
                return new MeshVertexCommand(Vertex.Component.Position);
            else if (name == "vt")
                return new MeshVertexCommand(Vertex.Component.UV);
            else if (name == "vn")
                return new MeshVertexCommand(Vertex.Component.Noraml);
            return null;
        }
    }

    public class MeshFaceCommand : IObjCommand
    {
        public void Execute(List<string> param, ObjCommandContext context)
        {
            var vertices = param.ConvertAll<Vertex>(str => Vertex.Parse(str));
            if (vertices.Count > 0)
            {
                if (context.currentMesh.vertexVaiidFlag == 0)
                    context.currentMesh.vertexVaiidFlag = vertices[0] != null ? vertices[0].GetValidFlag() : 0;
                if (context.currentMesh.vertexVaiidFlag == 0)
                    return;
                context.currentMesh.AddFace(vertices);
            }
        }
    }

    public class MeshDumpCommand : IObjCommand
    {
        public void Execute(List<string> param, ObjCommandContext context)
        {
            Console.WriteLine(Utils.Dump(context.currentMesh, "    "));
        }
    }
}