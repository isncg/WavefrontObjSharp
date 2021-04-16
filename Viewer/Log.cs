using System;
using System.Collections.Generic;
using System.Text;
using OpenGL;

    public class Log
    {
        public static void LogOnGlErr(string info)
        {
            var err = Gl.GetError();
            if (err != 0)
            {
                Console.WriteLine(string.Format("[OpenGL Error {0}] {1}", err, info));
            }
        }

    public static void LogOnGlErrF(string infoFormat, params object[] args)
    {
        var err = Gl.GetError();
        if (err != 0)
        {
            Console.WriteLine(string.Format("[OpenGL Error {0}] {1}", err, string.Format(infoFormat, args)));
        }
    }
}

