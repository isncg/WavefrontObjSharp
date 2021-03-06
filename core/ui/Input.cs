using GLFW;
using System;
using System.Collections.Generic;
using KeyCode = GLFW.Keys;

namespace WavefrontObjSharp
{



    public static class Input
    {
        public interface IFrameHandler
        {
            void FrameUpdate();
        }


        private static HashSet<IFrameHandler> handlers;
        public static void RegisterHandler(IFrameHandler handler)
        {
            if (null == handlers)
                handlers = new HashSet<IFrameHandler>();
            handlers.Add(handler);
        }

        public static void UnRegisterHandler(IFrameHandler handler)
        {
            if (null != handlers)
                handlers.Remove(handler);
        }

        public static void AllHandlersFrameUpdate()
        {
            if (null != handlers)
                foreach (var h in handlers)
                    if (null != h)
                        h.FrameUpdate();
        }

        public static bool GetKey(KeyCode keyCode)
        {
            return KeyState.current != null && KeyState.current.keyHold.Contains((int)keyCode);
        }
        public static bool GetKeyDown(KeyCode keyCode)
        {
            return KeyState.current != null && KeyState.current.frameKeyDown.Contains((int)keyCode);
        }
        public static bool GetKeyUp(KeyCode keyCode)
        {
            return KeyState.current != null && KeyState.current.frameKeyUp.Contains((int)keyCode);
        }

        public static bool GetMousePos(out double x, out double y)
        {
            if (MouseState.current != null)
            {
                MouseState.current.GetCursorPosition(out x, out y);
                return true;
            }
            x = 0; y = 0;
            return false;
        }
    }


    public class KeyState
    {
        public static KeyState current;
        public HashSet<int> keyHold = new HashSet<int>();
        public HashSet<int> frameKeyDown = new HashSet<int>();
        public HashSet<int> frameKeyUp = new HashSet<int>();

        protected void OnKeyDown(KeyCode keyCode)
        {
            keyHold.Add((int)keyCode);
            frameKeyDown.Add((int)keyCode);
            frameKeyUp.Remove((int)keyCode);
        }

        protected void OnKeyUp(KeyCode keyCode)
        {
            keyHold.Remove((int)keyCode);
            frameKeyDown.Remove((int)keyCode);
            frameKeyUp.Add((int)keyCode);
        }

        public void Clear()
        {
            keyHold.Clear();
            frameKeyDown.Clear();
            frameKeyUp.Clear();
        }
        public void FrameClear()
        {
            frameKeyDown.Clear();
            frameKeyUp.Clear();
        }
    }

    public class MouseState
    {
        public static MouseState current;
        public virtual bool IsMouseHover() { return false; }
        public virtual void GetCursorPosition(out double x, out double y) { x = 0; y = 0; }
    }

    public class GLFWKeyState : KeyState
    {
        GLFW.Window window;
        GLFW.KeyCallback callback = null;
        void KeyCallback(IntPtr window, KeyCode key, int scanCode, InputState state, ModiferKeys mods)
        {
            if (this.window == window)
            {
                switch (state)
                {
                    case InputState.Release:
                        OnKeyUp(key);
                        break;
                    case InputState.Repeat:
                    case InputState.Press:
                        OnKeyDown(key);
                        break;
                }
            }
        }

        public void Init(GLFW.Window window)
        {
            this.window = window;
            this.callback = new GLFW.KeyCallback(KeyCallback);
            GLFW.Glfw.SetKeyCallback(window, callback);
            current = this;
        }
    }

    public class GLFWMouseState : MouseState
    {
        GLFW.Window window;

        public override bool IsMouseHover()
        {
            return GLFW.Glfw.GetWindowAttribute(this.window, WindowAttribute.MouseHover);
        }

        public void Init(GLFW.Window window)
        {
            this.window = window;
            current = this;
        }

        public override void GetCursorPosition(out double x, out double y)
        {
            GLFW.Glfw.GetCursorPosition(window, out x, out y);
        }
    }
}