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
                Console.WriteLine(Utils.Dump(model.CurrentMesh, "    "));
            }
        }
    }
}
