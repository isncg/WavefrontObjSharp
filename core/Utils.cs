using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public static class Utils
	{
		public static string Dump(this Vector vector)
		{
			return string.Format("<{0}>", string.Join(", ", new List<float>(vector.v).ConvertAll(v => string.Format("{0,9}", v))));
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
			int count = mesh.componentCount;
			for (int i = 0; i < count; i++)
			{
				sb.Append(string.Format("[{0}]:\n{1}\n", mesh.componentNames[i], Dump(mesh.data[mesh.componentNames[i]], indent)));
			}
			var vertexArray = mesh.SelectVertexArray(selectNames);
			foreach(var v in vertexArray)
            {
				foreach(var e in v)
                {
					sb.Append(e.Dump());
                }
				sb.Append("\n");
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
