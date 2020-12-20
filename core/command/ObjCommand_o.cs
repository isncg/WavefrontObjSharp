using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjCommand_o : IObjCommand
	{
		public void Execute(List<string> param, ObjModel model)
		{
			string name = param.Count > 0 ? param[0] : string.Empty;
			if (string.IsNullOrEmpty(name))
				name = "mesh";
			model.SwitchMesh(name, true);
		}
	}
}