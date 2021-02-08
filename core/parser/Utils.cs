using System;
using System.Collections.Generic;
using System.IO;
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
		public static string Dump(this ParamVector vector)
		{
			if (vector == null)
				return "<null>";
			return string.Format("< {0} >", string.Join(", ", new List<string>(vector.values).ConvertAll(
                (v) => {
					var str = v;// string.Format("{0:####.000}", v);
					int strlen = str.Length;
					if (strlen < 9)
						return GetSpaceStr(9 - strlen) + str;
					else
						return str;
				})));
		}

		public static string Dump(List<ParamVector> vector, string indent = "")
		{
			return string.Join("\n", vector.ConvertAll((v => indent + v.Dump())));
		}

		public static string Dump(this ObjVertex vertex)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			int count = vertex.attrIndex.Length;
			for (int i = 0; i < count; i++)
			{
				sb.Append(string.Format("<{0}>", vertex.attrIndex[i]));
			}
			sb.Append("]");
			return sb.ToString();
		}

		public static string Dump(this Mesh mesh, string indent = "", string[] selectNames = null)
		{
			StringBuilder sb = new StringBuilder();
			int componentCount = mesh.attrCount;
			if (selectNames == null)
				selectNames = mesh.attrNames.ToArray();
			for (int i = 0; i < componentCount; i++)
			{
				sb.Append(string.Format("[{0}]:\n{1}\n", mesh.attrNames[i], Dump(mesh.data[mesh.attrNames[i]], indent)));
			}
			var vertexArray = mesh.Select(selectNames);
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

			foreach(var kv in mesh.mtl2FaceDict)
            {
				sb.Append(string.Format("[mtl {0} idx]\n", kv.Key));
				List<int> triangleIndices = new List<int>();
				foreach(var face in kv.Value)
                {
					int len = face.attributeIndices.Length;
					for(int i = 2;i< len; i++)
                    {
						triangleIndices.Add(face.attributeIndices[i - 2]);
						triangleIndices.Add(face.attributeIndices[i - 1]);
						triangleIndices.Add(face.attributeIndices[i]);
					}
                }
				sb.Append(string.Format("    [{0}] [{1}]\n", triangleIndices.Count, string.Join(", ", triangleIndices)));
            }

			return sb.ToString();
		}
		
        public static string GetDataFilePath(string path)
        {
            var dir = Directory.GetCurrentDirectory();
            if (path == null || path.Length < 1)
            {
                return null;
            }
            if (path[0] != '\\' && path[0] != '/')
            {
                path = "/" + path;
            }
            while (!File.Exists(dir + path))
            {
                DirectoryInfo dirInfo = Directory.GetParent(dir);
                if (dirInfo == null)
                {
                    Console.WriteLine("Search end at dir" + dir);
                    return null;
                }
                dir = dirInfo.FullName;
            }
            return dir + path;
        }

		public static StreamReader GetStreamReader(string path)
		{
			var dir = Directory.GetCurrentDirectory();
            if (path == null || path.Length < 1)
            {
				return null;
            }
			if(path[0]!='\\' && path[0] != '/')
            {
				path = "/" + path;
            }
			while (!File.Exists(dir + path))
			{
				DirectoryInfo dirInfo = Directory.GetParent(dir);
				if (dirInfo == null)
				{
					Console.WriteLine("Search end at dir" + dir);
					return null;
				}
				dir = dirInfo.FullName;
			}
			StreamReader reader = null;
			try
			{
				reader = File.OpenText(dir + path);
			}
			catch (Exception e)
			{
				Console.WriteLine(string.Format("Cannot open file {0}:{1}", dir + path, e.Message));
			}
			return reader;
		}
	}
}
