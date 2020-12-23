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
		private List<Face> curFaceList = null;
		public List<Face> CurFaceList
		{
			get
			{
				if (curFaceList != null) return curFaceList;
				string defaultName = string.Empty;
				if (!mtlFaceList.TryGetValue(defaultName, out curFaceList))
				{
					curFaceList = new List<Face>();
					mtlFaceList[defaultName] = curFaceList;
				}
				return curFaceList;
			}
		}

		public int componentCount { get { return componentNames.Count; } }

		public bool SwitchFaceList(string name, bool createIfNotExist = false)
		{
			if (mtlFaceList.TryGetValue(name, out curFaceList))
			{
				return true;
			}
			if (createIfNotExist)
			{
				curFaceList = new List<Face>();
				mtlFaceList[name] = curFaceList;
				return true;
			}
			return false;
		}
		public Dictionary<string, List<Face>> mtlFaceList = new Dictionary<string, List<Face>>();
		public List<string> componentNames = new List<string>();
		public Mesh()
		{
			data = new Dictionary<string, List<Vector>>();//new List<List<Vector>>((int)Vertex.Component.Count);
														  //for (int i = 0; i < (int)Vertex.Component.Count; i++)
														  //{
														  //	data.Add(new List<Vector>());
														  //}
			corners = new List<Vertex>();
			curFaceList = new List<Face>();
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
	}
}
