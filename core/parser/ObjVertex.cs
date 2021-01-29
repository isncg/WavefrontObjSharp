using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class ObjVertex: IEquatable<ObjVertex>
    {
        public int[] attrIndex;
        public ObjVertex(int attrCount)
        {
            attrIndex = new int[attrCount];
            for (int i = 0; i < attrCount; i++)
                attrIndex[i] = -1;
        }

        public ulong GetValidFlag()
        {
            ulong result = 0;
            for (int i = 0; i < attrIndex.Length; i++)
            {
                if (attrIndex[i] >= 0)
                    result |= 1ul << i;
            }
            return result;
        }


        public static ObjVertex Parse(Mesh mesh,  string[] strs)
        {
            ObjVertex vertex = new ObjVertex(mesh.attrCount);
            int componentCount = mesh.attrCount;
            int paramLen = strs.Length;

            for (int i = 0; i < componentCount; i++)
            {
                vertex.attrIndex[i] = -1;
                if (i>=0 && i < paramLen && !string.IsNullOrEmpty(strs[i]))
                {
                    int.TryParse(strs[i], out vertex.attrIndex[i]);
                }
            }

            return vertex;
        }

        public static ObjVertex Parse(Mesh mesh, string str, string sp = "/")
        {
            return Parse(mesh, str.Split(sp));
        }

        public bool Equals(ObjVertex other)
        {
            if (attrIndex.Length != other.attrIndex.Length)
                return false;
            for(int i = 0; i < attrIndex.Length; i++)
            {
                if (attrIndex[i] != other.attrIndex[i])
                    return false;
            }
            return true;
        }
    }
}
