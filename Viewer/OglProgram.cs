﻿using System;
using System.Collections.Generic;
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
            if(Program <=0)
            {
                List<uint> shaders = new List<uint>();
                Program = Gl.glCreateProgram();
                foreach(var kv in shaderFiles)
                {
                    var reader = Utils.GetStreamReader(kv.Value.filename);
                    if(reader == null)
                    {
                        Console.WriteLine("Cannot open file " + kv.Value.filename);
                        return false;
                    }
                    var glsl = reader.ReadToEnd();
                    var shader = CreateShader((int)kv.Key, glsl);
                    shaders.Add(shader);
                    Gl.glAttachShader(Program, shader);
                }
                Gl.glLinkProgram(Program);
                foreach(var shader in shaders)
                {
                    Gl.glDeleteShader(shader);
                }
                Gl.glUseProgram(Program);
                foreach(var kv in uniforms)
                {
                    var location = Gl.glGetUniformLocation(Program, kv.Key);
                    kv.Value.location = location;
                }
                return true;
            }
            return false;
        }


        public bool Use()
        {
            if(!Compile())
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

            public void AddUniform(Uniform.Type type, string name)
            {
                this.oglProgram.uniforms[name] = new Uniform { type = type};
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


        public class Uniform
        {
            public enum Type
            {
                Float3
            }
            public Type type;
            public int location = -1;
            //public string name = null;
        }

        public void SetUniform(string name, UniformValue value)
        {
            if(this.uniforms.TryGetValue(name, out var uniform))
            {
                value.Apply(uniform);
            }
        }

        public abstract class UniformValue {
            protected abstract Uniform.Type Type { get; }
            protected abstract void ApplyToUniform(Uniform uniform);

            public void Apply(Uniform uniform)
            {
                if (uniform.type == Type)
                    ApplyToUniform(uniform);
            }
        }

        public class UniformValueFloat3 : UniformValue
        {            
            public float v0, v1,v2;
         
            protected override Uniform.Type Type => Uniform.Type.Float3;

            protected override void ApplyToUniform(Uniform uniform)
            {
                Gl.glUniform3f(uniform.location, v0, v1, v2);
            }
        }

        public Dictionary<string, Uniform> uniforms = new Dictionary<string, Uniform>();
    }
}
