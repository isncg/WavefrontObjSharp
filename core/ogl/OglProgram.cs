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

        public enum UniformID: int
        {
            _tex,
            _color,
            _mvp,
        }


        private Dictionary<uint, ShaderFile> shaderFiles = new Dictionary<uint, ShaderFile>();

        public uint Program { get; private set; } = 0;
        public enum ShaderType
        {
            Unknown = 0,
            Vertex = Gl.GL_VERTEX_SHADER,
            Fragment = Gl.GL_FRAGMENT_SHADER
        }

        public OglProgram() { }
        public OglProgram(params string[] fileNames)
        {
            Init((option) => { option.Compile().ShaderFiles(fileNames); });
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
                    Log.LogOnGlErrF("[OglProgram:Compile] glAttachShader {0} {1}", kv.Value.filename, shader);
                }
                Gl.glLinkProgram(Program);
                var filenames =  string.Join(",", new List<ShaderFile>(shaderFiles.Values).ConvertAll(f=>f.filename));
                Log.LogOnGlErrF("[OglProgram:Compile] glLinkProgram {0} {1}", Program, filenames);
                foreach (var shader in shaders)
                {
                    Console.WriteLine(Gl.glGetShaderInfoLog(shader));
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

            public InitOption ShaderFile(ShaderType shaderType, string fileName)
            {
                if (oglProgram.shaderFiles.ContainsKey((uint)shaderType))
                    Console.WriteLine(string.Format("ERROR: Shader type {0} already exist '{1}'", shaderType, oglProgram.shaderFiles[(uint)shaderType].filename));
                else
                    oglProgram.shaderFiles.Add((uint)shaderType, new ShaderFile { filename = fileName, type = shaderType });
                return this;
            }

            public InitOption ShaderFiles(params string[] fileNames)
            {
                for(int i = 0; i < fileNames.Length; i++)
                {
                    var fnameSplit = fileNames[i].Split('.');
                    ShaderType shaderType = ShaderType.Unknown;
                    if (fnameSplit.Length > 0)
                    {
                        var postFix = fnameSplit[fnameSplit.Length - 1];
                        if (postFix == "vert")
                            shaderType = ShaderType.Vertex;
                        else if (postFix == "frag")
                            shaderType = ShaderType.Fragment;
                    }
                    if(shaderType == ShaderType.Unknown)
                        Console.WriteLine(string.Format("ERROR: Unknown shader type '{0}'", fileNames[i]));
                    else
                        ShaderFile(shaderType, fileNames[i]);
                }
                return this;
            }

            public InitOption Compile(bool compileNow = true)
            {
                this.IsCompileNow = compileNow;
                return this;
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
            Log.LogOnGlErrF("[OglProgram:CreateShader] source {0} {1}", type, source);
            Gl.glCompileShader(shader);
            Log.LogOnGlErrF("[OglProgram:CreateShader] compile {0} {1}", type, source);
            return shader;
        }


        public abstract class Uniform
        {
            //[StructLayout(LayoutKind.Explicit)]
            protected struct Cache
            {
                //[FieldOffset(0)]
                public FloatBuffer.Info bufferInfo;
                //[FieldOffset(0)]
                public int textureUnit;
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
                Log.LogOnGlErrF("[Uniform:Apply FloatBuffer] {0}", name);
                return this;
            }

            public Uniform Apply(Texture texture)
            {
                cache.textureUnit = texture.ActiveID;
                ApplyCache();
                Log.LogOnGlErrF("[Uniform:Apply Texture] {0}", name);
                return this;
            }
        }


        private UniformConfig uniformConfig = null;
        public bool Use(Action<UniformConfig> callback)
        {
            if (!Compile())
            {
                Gl.glUseProgram(Program);
            }
            if (Program <= 0)
                return false;
            if (null != callback)
            {
                if (null == this.uniformConfig)
                    this.uniformConfig = new UniformConfig { program = this };
                callback.Invoke(this.uniformConfig);
            }
            return true;
        }



        public class UniformConfig
        {
            public OglProgram program;

            public Dictionary<string, Uniform> uniformInfoDict = new Dictionary<string, Uniform>();
            public Dictionary<int, Uniform> commonUniformInfoDict = new Dictionary<int, Uniform>();
            //public Uniform GetUniformInfo<T>(string name) where T : Uniform, new()
            //{
            //    Uniform result = null;
            //    if (!uniformInfoDict.TryGetValue(name, out result))
            //    {
            //        int location = Gl.glGetUniformLocation(program.Program, name);
            //        if (location < 0)
            //        {
            //            //TODO: can not locate uniform error
            //            return null;
            //        }
            //        result = new T
            //        {
            //            name = name,
            //            location = location
            //        };
            //        uniformInfoDict[name] = result;
            //    }
            //    return result;
            //}

            public Uniform GetUniformInfo<T>(UniformID commonUniform) where T: Uniform, new()
            {
                Uniform result = null;
                if (!commonUniformInfoDict.TryGetValue((int)commonUniform, out result)){
                    string name = commonUniform.ToString();
                    int location = Gl.glGetUniformLocation(program.Program, name);
                    Console.WriteLine(string.Format("glGetUniformLocation prog:{0} name:{1} location:{2}", program.Program, name, location));
                    if (location < 0)
                        return null;
                    result = new T
                    {
                        name = name,
                        location = location
                    };
                    commonUniformInfoDict[(int)commonUniform] = result;
                }
                return result;
            }


            public class Uniform_Vector3f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform3fv(location, 1, cache.bufferInfo.pointer); }
            //public Uniform SetUniform(string name, Vector3f value) => GetUniformInfo<Uniform_Vector3f>(name)?.Apply(value);
            public Uniform SetUniform(UniformID uniformID, Vector3f value) => GetUniformInfo<Uniform_Vector3f>(uniformID)?.Apply(value);

            public class Uniform_Vector4f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform4fv(location, 1, cache.bufferInfo.pointer); }
            //public Uniform SetUniform(string name, Vector4f value) => GetUniformInfo<Uniform_Vector4f>(name)?.Apply(value);
            public Uniform SetUniform(UniformID uniformID, Vector4f value) => GetUniformInfo<Uniform_Vector4f>(uniformID)?.Apply(value);

            public class Uniform_Matrix3x3 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix3fv(location, 1, false, cache.bufferInfo.pointer); }
            //public Uniform SetUniform(string name, Matrix3x3 value) => GetUniformInfo<Uniform_Matrix3x3>(name)?.Apply(value);
            public Uniform SetUniform(UniformID uniformID, Matrix3x3 value) => GetUniformInfo<Uniform_Matrix3x3>(uniformID)?.Apply(value);

            public class Uniform_Matrix4x4 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix4fv(location, 1, false, cache.bufferInfo.pointer); }
            //public Uniform SetUniform(string name, Matrix4x4 value) => GetUniformInfo<Uniform_Matrix4x4>(name)?.Apply(value);
            public Uniform SetUniform(UniformID uniformID, Matrix4x4 value) => GetUniformInfo<Uniform_Matrix4x4>(uniformID)?.Apply(value);

            public class Uniform_Sampler : Uniform { public override void ApplyCache() => Gl.glUniform1i(location, cache.textureUnit); }
            //public Uniform SetUniform(string name, Texture value) => GetUniformInfo<Uniform_Sampler>(name)?.Apply(value);
            public Uniform SetUniform(UniformID uniformID, Texture value) => GetUniformInfo<Uniform_Sampler>(uniformID)?.Apply(value);

        }

    }
}
