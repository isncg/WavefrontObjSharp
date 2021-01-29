using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    //public class Vector
    //{
    //    public float[] v;
    //    public int Dim { get; private set; }
    //    public Vector(int dim)
    //    {
    //        v = new float[dim];
    //        Dim = dim;
    //    }

    //    public static Vector Parse(string[] strs)
    //    {
    //        int dim = strs.Length > 4 ? 4 : strs.Length;
    //        Vector vec = new Vector(dim);
    //        for (int i = 0; i < dim; i++)
    //        {
    //            vec.v[i] = float.Parse(strs[i]);
    //        }
    //        return vec;
    //    }
    //}

    public class ParamVector
    {
        public class ParseException: Exception
        {
            public string rawValue;
            public int index;
        }
        public string[] values;
        public int[] GetIntValues()
        {
            int[] result = new int[values.Length];
            for(int i=0;i<result.Length;i++)
            {
                if (!int.TryParse(values[i], out result[i]))
                    throw new ParseException { rawValue = values[i], index = i };
            }
            return result;
        }

        public float[] GetFloatValues()
        {
            float[] result = new float[values.Length];
            for (int i = 0; i < result.Length; i++)
            {
                if (!float.TryParse(values[i], out result[i]))
                    throw new ParseException { rawValue = values[i], index = i };
            }
            return result;
        }
    }
}
