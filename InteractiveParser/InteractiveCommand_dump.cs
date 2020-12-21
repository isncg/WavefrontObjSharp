using System;
using System.Collections.Generic;
using System.Text;

namespace WavefrontObjSharp.InteractiveParser
{
    class InteractiveCommand_dump: IObjCommand
    {
        public void Execute(List<string> param, ObjModel model)
        {
            foreach (var kv in model.meshDict)
            {
                Console.WriteLine("mesh: " + kv.Key);
                string[] names = param.Count > 0 ? param.ToArray() : null;
                Console.WriteLine(Utils.Dump(model.CurrentMesh, "    ", names));
            }
        }
    }
}
