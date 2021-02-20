using System;
using System.Collections.Generic;
using System.IO;

namespace WavefrontObjSharp
{

	public class Parser
	{
		Dictionary<string, IObjCommand> commands = new Dictionary<string, IObjCommand>();

		public delegate string GetLineFunc();
		GetLineFunc GetLine = null;
		public class Option
		{
			private Parser parser = null;
			public Option(Parser parser) { this.parser = parser; }
			public void AddCommand(string name, IObjCommand command)
			{
				parser.commands.Add(name, command);
			}

			public void SetInput(GetLineFunc func)
			{
				parser.GetLine = func;
			}
		}

		public void Configure(Action<Option> callback)
		{
			Option opt = new Option(this);
			callback?.Invoke(opt);
		}

        public ObjModel Run(string filename)
        {
            var reader = Utils.GetStreamReader(filename);
            if (reader == null)
                return null;
            return Run(reader);
        }


        public ObjModel Run(StreamReader reader = null)
		{
            if (reader != null)
                GetLine = reader.ReadLine;
			ObjModel model = new ObjModel();
			string line;
			while (null != (line = GetLine()))
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;
				if (line[0] == '#')
					continue;
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
						cmd.Execute(param, model);
					}
				}
			}
			return model;
		}

        public static Parser CreateDefault()
        {
            var parser = new Parser();
            parser.Configure(option =>
            {
                option.AddCommand("v", new ObjCommand_v("v"));
                option.AddCommand("vn", new ObjCommand_v("vn"));
                option.AddCommand("vt", new ObjCommand_v("vt"));
                option.AddCommand("f", new ObjCommand_f());
                option.AddCommand("o", new ObjCommand_o());
            });
            return parser;
        }
	}

}