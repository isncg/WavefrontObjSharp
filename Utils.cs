using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public static class Utils
    {
        public static string Dump(this Vector vector)
        {
            return string.Format("<{0}>", string.Join(", ", vector.v));
        }

        public static string Dump(List<Vector> vector, string indent = "")
        {
            return string.Join("\n", vector.ConvertAll((v => indent + v.Dump())));
        }

        public static string Dump(this Vertex vertex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for(int i=0; i< (int)Vertex.Component.Count; i++)
            {
                sb.Append(string.Format("<{0}:{1}>", (Vertex.Component)i, vertex.compIndex[i]));
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string Dump(this Mesh mesh, string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < (int)Vertex.Component.Count; i++)
            {
                sb.Append(string.Format("[{0}]:\n{1}\n", (Vertex.Component)i, Dump(mesh.data[i], indent)));
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
