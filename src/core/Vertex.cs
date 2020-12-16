using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class Vertex: IEquatable<Vertex>
    {
        public enum Component
        {
            Position = 0,
            UV = 1,
            Noraml = 2,
            Color = 3,
            //---------//
            Count = 4
        }
        public int[] compIndex;
        public Vertex()
        {
            int componentCount = (int)Component.Count;
            compIndex = new int[componentCount];
            for (int i = 0; i < componentCount; i++)
                compIndex[i] = -1;
        }

        public ulong GetValidFlag()
        {
            ulong result = 0;
            int componentCount = (int)Component.Count;
            for (int i = 0; i < componentCount; i++)
            {
                if (compIndex[i] >= 0)
                    result |= 1ul << i;
            }
            return result;
        }


        public static Vertex Parse(string[] strs)
        {
            Vertex vertex = new Vertex();
            int componentCount = (int)Component.Count;
            int paramLen = strs.Length;

            for (int i = 0; i < componentCount; i++)
            {
                vertex.compIndex[i] = -1;
                if (i>=0 && i < paramLen && !string.IsNullOrEmpty(strs[i]))
                {
                    int.TryParse(strs[i], out vertex.compIndex[i]);
                }
            }

            return vertex;
        }

        public static Vertex Parse(string str, string sp = "/")
        {
            return Parse(str.Split(sp));
        }

        public bool Equals(Vertex other)
        {
            if (compIndex.Length != other.compIndex.Length)
                return false;
            for(int i = 0; i < compIndex.Length; i++)
            {
                if (compIndex[i] != other.compIndex[i])
                    return false;
            }
            return true;
        }
    }
}
