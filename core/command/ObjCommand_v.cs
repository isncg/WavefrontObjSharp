using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjCommand_v : IObjCommand
	{
		public string name { get; private set; }
		public ObjCommand_v(string name)
		{
			this.name = name;
		}

		public void Execute(List<string> param, ObjModel model)
		{
			var vec = new ParamVector { values = param.ToArray() }; //Vector.Parse(param.ToArray());

			if (!model.CurrentMesh.componentNames.Contains(name))
            {
				model.CurrentMesh.componentNames.Add(name);
			}
            if (!model.CurrentMesh.data.ContainsKey(name))
            {
				model.CurrentMesh.data[name] = new List<ParamVector>();
            }
			model.CurrentMesh.data[name].Add(vec);
		}
	}
}