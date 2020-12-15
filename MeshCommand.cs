using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public class ObjCommand_v : IObjCommand
	{
		public Vertex.Component Component { get; private set; }
		public ObjCommand_v(Vertex.Component component)
		{
			Component = component;
		}

		public void Execute(List<string> param, ObjCommandContext context)
		{
			var vec = Vector.Parse(param.ToArray());
			context.currentMesh.data[(int)Component].Add(vec);
		}
	}

	public class ObjCommand_f : IObjCommand
	{
        static bool VerifyVertex(Mesh mesh, Vertex vertex, ulong flag = 0)
		{
			if (flag == 0)
				flag = mesh.vertexVaiidFlag;
			if (vertex.GetValidFlag() != flag)
				return false;
			for (int i = 0; i < (int)Vertex.Component.Count; i++)
				if (vertex.compIndex[i] >= mesh.data[i].Count)
					return false;
			return true;
		}

        static int GetCornerIndex(Mesh mesh, Vertex vertex, bool addNewCorner = true)
		{
			for (int i = 0; i < mesh.corners.Count; i++)
			{
				if (mesh.corners[i].Equals(vertex))
					return i;
			}
            if(addNewCorner){
                mesh.corners.Add(vertex);
                return mesh.corners.Count - 1;
            }
            return -1;
		}
        static bool AddNewFace(Mesh mesh, List<Vertex> vertices){
            if (vertices.Count > 0)
			{
				ulong flag = mesh.vertexVaiidFlag;
				if (flag == 0)
					flag = vertices[0].GetValidFlag();
				if (flag == 0)
					return false;
				foreach (var v in vertices)
					if (!VerifyVertex(mesh, v, flag))
						return false;
                string defaultFaceName = string.Empty;
               
				mesh.CurFaceList.Add(new Face
				{
					vertexIndices = vertices.ConvertAll(
                        vertex => GetCornerIndex(mesh, vertex)).ToArray()
				});
				if (mesh.vertexVaiidFlag == 0)
					mesh.vertexVaiidFlag = flag;
				return true;
			}
			return false;
        }
		public void Execute(List<string> param, ObjCommandContext context)
		{
            if(context.currentMesh == null){
                context.currentMesh = context.CreateNewMesh("mesh");
                context.meshDict.Add(context.currentMesh.name, context.currentMesh);
            }
			var vertices = param.ConvertAll<Vertex>(str => Vertex.Parse(str));
			if (vertices.Count > 0)
			{
				if (context.currentMesh.vertexVaiidFlag == 0)
					context.currentMesh.vertexVaiidFlag = vertices[0] != null ? vertices[0].GetValidFlag() : 0;
				if (context.currentMesh.vertexVaiidFlag == 0)
					return;
				AddNewFace(context.currentMesh, vertices);
			}
		}
	}

	public class UtilCommand_dump : IObjCommand
	{
		public void Execute(List<string> param, ObjCommandContext context)
		{
			Console.WriteLine(Utils.Dump(context.currentMesh, "    "));
		}
	}


	public class ObjCommand_o : IObjCommand
	{
		public void Execute(List<string> param, ObjCommandContext context)
		{
			string name = param.Count > 0 ? param[0] : string.Empty;
			if (string.IsNullOrEmpty(name))
				name = "mesh";
			var mesh = context.CreateNewMesh(name);
			context.meshDict.Add(mesh.name, mesh);
			context.currentMesh = mesh;
		}
	}

    public class ObjCommand_usemtl : IObjCommand
    {
        public void Execute(List<string> param, ObjCommandContext context)
        {
            if(param.Count>0){
                context.currentMesh.SwitchFaceList(param[0], true);
            }
        }
    }
}