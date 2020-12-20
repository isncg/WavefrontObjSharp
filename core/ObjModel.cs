using System.Collections.Generic;

namespace WavefrontObjSharp
{
	public class ObjModel
	{
		public Dictionary<string, Mesh> meshDict = new Dictionary<string, Mesh>();
		private Mesh _currentMesh = null;
		public Mesh CurrentMesh
		{
			get
			{
				if (_currentMesh == null)
				{
					string defaultMashName = "mesh";
					SwitchMesh(defaultMashName, true);
				}
				return _currentMesh;
			}
		}

		public bool SwitchMesh(string name, bool createIfNotExist)
		{
			Mesh mesh = null;
			if (meshDict.TryGetValue(name, out mesh))
			{
				_currentMesh = mesh;
				return true;
			}
			else if (createIfNotExist)
			{
				mesh = new Mesh();
				meshDict[name] = mesh;
				_currentMesh = mesh;
				return true;
			}
			return false;
		}

		public Mesh CreateNewMesh(string name)
		{
			int retryCount = 1;
			string retryName = name;
			while (meshDict.ContainsKey(retryName))
			{
				retryName = string.Format("{0}_{1}", name, retryCount);
				retryCount++;
			}
			name = retryName;
			return new Mesh { name = retryName };
		}
	}
}