using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class Face
    {
        public int[] attributeIndices;
    }

	public class Mesh
	{
		public string name = string.Empty;
		public Dictionary<string, List<ParamVector>> data;
		public List<ObjVertex> objVertexList;
		public ulong vertexVaiidFlag = 0;
		private List<Face> curMtlFaceList = null;
		public List<Face> CurMtlFaceList
		{
			get
			{
				if (curMtlFaceList != null) return curMtlFaceList;
				string defaultName = string.Empty;
				if (!mtl2FaceDict.TryGetValue(defaultName, out curMtlFaceList))
				{
					curMtlFaceList = new List<Face>();
					mtl2FaceDict[defaultName] = curMtlFaceList;
				}
				return curMtlFaceList;
			}
		}

		public int attrCount { get { return attrNames.Count; } }

		public bool UseMaterial(string mtlName, bool forceUse = false)
		{
			if (mtl2FaceDict.TryGetValue(mtlName, out curMtlFaceList))
			{
				return true;
			}
			if (forceUse)
			{
				curMtlFaceList = new List<Face>();
				mtl2FaceDict[mtlName] = curMtlFaceList;
				return true;
			}
			return false;
		}
		public Dictionary<string, List<Face>> mtl2FaceDict = new Dictionary<string, List<Face>>();
		public List<string> attrNames = new List<string>();
		public Mesh()
		{
			data = new Dictionary<string, List<ParamVector>>();
			objVertexList = new List<ObjVertex>();
			curMtlFaceList = null;
		}
            

        public List<ParamVector[]> Select(params string[] attrNames)
		{
			List<ParamVector[]> result = new List<ParamVector[]>();
			int vertexCount = objVertexList.Count;
			if (attrNames == null)
				attrNames = this.attrNames.ToArray();
			int vectorRowLength = attrNames.Length;
			for (int cornerIndex = 0; cornerIndex < vertexCount; cornerIndex++)
			{
				ParamVector[] vectorRow = new ParamVector[vectorRowLength];
				ObjVertex objVertex = objVertexList[cornerIndex];

				for (int i = 0; i < vectorRow.Length; i++)
				{
					string componentName = attrNames[i];
					int nameIndex = this.attrNames.FindIndex(str => str == componentName);
					if (nameIndex >= 0 && nameIndex<objVertex.attrIndex.Length)
					{
						int componentIndex = objVertex.attrIndex[nameIndex];
						var componentList = data[componentName];
						if(componentIndex>=0 && componentIndex < componentList.Count)
                        {
							vectorRow[i] = componentList[componentIndex];
						}
                        else
                        {
							vectorRow[i] = null;
                        }
					}
					else
						vectorRow[i] = null;
				}
				result.Add(vectorRow);
			}
			return result;
		}

		public uint[] GetTriangleIndices(string matName)
        {
			var faceList = mtl2FaceDict[matName];
			List<uint> triangleIndices = new List<uint>();
			foreach (var face in faceList)
			{
				int len = face.attributeIndices.Length;
				for (int i = 2; i < len; i++)
				{
					triangleIndices.Add((uint)face.attributeIndices[0]);
					triangleIndices.Add((uint)face.attributeIndices[i - 1]);
					triangleIndices.Add((uint)face.attributeIndices[i]);
				}
			}
			return triangleIndices.ToArray();
		}

	}
}
