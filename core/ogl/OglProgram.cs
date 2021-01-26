using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using OpenGL;
using WavefrontObjSharp;

namespace Viewer
{
    class OglProgram
    {
        public enum CommonVertexAttribute
        {
            pos3f = 1,
            norm3f = 2,
            uv02f = 4,
            uv12f = 8,
            color4f = 16
        }


        private Dictionary<uint, ShaderFile> shaderFiles = new Dictionary<uint, ShaderFile>();

        public uint Program { get; private set; } = 0;
        public enum ShaderType
        {
            Vertex = Gl.GL_VERTEX_SHADER,
            Fragment = Gl.GL_FRAGMENT_SHADER
        }

        public OglProgram Init(Action<InitOption> callback)
        {
            if (callback != null)
            {
                var option = new InitOption(this);
                callback(option);
                if (option.IsCompileNow)
                    Compile();
            }
            return this;
        }

        public bool Compile()
        {
            if (Program <= 0)
            {
                List<uint> shaders = new List<uint>();
                Program = Gl.glCreateProgram();
                foreach (var kv in shaderFiles)
                {
                    var reader = Utils.GetStreamReader(kv.Value.filename);
                    if (reader == null)
                    {
                        Console.WriteLine("Cannot open file " + kv.Value.filename);
                        return false;
                    }
                    var glsl = reader.ReadToEnd();
                    var shader = CreateShader((int)kv.Key, glsl);
                    shaders.Add(shader);
                    Gl.glAttachShader(Program, shader);
                    Console.WriteLine(string.Format("create shader {0} = {1}, err = {2}", kv.Value.filename, shader, Gl.GetError()));
                }
                Gl.glLinkProgram(Program);
                var filenames =  string.Join(",", new List<ShaderFile>(shaderFiles.Values).ConvertAll(f=>f.filename));
                Console.WriteLine(string.Format("link shader {0} [{1}] err={2}", Program, filenames, Gl.GetError()));
                foreach (var shader in shaders)
                {
                    Gl.glDeleteShader(shader);
                }
                Gl.glUseProgram(Program);
                return true;
            }
            return false;
        }


        public bool Use()
        {
            if (!Compile())
            {
                Gl.glUseProgram(Program);
            }
            return Program > 0;
        }

        public class InitOption
        {
            private OglProgram oglProgram;
            public bool IsCompileNow { get; private set; } = false;

            public InitOption(OglProgram oglProgram)
            {
                this.oglProgram = oglProgram;
            }

            public void AddShader(ShaderType shaderType, string fileName)
            {
                oglProgram.shaderFiles.Add((uint)shaderType, new ShaderFile { filename = fileName, type = shaderType });
            }

            public void CompileNow(bool compileNow = true)
            {
                this.IsCompileNow = compileNow;
            }
        }


        private class ShaderFile
        {
            public string filename;
            public ShaderType type;
        }

        private static uint CreateShader(int type, string source)
        {
            var shader = Gl.glCreateShader(type);
            Gl.glShaderSource(shader, source);
            Gl.glCompileShader(shader);
            return shader;
        }


        public abstract class Uniform
        {
            [StructLayout(LayoutKind.Explicit)]
            protected struct Cache
            {
                [FieldOffset(0)]
                public FloatBuffer.Info bufferInfo;
            }
            public string name = null;
            public int location = -1;
            protected Cache cache = new Cache();
            private bool isCacheInitialized = false;
            public abstract void ApplyCache();
            public Uniform Apply(FloatBuffer data)
            {
                if (cache.bufferInfo == null || !isCacheInitialized)
                {
                    cache.bufferInfo = new FloatBuffer.Info();
                    isCacheInitialized = true;
                }
                data.GetBufferInfo(cache.bufferInfo);
                ApplyCache();
                return this;
            }
        }

        public Dictionary<string, Uniform> uniformInfoDict = new Dictionary<string, Uniform>();

        public Uniform GetUniformInfo<T>(string name) where T : Uniform, new()
        {
            Uniform result = null;
            if (!uniformInfoDict.TryGetValue(name, out result))
            {
                int location = Gl.glGetUniformLocation(this.Program, name);
                if (location < 0)
                {
                    //TODO: can not locate uniform error
                    return null;
                }
                result = new T
                {
                    name = name,
                    location = location
                };
                uniformInfoDict[name] = result;
            }
            return result;
        }


        public class Uniform_Vector3f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform3fv(location, 1, cache.bufferInfo.pointer); }
        public Uniform SetUniform(string name, Vector3f value) => GetUniformInfo<Uniform_Vector3f>(name)?.Apply(value);

        public class Uniform_Vector4f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform4fv(location, 1, cache.bufferInfo.pointer); }
        public Uniform SetUniform(string name, Vector4f value) => GetUniformInfo<Uniform_Vector4f>(name)?.Apply(value);

        public class Uniform_Matrix3x3 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix3fv(location, 1, false, cache.bufferInfo.pointer); }
        public Uniform SetUniform(string name, Matrix3x3 value) => GetUniformInfo<Uniform_Matrix3x3>(name)?.Apply(value);

        public class Uniform_Matrix4x4 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix4fv(location, 1, false, cache.bufferInfo.pointer); }
        public Uniform SetUniform(string name, Matrix4x4 value) => GetUniformInfo<Uniform_Matrix4x4>(name)?.Apply(value);
    }
}
