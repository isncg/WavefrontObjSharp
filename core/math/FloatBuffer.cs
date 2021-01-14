namespace WavefrontObjSharp
{
    public class FloatBuffer
    {
        public unsafe class Info
        {
            public float* pointer = null;
            public int length = 0;
        }

        public unsafe Info GetBufferInfo(Info info = null)
        {
            if(info==null)
                info = new Info();
            if (buffer != null)
            {
                info.length = buffer.Length;
                if (buffer.Length > 0)
                {
                    fixed(float* pointer = &buffer[0])
                    {
                        info.pointer = pointer;
                    }
                }
            }
            return info;
        }
        protected float[] buffer = null;
        public float[] GetArray() { return buffer; }
    }

    public class Vector3f: FloatBuffer
    {
        private int px, py, pz;
        public Vector3f() { buffer = new float[3]; px = 0;py = 1;pz = 2; }
        public Vector3f(float[] buffer, int px, int py, int pz) { this.buffer = buffer; this.px = px; this.py = py; this.pz = pz; }
        public float X { get { return buffer[px]; } set { buffer[px] = value; } }
        public float Y { get { return buffer[py]; } set { buffer[py] = value; } }
        public float Z { get { return buffer[pz]; } set { buffer[pz] = value; } }

        public static float operator*(Vector3f l, Vector3f r)
        {
            return l.X * r.X + l.Y * r.Y + l.Z * r.Z;
        }

        public static Vector3f Add(Vector3f l, Vector3f r, Vector3f result = null)
        {
            if (result == null)
                result = new Vector3f();
            result.X = l.X + r.X;
            result.Y = l.Y + r.Y;
            result.Z = l.Z + r.Z;
            return result;
        }

        public static Vector3f Sub(Vector3f l, Vector3f r, Vector3f result = null)
        {
            if (result == null)
                result = new Vector3f();
            result.X = l.X - r.X;
            result.Y = l.Y - r.Y;
            result.Z = l.Z - r.Z;
            return result;
        }

        public static Vector3f Cross(Vector3f l, Vector3f r, Vector3f result = null)
        {
            if (result == null)
                result = new Vector3f();
            result.X = l.Y * r.Z - r.Y * l.Z;
            result.Y = r.X * l.Z - l.X * r.Z;
            result.Z = l.X * r.Y - r.X * l.Y;
            return result;
        }

        public static Vector3f operator+(Vector3f l, Vector3f r)
        {
            return Add(l, r);
        }

        public static Vector3f operator -(Vector3f l, Vector3f r)
        {
            return Sub(l, r);
        }
    }

    public class Vector4f: FloatBuffer
    {
        private int px, py, pz,pw;
        public Vector4f() { buffer = new float[4]; px = 0; py = 1; pz = 2; pw = 3; }
        public Vector4f(float[] buffer, int px, int py, int pz, int pw) { this.buffer = buffer; this.px = px; this.py = py; this.pz = pz; this.pw = pw; }
        public float X { get { return buffer[0]; } set { buffer[0] = value; } }
        public float Y { get { return buffer[1]; } set { buffer[1] = value; } }
        public float Z { get { return buffer[2]; } set { buffer[2] = value; } }
        public float W { get { return buffer[2]; } set { buffer[2] = value; } }

        public static float operator *(Vector4f l, Vector4f r)
        {
            return l.X * r.X + l.Y * r.Y + l.Z * r.Z + l.W*r.W;
        }

        public static Vector4f Add(Vector4f l, Vector4f r, Vector4f result = null)
        {
            if (result == null)
                result = new Vector4f();
            result.X = l.X + r.X;
            result.Y = l.Y + r.Y;
            result.Z = l.Z + r.Z;
            result.W = l.W + r.W;
            return result;
        }

        public static Vector4f Sub(Vector4f l, Vector4f r, Vector4f result = null)
        {
            if (result == null)
                result = new Vector4f();
            result.X = l.X - r.X;
            result.Y = l.Y - r.Y;
            result.Z = l.Z - r.Z;
            result.W = l.W - r.W;
            return result;
        }

        public static Vector4f operator +(Vector4f l, Vector4f r)
        {
            return Add(l, r);
        }

        public static Vector4f operator -(Vector4f l, Vector4f r)
        {
            return Sub(l, r);
        }

    }

    public class Matrix3x3 : FloatBuffer
    {
        public Matrix3x3() { buffer = new float[9]; }
        public float M11 { get { return buffer[0]; } set { buffer[0] = value; } }
        public float M12 { get { return buffer[1]; } set { buffer[1] = value; } }
        public float M13 { get { return buffer[2]; } set { buffer[2] = value; } }
        public float M21 { get { return buffer[3]; } set { buffer[3] = value; } }
        public float M22 { get { return buffer[4]; } set { buffer[4] = value; } }
        public float M23 { get { return buffer[5]; } set { buffer[5] = value; } }
        public float M31 { get { return buffer[6]; } set { buffer[6] = value; } }
        public float M32 { get { return buffer[7]; } set { buffer[7] = value; } }
        public float M33 { get { return buffer[8]; } set { buffer[8] = value; } }
    }

    public class Matrix4x4: FloatBuffer
    {
        public Matrix4x4() { buffer = new float[16]; }
        public float M11 { get { return buffer[0]; } set { buffer[0] = value; } }
        public float M12 { get { return buffer[1]; } set { buffer[1] = value; } }
        public float M13 { get { return buffer[2]; } set { buffer[2] = value; } }
        public float M14 { get { return buffer[3]; } set { buffer[3] = value; } }
        public float M21 { get { return buffer[4]; } set { buffer[4] = value; } }
        public float M22 { get { return buffer[5]; } set { buffer[5] = value; } }
        public float M23 { get { return buffer[6]; } set { buffer[6] = value; } }
        public float M24 { get { return buffer[7]; } set { buffer[7] = value; } }
        public float M31 { get { return buffer[8]; } set { buffer[8] = value; } }
        public float M32 { get { return buffer[9]; } set { buffer[9] = value; } }
        public float M33 { get { return buffer[10]; } set { buffer[10] = value; } }
        public float M34 { get { return buffer[11]; } set { buffer[11] = value; } }
        public float M41 { get { return buffer[12]; } set { buffer[12] = value; } }
        public float M42 { get { return buffer[13]; } set { buffer[13] = value; } }
        public float M43 { get { return buffer[14]; } set { buffer[14] = value; } }
        public float M44 { get { return buffer[15]; } set { buffer[15] = value; } }
    }
}