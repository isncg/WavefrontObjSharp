using System;

namespace WavefrontObjSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ObjCommandContext context = new ObjCommandContext();
            context.RegisterCommands();

            string line;
            while (null != (line = Console.ReadLine()))
            {
                context.NextLine(line.Trim());
            }
        }
    }
}
