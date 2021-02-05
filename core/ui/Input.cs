using GLFW;
using System;
using System.Collections.Generic;
using KeyCode = GLFW.Keys;

namespace WavefrontObjSharp
{
    public static class Input
    {
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
            this.callback = new GLFW.KeyCallback(this.KeyCallback);
            GLFW.Glfw.SetKeyCallback(window, callback);
            current = this;
        }
    }
}