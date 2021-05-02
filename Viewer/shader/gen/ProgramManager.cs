using System;
using System.Collections.Generic;
using System.Text;
using WavefrontObjSharp;

namespace Viewer
{
    class ShaderManager
    {
		public class DeferredUniformSet : UniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex", "_mvp" };
			public Texture _tex { set { SetUniformValue(0, value); } }
			public Matrix4x4 _mvp { set { SetUniformValue(1, value); } }
        }
        public Shader<DeferredUniformSet> deferred;

		public class Deferred_debugUniformSet : UniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex", "step_min", "step_max" };
			public Texture _tex { set { SetUniformValue(0, value); } }
			public Vector4f step_min { set { SetUniformValue(1, value); } }
			public Vector4f step_max { set { SetUniformValue(2, value); } }
        }
        public Shader<Deferred_debugUniformSet> deferred_debug;

		public class QuadUniformSet : UniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex" };
			public Texture _tex { set { SetUniformValue(0, value); } }
        }
        public Shader<QuadUniformSet> quad;

		public class RectUniformSet : UniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_tex" };
			public Texture _tex { set { SetUniformValue(0, value); } }
        }
        public Shader<RectUniformSet> rect;

		public class TeapotUniformSet : UniformSet
        {
            protected override string[] GetUniformNames() => new string[] { "_color", "_tex", "_mvp" };
			public Vector3f _color { set { SetUniformValue(0, value); } }
			public Texture _tex { set { SetUniformValue(1, value); } }
			public Matrix4x4 _mvp { set { SetUniformValue(2, value); } }
        }
        public Shader<TeapotUniformSet> teapot;

		
        public void Init()
        {
			deferred = new Shader<DeferredUniformSet>("./shader/deferred.frag", "./shader/deferred.vert");
			deferred_debug = new Shader<Deferred_debugUniformSet>("./shader/deferred_debug.frag", "./shader/deferred_debug.vert");
			quad = new Shader<QuadUniformSet>("./shader/quad.frag", "./shader/quad.vert");
			rect = new Shader<RectUniformSet>("./shader/rect.frag", "./shader/rect.vert");
			teapot = new Shader<TeapotUniformSet>("./shader/teapot.frag", "./shader/teapot.vert");
			
        }
    }
}