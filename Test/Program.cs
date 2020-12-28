using System;
using System.Collections.Generic;
using System.IO;
namespace WavefrontObjSharp.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = Directory.GetCurrentDirectory();
            while (!File.Exists(dir + "/data/test.obj"))
            {
                DirectoryInfo dirInfo = Directory.GetParent(dir);
                if (dirInfo == null)
                {
                    Console.WriteLine("Search end at dir" + dir);
                    return;
                }
                dir = dirInfo.FullName;
            }
            StreamReader reader = null;
            try
            {
                reader = File.OpenText(dir + "/data/test.obj");
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    string.Format("Cannot open file {0}, {1}", args[0], e.Message)
                );
            }
            if (reader == null)
                return;
            Parser parser = new Parser();
            parser.Configure(option =>
            {
                option.AddCommand("v", new ObjCommand_v("v"));
                option.AddCommand("vn", new ObjCommand_v("vn"));
                option.AddCommand("vt", new ObjCommand_v("vt"));
                option.AddCommand("f", new ObjCommand_f());
                option.AddCommand("o", new ObjCommand_o());

                if (reader != null)
                    option.SetInput(reader.ReadLine);
                else
                    option.SetInput(Console.ReadLine);
            });

            var model = parser.Run();

            foreach (var kv in model.meshDict)
            {
                Console.WriteLine("mesh: " + kv.Key);
                Console.WriteLine(Utils.Dump(model.CurrentMesh, "    ", null));
            }
        }
    }
}
