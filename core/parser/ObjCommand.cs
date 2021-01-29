using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public interface IObjCommand
	{
		void Execute(List<string> param, ObjModel model);
	}
}

