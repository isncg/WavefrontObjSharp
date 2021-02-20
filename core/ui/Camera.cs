using GLFW;
using System;

namespace WavefrontObjSharp
{
    public class Camera
    {
        public interface Controller
        {
            void Setup(Camera camera);
        }
        public Matrix4x4 lookat { get; private set; } = new Matrix4x4();// = Matrix.LookAt(new Vector3f(2, 3, 5), new Vector3f(0, 0, 0), new Vector3f(0, 1, 0));
        public Matrix4x4 perspective { get; private set; } = new Matrix4x4();// Matrix.Perspective(60, 1.333f, 0.1f, 100);

        private Matrix4x4 _mvp = new Matrix4x4();
        public Matrix4x4 MVP => Matrix4x4.Mul(perspective, lookat, _mvp);
        //public CameraParam param = null;// new CameraParam();
        public Controller controller;

        public Vector3f GetForward(Vector3f result = null)
        {
            if (result == null) result = new Vector3f();
            result.Set(-lookat[2, 0], -lookat[2, 1], -lookat[2, 2]);
            return result;
        }

        public Vector3f GetRight(Vector3f result = null)
        {
            if (result == null) result = new Vector3f();
            result.Set(lookat[0, 0], lookat[0, 1], lookat[0, 2]);
            return result;
        }

        public Vector3f GetUp(Vector3f result = null)
        {
            if (result == null) result = new Vector3f();
            result.Set(lookat[1, 0], lookat[1, 1], lookat[1, 2]);
            return result;
        }

        public void Update()
        {
            if (controller != null)
                controller.Setup(this);
            //Matrix.LookAt(param.position, param.center, param.up, lookat);
            //Matrix.Perspective(param.fov, param.aspect, param.zNear, param.zFar, perspective);
        }
    }

    public class CameraPerspectiveController : Camera.Controller
    {
        public float fov = 60;
        public float aspect = 1.333f;
        public float zNear = 0.1f;
        public float zFar = 100f;

        public void Setup(Camera camera)
        {
            Matrix.Perspective(fov, aspect, zNear, zFar, camera.perspective);
        }
    }

    public class CameraLookatController : Camera.Controller
    {
        public Vector3f position = new Vector3f(0, 0, 10);
        public Vector3f center = new Vector3f();
        public Vector3f up = new Vector3f(0, 1, 0);

        CameraPerspectiveController perspectiveController = new CameraPerspectiveController();

        public void Setup(Camera camera)
        {
            Matrix.LookAt(position, center, up, camera.lookat);
            //Matrix.Perspective(fov, aspect, zNear, zFar, camera.perspective);
            perspectiveController.Setup(camera);
        }
    }

    public class CameraFirstPersonController : Camera.Controller
    {
        public Vector3f position = new Vector3f(0, 0, 10);
        public double heading = 0;
        public double pitch = 0;
        public CameraPerspectiveController perspectiveController = new CameraPerspectiveController();
        public float MouseSensitivityX = 0.5f;
        public float MouseSensitivityY = 0.5f;

        private Vector3f dir = new Vector3f();
        private Vector3f up = new Vector3f(0, 1, 0);
        private Vector3f[] cache = new Vector3f[] { new Vector3f(), new Vector3f(), new Vector3f() };

        public void Setup(Camera camera)
        {
            dir.X = (float)(Math.Sin(MathUtils.Radians(heading)) * Math.Cos(MathUtils.Radians(pitch)));
            dir.Y = (float)Math.Sin(MathUtils.Radians(pitch));
            dir.Z = (float)(-Math.Cos(MathUtils.Radians(heading)) * Math.Cos(MathUtils.Radians(pitch)));
            Matrix.LookAround(position, dir, up, camera.lookat, cache);

            perspectiveController.Setup(camera);
        }

        public class InputHandler
        {
            public CameraFirstPersonController controller;
            public Camera camera;
            public void EnableMouseLook(bool enable)
            {
                if (mouseLookEnable != enable)
                {
                    if (enable)
                    {
                        StartTrackMousePos();
                    }
                }
            }
            private void StartTrackMousePos()
            {
                if (Input.GetMousePos(out var x, out var y))
                {
                    mouseBeginX = x;
                    mouseBeginY = y;
                    mouseBeginPitch = controller.pitch;
                    mouseBeginHeading = controller.heading;
                    mouseLookEnable = true;
                    //Console.WriteLine(string.Format("mouseBeginX:{0}, mouseBeginY:{1}, mouseBeginPitch:{2}, mouseBeginHeading:{3}, mouseLookEnable:{4}",
                    //    mouseBeginX, mouseBeginY, mouseBeginPitch, mouseBeginHeading, mouseLookEnable));
                }
                //else
                //{
                //    Console.WriteLine("Cannot get mouse pos");
                //}
            }
            private bool mouseLookEnable = false;
            private double mouseBeginX = 0;
            private double mouseBeginY = 0;
            private double mouseBeginPitch = 0;
            private double mouseBeginHeading = 0;

            public void Update()
            {
                if (Input.GetKey(Keys.A))
                    controller.position -= 1f * Time.DeltaTime * camera.GetRight();
                if (Input.GetKey(Keys.D))
                    controller.position += 1f * Time.DeltaTime * camera.GetRight();
                if (Input.GetKey(Keys.W))
                    controller.position += 1f * Time.DeltaTime * camera.GetForward();
                if (Input.GetKey(Keys.S))
                    controller.position -= 1f * Time.DeltaTime * camera.GetForward();

                if (Input.GetKey(Keys.Up))
                    controller.pitch += 10 * Time.DeltaTime;
                if (Input.GetKey(Keys.Down))
                    controller.pitch -= 10 * Time.DeltaTime;
                if (Input.GetKey(Keys.Left))
                    controller.heading -= 10 * Time.DeltaTime;
                if (Input.GetKey(Keys.Right))
                    controller.heading += 10 * Time.DeltaTime;

                if (mouseLookEnable)
                {
                    if (Input.GetKey(Keys.Up) || Input.GetKey(Keys.Down) || Input.GetKey(Keys.Left) || Input.GetKey(Keys.Right) || !MouseState.current.IsMouseHover())
                    {
                        StartTrackMousePos();
                    }
                    else
                    {
                        if (Input.GetMousePos(out var x, out var y))
                        {
                            var dx = x - mouseBeginX;
                            var dy = y - mouseBeginY;
                            controller.pitch = mouseBeginPitch - dy * controller.MouseSensitivityY;
                            controller.heading = mouseBeginHeading + dx * controller.MouseSensitivityX;
                            if (controller.pitch > 90)
                            {
                                controller.pitch = 90;
                                StartTrackMousePos();
                            }
                            if (controller.pitch < -90)
                            {
                                controller.pitch = -90;
                                StartTrackMousePos();
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(Keys.LeftAlt))
                {
                    if (mouseLookEnable)
                        mouseLookEnable = false;
                    else
                    {
                        mouseLookEnable = true;
                        StartTrackMousePos();
                    }

                }

                controller.Setup(camera);
            }
        }
    }
}