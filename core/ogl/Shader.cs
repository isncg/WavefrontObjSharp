using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using OpenGL;
using WavefrontObjSharp;

namespace Viewer
{
    public enum ShaderType
    {
        Unknown = 0,
        Vertex = Gl.GL_VERTEX_SHADER,
        Fragment = Gl.GL_FRAGMENT_SHADER
    }

    public class ShaderFile
    {
        public string filename;
        public ShaderType type;
    }


    public class ShaderBase
    {
        public Dictionary<uint, ShaderFile> shaderFiles = new Dictionary<uint, ShaderFile>();

        public uint Program { get; protected set; } = 0;


        private static uint CreateShader(int type, string source)
        {
            var shader = Gl.glCreateShader(type);
            Gl.glShaderSource(shader, source);
            Log.LogOnGlErrF("[Shader:CreateShader] source {0} {1}", type, source);
            Gl.glCompileShader(shader);
            Log.LogOnGlErrF("[Shader:CreateShader] compile {0} {1}", type, source);
            return shader;
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
                    Console.WriteLine(string.Format("[Shader:Compile] glAttachShader {0} {1}", kv.Value.filename, shader));
                    Log.LogOnGlErrF("[Shader:Compile] glAttachShader {0} {1}", kv.Value.filename, shader);
                }
                Gl.glLinkProgram(Program);
                var filenames = string.Join(",", new List<ShaderFile>(shaderFiles.Values).ConvertAll(f => f.filename));
                Console.WriteLine(string.Format("[Shader:Compile] glLinkProgram {0} {1}", Program, filenames));
                Log.LogOnGlErrF("[Shader:Compile] glLinkProgram {0} {1}", Program, filenames);
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
    }

    public class ShaderInitOption
    {
        private ShaderBase oglProgram;
        public bool IsCompileNow { get; private set; } = false;

        public ShaderInitOption(ShaderBase oglProgram)
        {
            this.oglProgram = oglProgram;
        }

        public ShaderInitOption ShaderFile(ShaderType shaderType, string fileName)
        {
            if (oglProgram.shaderFiles.ContainsKey((uint)shaderType))
                Console.WriteLine(string.Format("ERROR: Shader type {0} already exist '{1}'", shaderType, oglProgram.shaderFiles[(uint)shaderType].filename));
            else
                oglProgram.shaderFiles.Add((uint)shaderType, new ShaderFile { filename = fileName, type = shaderType });
            return this;
        }

        public ShaderInitOption ShaderFiles(params string[] fileNames)
        {
            for (int i = 0; i < fileNames.Length; i++)
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
                if (shaderType == ShaderType.Unknown)
                    Console.WriteLine(string.Format("ERROR: Unknown shader type '{0}'", fileNames[i]));
                else
                    ShaderFile(shaderType, fileNames[i]);
            }
            return this;
        }

        public ShaderInitOption Compile(bool compileNow = true)
        {
            this.IsCompileNow = compileNow;
            return this;
        }
    }

    public class Shader<T> : ShaderBase where T: UniformSet, new()
    {
        public Shader() { }
        public Shader(params string[] fileNames)
        {
            Init((option) => { option.Compile().ShaderFiles(fileNames); });
        }

        public Shader<T> Init(Action<ShaderInitOption> callback)
        {
            if (callback != null)
            {
                var option = new ShaderInitOption(this);
                callback(option);
                if (option.IsCompileNow)
                    Compile();
            }
            return this;
        }

        private T uniformConfig = null;
        public bool Use(Action<T> callback)
        {
            if (!Compile())// Compile returns false if already compiled
            {
                Gl.glUseProgram(Program);
            }
            if (Program <= 0)
                return false;
            if (null != callback)
            {
                if (null == this.uniformConfig)
                    this.uniformConfig = new T { program = this };
                ActiveTextures.Clear();
                callback.Invoke(this.uniformConfig);
            }
            return true;
        }
    }

    public abstract class Uniform
    {
        unsafe protected struct Cache
        {
            public float* fp1;
            public int i1;
        }
        public string name = null;
        public int location = -1;
        protected Cache cache = new Cache();
        private bool isCacheInitialized = false;
        public abstract void ApplyCache();

        private static readonly FloatBuffer.Info fbInfo = new FloatBuffer.Info();
        public unsafe Uniform Apply(FloatBuffer data)
        {
            isCacheInitialized = true;
            data.GetBufferInfo(fbInfo);
            cache.fp1 = fbInfo.pointer;
            cache.i1 = fbInfo.length;
            ApplyCache();
            Log.LogOnGlErrF("[Uniform:Apply FloatBuffer] {0}", name);
            return this;
        }

        public Uniform Apply(Texture texture)
        {
            //cache.textureUnit = texture.ActiveID;
            cache.i1 = ActiveTextures.Activate(texture); //texture.ActiveID;
            ApplyCache();
            Log.LogOnGlErrF("[Uniform:Apply Texture] {0}", name);
            return this;
        }
    }

    public abstract class UniformSet
    {
        public ShaderBase program;
        public Dictionary<int, Uniform> commonUniformInfoDict = new Dictionary<int, Uniform>();
        private string[] uniformNames = null;
        protected abstract string[] GetUniformNames();
        protected string GetUniformName(int id) => (null == uniformNames) ? ((uniformNames = GetUniformNames())[id]) : uniformNames[id];
        protected Uniform GetUniform<T>(int id) where T : Uniform, new()
        {
            Uniform result = null;
            if (!commonUniformInfoDict.TryGetValue(id, out result))
            {
                string name = GetUniformName(id);//commonUniform.ToString();
                int location = Gl.glGetUniformLocation(program.Program, name);
                Console.WriteLine(string.Format("glGetUniformLocation prog:{0} name:{1} location:{2}", program.Program, name, location));
                if (location < 0)
                    return null;
                result = new T
                {
                    name = name,
                    location = location
                };
                commonUniformInfoDict[id] = result;
            }
            return result;
        }

        protected class Uniform_Vector3f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform3fv(location, 1, cache.fp1); }
        protected class Uniform_Vector4f : Uniform { public override unsafe void ApplyCache() => Gl.glUniform4fv(location, 1, cache.fp1); }
        protected class Uniform_Matrix3x3 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix3fv(location, 1, false, cache.fp1); }
        protected class Uniform_Matrix4x4 : Uniform { public override unsafe void ApplyCache() => Gl.glUniformMatrix4fv(location, 1, false, cache.fp1); }
        protected class Uniform_Texture : Uniform { public override void ApplyCache() => Gl.glUniform1i(location, cache.i1); }

        protected Uniform SetUniformValue(int uniformID, Vector3f value) => GetUniform<Uniform_Vector3f>(uniformID)?.Apply(value);
        protected Uniform SetUniformValue(int uniformID, Vector4f value) => GetUniform<Uniform_Vector4f>(uniformID)?.Apply(value);
        protected Uniform SetUniformValue(int uniformID, Matrix3x3 value) => GetUniform<Uniform_Matrix3x3>(uniformID)?.Apply(value);
        protected Uniform SetUniformValue(int uniformID, Matrix4x4 value) => GetUniform<Uniform_Matrix4x4>(uniformID)?.Apply(value);
        protected Uniform SetUniformValue(int uniformID, Texture value) => GetUniform<Uniform_Texture>(uniformID)?.Apply(value);

    }



}
