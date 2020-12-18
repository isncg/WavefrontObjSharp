using System.Collections.Generic;

namespace WavefrontObjSharp
{
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
			if (addNewCorner)
			{
				mesh.corners.Add(vertex);
				return mesh.corners.Count - 1;
			}
			return -1;
		}
		static bool AddNewFace(Mesh mesh, List<Vertex> vertices)
		{
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
					cornerIndices = vertices.ConvertAll(
						vertex => GetCornerIndex(mesh, vertex)).ToArray()
				});
				if (mesh.vertexVaiidFlag == 0)
					mesh.vertexVaiidFlag = flag;
				return true;
			}
			return false;
		}
		public void Execute(List<string> param, ObjModel model)
		{
			var vertices = param.ConvertAll<Vertex>(str => Vertex.Parse(str));
			if (vertices.Count > 0)
			{
				if (model.CurrentMesh.vertexVaiidFlag == 0)
					model.CurrentMesh.vertexVaiidFlag = vertices[0] != null ? vertices[0].GetValidFlag() : 0;
				if (model.CurrentMesh.vertexVaiidFlag == 0)
					return;
				AddNewFace(model.CurrentMesh, vertices);
			}
		}
	}
}