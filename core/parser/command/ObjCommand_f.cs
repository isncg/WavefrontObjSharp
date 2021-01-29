using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjCommand_f : IObjCommand
	{
		static bool VerifyVertex(Mesh mesh, ObjVertex vertex, ulong flag = 0)
		{
			if (flag == 0)
				flag = mesh.vertexVaiidFlag;
			if (vertex.GetValidFlag() != flag)
				return false;
			int componentCount = mesh.attrCount;
			for (int i = 0; i < componentCount; i++)
				if (vertex.attrIndex[i] >= mesh.data[mesh.attrNames[i]].Count)
					return false;
			return true;
		}

		static int GetCornerIndex(Mesh mesh, ObjVertex vertex, bool addNewCorner = true)
		{
			for (int i = 0; i < mesh.objVertexList.Count; i++)
			{
				if (mesh.objVertexList[i].Equals(vertex))
					return i;
			}
			if (addNewCorner)
			{
				mesh.objVertexList.Add(vertex);
				return mesh.objVertexList.Count - 1;
			}
			return -1;
		}
		static bool AddNewFace(Mesh mesh, List<ObjVertex> vertices)
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

				mesh.CurMtlFaceList.Add(new Face
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
			var vertices = param.ConvertAll<ObjVertex>(
                (str) => {
					var vertex = ObjVertex.Parse(model.CurrentMesh, str);
					for(int i = 0; i < vertex.attrIndex.Length; i++)
                    {
						vertex.attrIndex[i] -= 1;
                    }
					return vertex;
				}
			);
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