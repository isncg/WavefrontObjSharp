using System;
using System.Collections.Generic;
using System.Text;
using WavefrontObjSharp;

namespace Viewer
{
    class ProgramManager
    {
		public class DeferredUniformSet : AbstractUniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex", "_mvp" };
			public Texture _tex { set { SetUniformValue(0, value); } }
			public Matrix4x4 _mvp { set { SetUniformValue(1, value); } }
        }
        public OglProgram<DeferredUniformSet> programDeferred;

		public class Deferred_debugUniformSet : AbstractUniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex", "step_min", "step_max" };
			public Texture _tex { set { SetUniformValue(0, value); } }
			public Vector4f step_min { set { SetUniformValue(1, value); } }
			public Vector4f step_max { set { SetUniformValue(2, value); } }
        }
        public OglProgram<Deferred_debugUniformSet> programDeferred_debug;

		public class QuadUniformSet : AbstractUniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex" };
			public Texture _tex { set { SetUniformValue(0, value); } }
        }
        public OglProgram<QuadUniformSet> programQuad;

		public class RectUniformSet : AbstractUniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex" };
			public Texture _tex { set { SetUniformValue(0, value); } }
        }
        public OglProgram<RectUniformSet> programRect;

		public class TeapotUniformSet : AbstractUniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_color", "_tex", "_mvp" };
			public Vector3f _color { set { SetUniformValue(0, value); } }
			public Texture _tex { set { SetUniformValue(1, value); } }
			public Matrix4x4 _mvp { set { SetUniformValue(2, value); } }
        }
        public OglProgram<TeapotUniformSet> programTeapot;

		
        public void Init()
        {
			programDeferred = new OglProgram<DeferredUniformSet>("./shader/deferred.frag", "./shader/deferred.vert");
			programDeferred_debug = new OglProgram<Deferred_debugUniformSet>("./shader/deferred_debug.frag", "./shader/deferred_debug.vert");
			programQuad = new OglProgram<QuadUniformSet>("./shader/quad.frag", "./shader/quad.vert");
			programRect = new OglProgram<RectUniformSet>("./shader/rect.frag", "./shader/rect.vert");
			programTeapot = new OglProgram<TeapotUniformSet>("./shader/teapot.frag", "./shader/teapot.vert");
			
        }
    }
}