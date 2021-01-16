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
            if (info == null)
                info = new Info();
            if (buffer != null)
            {
                info.length = buffer.Length;
                if (buffer.Length > 0)
                {
                    fixed (float* pointer = &buffer[0])
                    {
                        info.pointer = pointer;
                    }
                }
            }
            return info;
        }
        protected float[] buffer = null;
        public float[] GetArray() { return buffer; }
        public float this[int index]
        {
            get
            {
                return buffer[index];
            }
            set
            {
                buffer[index] = value;
            }
        }
    }

    public class Vector3f: FloatBuffer
    {
        private int px, py, pz;
        public Vector3f() { buffer = new float[3]; px = 0;py = 1;pz = 2; }
        public Vector3f(float[] buffer, int px, int py, int pz) { this.buffer = buffer; this.px = px; this.py = py; this.pz = pz; }
        public float X { get { return buffer[px]; } set { buffer[px] = value; } }
        public float Y { get { return buffer[py]; } set { buffer[py] = value; } }
        public float Z { get { return buffer[pz]; } set { buffer[pz] = value; } }

      

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

        public static Vector3f Mul(Vector3f l, float r, Vector3f result = null)
        {
            if (result == null)
                result = new Vector3f();
            result.X = l.X * r;
            result.Y = l.Y * r;
            result.Z = l.Z * r;
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

        public static float operator *(Vector3f l, Vector3f r)
        {
            return l.X * r.X + l.Y * r.Y + l.Z * r.Z;
        }

        public static Vector3f operator *(Vector3f l, float r)
        {
            return Mul(l, r);
        }

        public static Vector3f operator *(float l, Vector3f r)
        {
            return Mul(r, l);
        }
    }

    public class Vector4f: FloatBuffer
    {
        private int px, py, pz,pw;
        public Vector4f() { buffer = new float[4]; px = 0; py = 1; pz = 2; pw = 3; }
        public Vector4f(float[] buffer, int px, int py, int pz, int pw) { this.buffer = buffer; this.px = px; this.py = py; this.pz = pz; this.pw = pw; }
        public float X { get { return buffer[px]; } set { buffer[px] = value; } }
        public float Y { get { return buffer[py]; } set { buffer[py] = value; } }
        public float Z { get { return buffer[pz]; } set { buffer[pz] = value; } }
        public float W { get { return buffer[pw]; } set { buffer[pw] = value; } }

        public void Set(Vector4f xyzw)
        {
            X = xyzw.X; Y = xyzw.Y; Z = xyzw.Z; W = xyzw.W;
        }

        public void Set(Vector3f xyz, float w = 0)
        {
            X = xyz.X; Y = xyz.Y; Z = xyz.Z; W = w;
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

        public static Vector4f Mul(Vector4f l, float r, Vector4f result = null)
        {
            if (result == null)
                result = new Vector4f();
            result.X = l.X * r;
            result.Y = l.Y * r;
            result.Z = l.Z * r;
            result.W = l.W * r;
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

        public static float operator *(Vector4f l, Vector4f r)
        {
            return l.X * r.X + l.Y * r.Y + l.Z * r.Z + l.W * r.W;
        }

        public static Vector4f operator *(Vector4f l, float r)
        {
            return Mul(l, r);
        }

        public static Vector4f operator *(float l, Vector4f r)
        {
            return Mul(r, l);
        }
    }

    public class Matrix3x3 : FloatBuffer
    {
        public Vector3f[] rows = null;
        public Vector3f[] cols = null;
        public Matrix3x3() {
            buffer = new float[9];
            rows = new Vector3f[3]
            {
                new Vector3f(buffer,0,3,6),
                new Vector3f(buffer,1,4,7),
                new Vector3f(buffer,2,5,8),
            };
            cols = new Vector3f[3]
            {
                new Vector3f(buffer,0,1,2),
                new Vector3f(buffer,3,4,5),
                new Vector3f(buffer,6,7,8),
            };
        }

        public float this[int i, int j]
        {
            get
            {
                return rows[i][j];
            }
            set
            {
                rows[i][j] = value;
            }
        }
        public static Matrix3x3 Mul(Matrix3x3 l, Matrix3x3 r, Matrix3x3 result = null)
        {
            if (result == null)
                result = new Matrix3x3();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    result.rows[i][j] = l.rows[i] * r.cols[j];
            return result;
        }

        public static Matrix3x3 Mul(Matrix3x3 l, float r, Matrix3x3 result = null)
        {
            if (result == null)
                result = new Matrix3x3();
            for (int i = 0; i < 9; i++)
                result[i] = l[i] * r;
            return result;
        }

        public static Vector3f Mul(Matrix3x3 l, Vector3f r, Vector3f result = null)
        {
            if (result == null)
                result = new Vector3f();
            result[0] = l.rows[0] * r;
            result[1] = l.rows[1] * r;
            result[2] = l.rows[2] * r;
            return result;
        }

        public static Matrix3x3 operator*(Matrix3x3 l, Matrix3x3 r)
        {
            return Mul(l, r);
        }

        public static Matrix3x3 operator *(Matrix3x3 l, float r)
        {
            return Mul(l, r);
        }

        public static Matrix3x3 operator *(float l, Matrix3x3 r)
        {
            return Mul(r, l);
        }

        public static Vector3f operator*(Matrix3x3 l, Vector3f r)
        {
            return Mul(l, r);
        }

        public static Matrix3x3 I(float e = 1, Matrix3x3 result = null)
        {
            if (result == null)
                result = new Matrix3x3();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    result[i,j] = i == j ? e : 0;

            return result;
        }
    }

    public class Matrix4x4: FloatBuffer
    {
        public Vector4f[] rows = null;
        public Vector4f[] cols = null;
        public Matrix4x4() {
            buffer = new float[16];
            rows = new Vector4f[4]
            {
                new Vector4f(buffer,0,4,8,12),
                new Vector4f(buffer,1,5,9,13),
                new Vector4f(buffer,2,6,10,14),
                new Vector4f(buffer,3,7,11,15)
            };

            cols = new Vector4f[4]
            {
                new Vector4f(buffer,0,1,2,3),
                new Vector4f(buffer,4,5,6,7),
                new Vector4f(buffer,8,9,10,11),
                new Vector4f(buffer,12,13,14,15)
            };
        }

        public float this[int i, int j]
        {
            get
            {
                return rows[i][j];
            }
            set
            {
                rows[i][j] = value;
            }
        }
      

        public static Matrix4x4 Add(Matrix4x4 l, Matrix4x4 r, Matrix4x4 result = null)
        {
            if (result == null)
                result = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                result[i] = l[i] + r[i];
            return result;
        }

        public static Matrix4x4 Sub(Matrix4x4 l, Matrix4x4 r, Matrix4x4 result = null)
        {
            if (result == null)
                result = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                result[i] = l[i] - r[i];
            return result;
        }

        public static Matrix4x4 Mul(Matrix4x4 l, Matrix4x4 r, Matrix4x4 result = null)
        {
            if (result == null)
                result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    result[i,j] = l.rows[i] * r.cols[j];
            return result;
        }

        public static Matrix4x4 Mul(Matrix4x4 l, float r, Matrix4x4 result = null)
        {
            if (result == null)
                result = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                result[i] = l[i] * r;
            return result;
        }

        public static Vector4f Mul(Matrix4x4 l,  Vector4f r, Vector4f result = null)
        {
            if (result == null)
                result = new Vector4f();
            result[0] = l.rows[0] * r;
            result[1] = l.rows[1] * r;
            result[2] = l.rows[2] * r;
            result[3] = l.rows[3] * r;
            return result;
        }

        public static Matrix4x4 operator+(Matrix4x4 l, Matrix4x4 r)
        {
            return Add(l, r);
        }

        public static Matrix4x4 operator -(Matrix4x4 l, Matrix4x4 r)
        {
            return Sub(l, r);
        }

        public static Matrix4x4 operator*(Matrix4x4 l, Matrix4x4 r)
        {
            return Mul(l, r);
        }

        public static Matrix4x4 operator *(Matrix4x4 l, float r)
        {
            return Mul(l, r);
        }

        public static Vector4f operator *(Matrix4x4 l, Vector4f r)
        {
            return Mul(l, r);
        }

        public static Matrix4x4 I(float e = 1, Matrix4x4 result = null)
        {
            if (result == null)
                result = new Matrix4x4();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    result[i,j] = i == j ? e : 0;

            return result;
        }
    }
}