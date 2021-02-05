namespace WavefrontObjSharp
{
    public class Camera
    {
        public Matrix4x4 lookat = new Matrix4x4();// = Matrix.LookAt(new Vector3f(2, 3, 5), new Vector3f(0, 0, 0), new Vector3f(0, 1, 0));
        public Matrix4x4 perspective = new Matrix4x4();// Matrix.Perspective(60, 1.333f, 0.1f, 100);
        public CameraParam param = null;// new CameraParam();

        public Camera(CameraParam param = null)
        {
            this.param = param != null ? param : new CameraParam();
            Update();
        }

        public void Update()
        {
            Matrix.LookAt(param.position, param.center, param.up, lookat);
            Matrix.Perspective(param.fov, param.aspect, param.zNear, param.zFar, perspective);
        }
    }

    public class CameraParam
    {
        public Vector3f position = new Vector3f(0,0,10);
        public Vector3f center = new Vector3f();
        public Vector3f up = new Vector3f(0,1,0);
        public float fov = 60;
        public float aspect = 1.333f;
        public float zNear = 0.1f;
        public float zFar = 100f;
    }
}