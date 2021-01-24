using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public class Face
	{
		public int[] cornerIndices;
	}


	public class Mesh
	{
		public string name = string.Empty;
		public Dictionary<string, List<Vector>> data;
		public List<Vertex> corners;
		public ulong vertexVaiidFlag = 0;
		private List<Face> curMtlFaceList = null;
		public List<Face> CurMtlFaceList
		{
			get
			{
				if (curMtlFaceList != null) return curMtlFaceList;
				string defaultName = string.Empty;
				if (!MtlFaceDict.TryGetValue(defaultName, out curMtlFaceList))
				{
					curMtlFaceList = new List<Face>();
					MtlFaceDict[defaultName] = curMtlFaceList;
				}
				return curMtlFaceList;
			}
		}

		public int componentCount { get { return componentNames.Count; } }

		public bool SwitchFaceList(string name, bool createIfNotExist = false)
		{
			if (MtlFaceDict.TryGetValue(name, out curMtlFaceList))
			{
				return true;
			}
			if (createIfNotExist)
			{
				curMtlFaceList = new List<Face>();
				MtlFaceDict[name] = curMtlFaceList;
				return true;
			}
			return false;
		}
		public Dictionary<string, List<Face>> MtlFaceDict = new Dictionary<string, List<Face>>();
		public List<string> componentNames = new List<string>();
		public Mesh()
		{
			data = new Dictionary<string, List<Vector>>();
			corners = new List<Vertex>();
			curMtlFaceList = null;
		}


		public List<Vector[]> CreateVectorArrayList(string[] componentNames = null)
		{
			List<Vector[]> result = new List<Vector[]>();
			int cornerCount = corners.Count;
			if (componentNames == null)
				componentNames = this.componentNames.ToArray();
			int vectorRowLength = componentNames.Length;
			for (int cornerIndex = 0; cornerIndex < cornerCount; cornerIndex++)
			{
				Vector[] vectorRow = new Vector[vectorRowLength];
				Vertex corner = corners[cornerIndex];

				for (int i = 0; i < vectorRow.Length; i++)
				{
					string componentName = componentNames[i];
					int nameIndex = this.componentNames.FindIndex(str => str == componentName);
					if (nameIndex >= 0 && nameIndex<corner.compIndex.Length)
					{
						int componentIndex = corner.compIndex[nameIndex];
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
			var faceList = MtlFaceDict[matName];
			List<uint> triangleIndices = new List<uint>();
			foreach (var face in faceList)
			{
				int len = face.cornerIndices.Length;
				for (int i = 2; i < len; i++)
				{
					triangleIndices.Add((uint)face.cornerIndices[0]);
					triangleIndices.Add((uint)face.cornerIndices[i - 1]);
					triangleIndices.Add((uint)face.cornerIndices[i]);
				}
			}
			return triangleIndices.ToArray();
		}

	}
}
