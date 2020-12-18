using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjCommand_v : IObjCommand
	{
		public Vertex.Component Component { get; private set; }
		public ObjCommand_v(Vertex.Component component)
		{
			Component = component;
		}

		public void Execute(List<string> param, ObjModel model)
		{
			var vec = Vector.Parse(param.ToArray());
			model.CurrentMesh.data[(int)Component].Add(vec);
		}
	}
}