//using System;
//using System.Collections.Generic;
//using System.Text;
//using WavefrontObjSharp;

//namespace Viewer
//{
//    class ProgramManager
//    {
//        public class TeapotUniformSet : AbstractUniformSet
//        {
//            protected override string[] GetUniformNames() => new string[] { "_mvp", "_color", "_tex" };
//            public Matrix4x4 _mvp { set { SetUniformValue(0, value); } }
//            public Vector4f _color { set { SetUniformValue(1, value); } }
//            public Texture _tex { set { SetUniformValue(2, value); } }

//        }
//        public OglProgram<TeapotUniformSet> programTeapot;


//        public class RectUniformSet : AbstractUniformSet
//        {
//            protected override string[] GetUniformNames() => new string[] { "_tex" };
//            public Texture _tex { set { SetUniformValue(0, value); } }
//        }
//        public OglProgram<RectUniformSet> programRect;

//        public class DeferredUniformSet : AbstractUniformSet
//        {
//            protected override string[] GetUniformNames() => new string[] { "_tex", "_mvp" };
//            public Texture _tex { set { SetUniformValue(0, value); } }
//            public Matrix4x4 _mvp { set { SetUniformValue(1, value); } }
//        }
//        public OglProgram<DeferredUniformSet> programDeferred;

//        public class DeferredDebugUniformSet : AbstractUniformSet
//        {
//            protected override string[] GetUniformNames() => new string[] { "_tex", "step_min", "step_max" };
//            public Texture _tex { set { SetUniformValue(0, value); } }
//            public Vector4f step_min { set { SetUniformValue(1, value); } }
//            public Vector4f step_max { set { SetUniformValue(2, value); } }
//        }
//        public OglProgram<DeferredDebugUniformSet> programDeferredDebug;


//        public void Init()
//        {
//            programTeapot = new OglProgram<TeapotUniformSet>("./shader/teapot.vert", "./shader/teapot.frag");
//            programRect = new OglProgram<RectUniformSet>("./shader/rect.vert", "./shader/rect.frag");
//            programDeferred = new OglProgram<DeferredUniformSet>("./shader/deferred.vert", "./shader/deferred.frag");
//            programDeferredDebug = new OglProgram<DeferredDebugUniformSet>("./shader/deferred_debug.vert", "./shader/deferred_debug.frag");
//        }
//    }
//}
