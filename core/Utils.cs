using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public static class Utils
	{
		public static Dictionary<int, string> spaceStr = new Dictionary<int, string>();
		public static string GetSpaceStr(int len)
        {
			string result = null;
			if(spaceStr.TryGetValue(len, out result))
            {
				return result;
            }
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < len; i++)
            {
				sb.Append(" ");
            }
			result = sb.ToString();
			spaceStr[len] = result;
			return result;
        }
		public static string Dump(this Vector vector)
		{
			if (vector == null)
				return "<null>";
			return string.Format("< {0} >", string.Join(", ", new List<float>(vector.v).ConvertAll(
                (v) => {
					var str = string.Format("{0:####.000}", v);
					int strlen = str.Length;
					if (strlen < 8)
						return GetSpaceStr(8 - strlen) + str;
					else
						return str;
				})));
		}

		public static string Dump(List<Vector> vector, string indent = "")
		{
			return string.Join("\n", vector.ConvertAll((v => indent + v.Dump())));
		}

		public static string Dump(this Vertex vertex)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			int count = vertex.compIndex.Length;
			for (int i = 0; i < count; i++)
			{
				sb.Append(string.Format("<{0}>", vertex.compIndex[i]));
			}
			sb.Append("]");
			return sb.ToString();
		}

		public static string Dump(this Mesh mesh, string indent = "", string[] selectNames = null)
		{
			StringBuilder sb = new StringBuilder();
			int componentCount = mesh.componentCount;
			if (selectNames == null)
				selectNames = mesh.componentNames.ToArray();
			for (int i = 0; i < componentCount; i++)
			{
				sb.Append(string.Format("[{0}]:\n{1}\n", mesh.componentNames[i], Dump(mesh.data[mesh.componentNames[i]], indent)));
			}
			var vertexArray = mesh.CreateVectorArrayList(selectNames);
			sb.Append(string.Format("[va] ['{0}']\n", string.Join("','", selectNames)));
			foreach(var v in vertexArray)
            {
				sb.Append(indent);
				foreach(var e in v)
                {
					sb.Append(e.Dump());
                }
				sb.Append("\n");
            }

			foreach(var kv in mesh.MtlFaceDict)
            {
				sb.Append(string.Format("[mtl {0} idx]\n", kv.Key));
				List<int> triangleIndices = new List<int>();
				foreach(var face in kv.Value)
                {
					int len = face.cornerIndices.Length;
					for(int i = 2;i< len; i++)
                    {
						triangleIndices.Add(face.cornerIndices[i - 2]);
						triangleIndices.Add(face.cornerIndices[i - 1]);
						triangleIndices.Add(face.cornerIndices[i]);
					}
                }
				sb.Append(string.Format("    [{0}] [{1}]\n", triangleIndices.Count, string.Join(", ", triangleIndices)));
            }

			return sb.ToString();
		}

		

		public static Vec2 Vec2(this Vector vector)
		{
			return new Vec2 { vec = vector };
		}

		public static Vec3 Vec3(this Vector vector)
		{
			return new Vec3 { vec = vector };
		}

		public static Vec4 Vec4(this Vector vector)
		{
			return new Vec4 { vec = vector };
		}
	}
}
