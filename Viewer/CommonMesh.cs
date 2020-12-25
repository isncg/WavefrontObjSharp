using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Viewer
{
    [Serializable]
    public class CommonVertex
    {
        public float[] position { get; set; } = new float[3];
        public float[] normal { get; set; } = new float[3];
        public float[] uv0 { get; set; } = new float[2];
        public float[] uv1 { get; set; } = new float[2];
        public float[] color { get; set; } = new float[4];
    }

 
    [Serializable]
    public class CommonMesh
    {
        public List<CommonVertex> VertexArray { get; set; } = new List<CommonVertex>();
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static CommonMesh Deserialize(string json)
        {
            try
            {
                var result = JsonSerializer.Deserialize<CommonMesh>(json);
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
