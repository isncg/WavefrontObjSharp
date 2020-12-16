using System;
using System.Collections.Generic;
using System.IO;

namespace WavefrontObjSharp
{
	class Program
	{
		static void Main(string[] args)
		{
			//Console.WriteLine(string.Join(", ", args));
			StreamReader reader = null;
			if (args.Length > 0)
			{
				try
				{
					reader = File.OpenText(args[0]);
				}
				catch (Exception e)
				{
					Console.WriteLine(
						string.Format("Cannot open file {0}, {1}", args[0], e.Message)
					);
				}
				if (reader == null)
					return;
			}
			Parser parser = new Parser();
			parser.Configure(option =>
			{
				option.AddCommand("v", new ObjCommand_v(Vertex.Component.Position));
				option.AddCommand("vn", new ObjCommand_v(Vertex.Component.Noraml));
				option.AddCommand("vt", new ObjCommand_v(Vertex.Component.UV));
				option.AddCommand("f", new ObjCommand_f());
				option.AddCommand("o", new ObjCommand_o());
				option.AddCommand("dump", new UtilCommand_dump());

				if (reader != null)
					option.SetInput(reader.ReadLine);
				else
					option.SetInput(Console.ReadLine);
			});

			parser.Run();
		}
	}
}
