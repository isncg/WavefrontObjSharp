using System;
using System.Collections.Generic;

namespace WavefrontObjSharp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Parser parser = new Parser();
			parser.Configure(option =>
			{
				option.AddCommand("v", new ObjCommand_v(Vertex.Component.Position));
				option.AddCommand("vn", new ObjCommand_v(Vertex.Component.Noraml));
				option.AddCommand("vf", new ObjCommand_v(Vertex.Component.UV));
				option.AddCommand("f", new ObjCommand_f());
				option.AddCommand("o", new ObjCommand_o());
				option.AddCommand("dump", new UtilCommand_dump());

				option.SetInput(Console.ReadLine);
			});

			parser.Run();
		}
	}
}
