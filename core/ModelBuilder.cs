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


		public List<Vector[]> SelectVertexArray(string[] names = null)
        {
			List<Vector[]> result = new List<Vector[]>();
			int count = 0;
			if (names == null)
				names = componentNames.ToArray();
			foreach (var v in corners)
            {
				Vector[] vertex = new Vector[names.Length];
				Vertex corner = corners[count];

				for(int i = 0; i < vertex.Length; i++)
                {
					int nameIndex = componentNames.FindIndex(str => str == names[i]);
					if (nameIndex >= 0)
						vertex[i] = data[names[i]][corner.compIndex[nameIndex]];
					else
						vertex[i] = null;
                }
				count++;
				result.Add(vertex);
            }
			return result;
        }
	}
}
