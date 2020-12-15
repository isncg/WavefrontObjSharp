using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp
{
	public interface IObjCommand
	{
		void Execute(List<string> param, ObjCommandContext context);
	}


	public class ObjCommandContext
	{
		public Mesh currentMesh = new Mesh();
		public Dictionary<string, Mesh> meshDict = new Dictionary<string, Mesh>();

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

		Dictionary<string, IObjCommand> commands = new Dictionary<string, IObjCommand>();

		public void RegisterCommands()
		{
			commands.Add("v", new ObjCommand_v(Vertex.Component.Position));
			commands.Add("vn", new ObjCommand_v(Vertex.Component.Noraml));
			commands.Add("vt", new ObjCommand_v(Vertex.Component.UV));
			commands.Add("f", new ObjCommand_f());
			commands.Add("o", new ObjCommand_o());
			commands.Add("dump", new UtilCommand_dump());
		}

		public void NextLine(string line)
		{
			if (string.IsNullOrWhiteSpace(line))
				return;
			if(line[0] == '#')
				return;
			var split = line.Split(" ");
			List<string> tokens = new List<string>();
			foreach (var str in split)
			{
				if (!string.IsNullOrWhiteSpace(str))
					tokens.Add(str);
			}

			if (tokens.Count > 0)
			{
				string name = tokens[0];
				List<string> param = tokens.GetRange(1, tokens.Count - 1);
				IObjCommand cmd = null;
				if (commands.TryGetValue(name, out cmd))
				{
					cmd.Execute(param, this);
				}
			}
		}
	}
}

