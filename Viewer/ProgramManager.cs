using System;
using System.Collections.Generic;
using System.Text;
using WavefrontObjSharp;

namespace Viewer
{
    class ProgramManager
    {
        public class TeapotUniformSet : AbstractUniformSet
        {
            public static string[] uniformNames = { "_mvp", "_color", "_tex" };
            protected override string GetUniformName(int id) => uniformNames[id];
            public Matrix4x4 _mvp { set { SetUniformValue(0, value); } }
            public Vector4f _color { set { SetUniformValue(1, value); } }
            public Texture _tex { set { SetUniformValue(2, value); } }
        }
        public OglProgram<TeapotUniformSet> programTeapot;


        public class RectUniformSet : AbstractUniformSet
        {
            public static string[] uniformNames = { "_tex" };
            protected override string GetUniformName(int id) => uniformNames[id];
            public Texture _tex { set { SetUniformValue(0, value); } }
        }
        public OglProgram<RectUniformSet> programRect;

        public class DeferredUniformSet : AbstractUniformSet
        {
            public static string[] uniformNames = { "_tex", "_mvp" };
            protected override string GetUniformName(int id) => uniformNames[id];
            public Texture _tex { set { SetUniformValue(0, value); } }
            public Matrix4x4 _mvp { set { SetUniformValue(1, value); } }
        }
        public OglProgram<DeferredUniformSet> programDeferred;

        public class DeferredDebugUniformSet : AbstractUniformSet
        {
            public static string[] uniformNames = { "_tex", "step_min", "step_max" };
            protected override string GetUniformName(int id) => uniformNames[id];
            public Texture _tex { set { SetUniformValue(0, value); } }
            public Vector4f step_min { set { SetUniformValue(1, value); } }
            public Vector4f step_max { set { SetUniformValue(2, value); } }
        }
        public OglProgram<DeferredDebugUniformSet> programDeferredDebug;


        public void Init()
        {
            programTeapot = new OglProgram<TeapotUniformSet>("./teapot.vert", "./teapot.frag");
            programRect = new OglProgram<RectUniformSet>("./rect.vert", "./rect.frag");
            programDeferred = new OglProgram<DeferredUniformSet>("./deferred.vert", "./deferred.frag");
            programDeferredDebug = new OglProgram<DeferredDebugUniformSet>("./deferred_debug.vert", "./deferred_debug.frag");
        }
    }
}
