using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjCommand_usemtl : IObjCommand
	{
		public void Execute(List<string> param, ObjModel model)
		{
			if (param.Count > 0)
			{
				model.CurrentMesh.SwitchFaceList(param[0], true);
			}
		}
	}
}