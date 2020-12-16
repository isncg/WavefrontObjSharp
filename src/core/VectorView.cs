using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
    public abstract class VectorView
    {
        public Vector vec;

        protected float Get(int index)
        {
            if (index < vec.Dim)
                return vec.v[index];
            return 0;
        }

        protected void Set(int index, float value)
        {
            if(index < vec.Dim)
                vec.v[index] = value;
        }

        public static Vec2 Vec2()
        {
            return new Vec2 { vec = new Vector(2) };
        }

        public static Vec3 Vec3()
        {
            return new Vec3 { vec = new Vector(3) };
        }

        public static Vec4 Vec4()
        {
            return new Vec4 { vec = new Vector(4) };
        }
    }

    public class Vec2: VectorView
    {
        public float X { get { return Get(0); } set { Set(0, value); } }
        public float Y { get { return Get(1); } set { Set(1, value); } }
    }

    public class Vec3 : VectorView
    {
        public float X { get { return Get(0); } set { Set(0, value); } }
        public float Y { get { return Get(1); } set { Set(1, value); } }
        public float Z { get { return Get(2); } set { Set(2, value); } }
    }

    public class Vec4 : VectorView
    {
        public float X { get { return Get(0); } set { Set(0, value); } }
        public float Y { get { return Get(1); } set { Set(1, value); } }
        public float Z { get { return Get(2); } set { Set(2, value); } }
        public float W { get { return Get(3); } set { Set(3, value); } }
    }
}
