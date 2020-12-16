using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public class Vector
    {
        public float[] v;
        public int Dim { get; private set; }
        public Vector(int dim)
        {
            v = new float[dim];
            Dim = dim;
        }

        public static Vector Parse(string[] strs)
        {
            int dim = strs.Length > 4 ? 4 : strs.Length;
            Vector vec = new Vector(dim);
            for (int i = 0; i < dim; i++)
            {
                vec.v[i] = float.Parse(strs[i]);
            }
            return vec;
        }
    }
}
