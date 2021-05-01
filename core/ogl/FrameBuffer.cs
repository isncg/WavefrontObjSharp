using GLFW;
using OpenGL;
using System;
using System.Collections.Generic;
using WavefrontObjSharp;

public enum FrameBufferStatus
{
    COMPLETE = Gl.GL_FRAMEBUFFER_COMPLETE,
    INCOMPLETE_ATTACHMENT = Gl.GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT,
    INCOMPLETE_MISSING_ATTACHMENT = Gl.GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT,
    INCOMPLETE_DRAW_BUFFER = Gl.GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER,
    INCOMPLETE_READ_BUFFER = Gl.GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER,
    INCOMPLETE_MULTISAMPLE = Gl.GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE,
    INCOMPLETE_LAYER_TARGETS = Gl.GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS,

    UNDEFINED = Gl.GL_FRAMEBUFFER_UNDEFINED,
    UNSUPPORTED = Gl.GL_FRAMEBUFFER_UNSUPPORTED,
}


public class FrameBufferClearOption
{
    public Vector4f color = new Vector4f();
    public uint clearMask = Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT;
    public void Clear()
    {
        Gl.glClearColor(color.X, color.Y, color.Z, color.W);
        Gl.glClear(clearMask);
    }
}

public class FrameBufferAttachmentInfo
{
    //public enum Format
    //{
    //    RGB = Gl.GL_RGB,
    //    DEPTH_COMPONENT = Gl.GL_DEPTH_COMPONENT
    //}

    //public enum InternalFormat
    //{
    //    RGB = Gl.GL_RGB32F,
    //    DEPTH_COMPONENT = Gl.GL_DEPTH_COMPONENT32F
    //}
    //public enum AttachmentType
    //{
    //    GL_COLOR_ATTACHMENT = Gl.GL_COLOR_ATTACHMENT0,
    //    GL_DEPTH_ATTACHMENT = Gl.GL_DEPTH_ATTACHMENT,
    //    GL_STENCIL_ATTACHMENT = Gl.GL_STENCIL_ATTACHMENT
    //}

    public int internalFormat;
    public int format;
    public int attachment;
    public uint textureID;
}

public class FrameBufferColorAttachmentInfo : FrameBufferAttachmentInfo
{
    public FrameBufferColorAttachmentInfo(int index)
    {
        this.internalFormat = Gl.GL_RGB32F;
        this.format = Gl.GL_RGB;
        this.attachment = Gl.GL_COLOR_ATTACHMENT0 + index;
    }
}

public class FrameBufferDepthAttachmentInfo : FrameBufferAttachmentInfo
{
    public FrameBufferDepthAttachmentInfo()
    {
        this.internalFormat = Gl.GL_DEPTH_COMPONENT32F;
        this.format = Gl.GL_DEPTH_COMPONENT;
        this.attachment = Gl.GL_DEPTH_ATTACHMENT;
    }
}

public class FrameBuffer
{
    public uint fbo;
    public Texture[] colors;
    public Texture depth;
    private uint width;
    private uint height;
    public FrameBufferClearOption clearOption = new FrameBufferClearOption();
    public virtual void Init(int colorBufferCount, bool hasDepth, uint width, uint height)
    {
        int textureCount = hasDepth ? colorBufferCount + 1 : colorBufferCount;
        FrameBufferAttachmentInfo[] attachmentInfos = new FrameBufferAttachmentInfo[textureCount];
        for (int i = 0; i < colorBufferCount; i++)
            attachmentInfos[i] = new FrameBufferColorAttachmentInfo(i);
        if (hasDepth)
            attachmentInfos[textureCount - 1] = new FrameBufferDepthAttachmentInfo();

        Init(attachmentInfos, width, height);

        colors = new Texture[colorBufferCount];
        for (int i = 0; i < colorBufferCount; i++)
            colors[i] = new Texture { textureID = attachmentInfos[i].textureID, width = width, height = height };
        if (hasDepth)
            depth = new Texture { textureID = attachmentInfos[textureCount - 1].textureID, width = width, height = height };
    }

    protected void Init(FrameBufferAttachmentInfo[] attachmentInfos, uint width, uint height)
    {
        this.width = width;
        this.height = height;
        var lastFbo = Gl.glGetInteger(Gl.GL_FRAMEBUFFER_BINDING);
        var lastTexture2D = Gl.glGetInteger(Gl.GL_TEXTURE_BINDING_2D);
        fbo = Gl.glGenFramebuffer();
        Gl.glBindFramebuffer(fbo);
        var textures = Gl.glGenTextures(attachmentInfos.Length);
        List<int> drawBuffers = new List<int>();
        for (int i = 0; i < attachmentInfos.Length; i++)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]); Log.LogOnGlErrF("[Framebuffer:Init] bindTexture {0}", i);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)attachmentInfos[i].internalFormat, (int)width, (int)height, 0, (int)attachmentInfos[i].format, Gl.GL_FLOAT, System.IntPtr.Zero); Log.LogOnGlErrF("[Framebuffer:Init] texImage {0}", i);

            int attachment = (int)attachmentInfos[i].attachment;
            if (attachment >= Gl.GL_COLOR_ATTACHMENT0 && attachment <= Gl.GL_COLOR_ATTACHMENT31)
                drawBuffers.Add(attachment);

            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            Log.LogOnGlErrF("[Framebuffer:Init] texParam {0}", i);
            Gl.glFramebufferTexture2D(Gl.GL_FRAMEBUFFER, attachment, Gl.GL_TEXTURE_2D, textures[i], 0); Log.LogOnGlErrF("[Framebuffer:Init] attach {0}", i);
            attachmentInfos[i].textureID = textures[i];

        }
        Gl.glDrawBuffers(drawBuffers.ToArray());
        var status = Gl.glCheckFramebufferStatus(Gl.GL_FRAMEBUFFER);
        if (status != (int)FrameBufferStatus.COMPLETE)
        {
            var err = Gl.GetError();
            Console.WriteLine(string.Format(
                "[FrameBuffer] {0}, {1}({2}), err: {3}", fbo, status, (FrameBufferStatus)status, err
                ));
        }
        Gl.glBindFramebuffer((uint)lastFbo);
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, (uint)lastTexture2D);
    }

    public void Use(bool clear = false)
    {
        Gl.glBindFramebuffer(fbo);
        Log.LogOnGlErrF("[Framebuffer:Use] {0}", fbo);
        if (clear)
            clearOption.Clear();
        Gl.glViewport(0, 0, (int)width, (int)height);
    }

    public static FrameBufferClearOption DefaultClearOption = new FrameBufferClearOption
    {
        clearMask = Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT,
        color = new Vector4f(0.2f, 0.2f, 0.2f, 0.0f)
    };
    public static void UseDefault(Window window, bool clear = false)
    {
        Gl.glBindFramebuffer(0);
        Glfw.GetWindowSize(window, out var width, out var height);
        Gl.glViewport(0, 0, width, height);
        if (clear)
            DefaultClearOption.Clear();
        Log.LogOnGlErr("[Framebuffer:UseDefault]");
    }
}

public class GBuffer : FrameBuffer
{
    public override void Init(int colorBufferCount, bool hasDepth, uint width, uint height)
    {
        //base.Init(colorBufferCount, hasDepth, width, height);
        throw new NotImplementedException();
    }

    public enum RenderTexture
    {
        Color = 0,
        Position = 1,
        Diffuse = 2,
        Normal = 3,
        Texcoord = 4,
    }

    public virtual void Init(uint width, uint height)
    {
        FrameBufferAttachmentInfo[] infos = new FrameBufferAttachmentInfo[]
        {
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_RGB32F, format=Gl.GL_RGB, attachment=Gl.GL_COLOR_ATTACHMENT0  },
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_RGB32F, format=Gl.GL_RGB, attachment=Gl.GL_COLOR_ATTACHMENT1  },
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_RGB32F, format=Gl.GL_RGB, attachment=Gl.GL_COLOR_ATTACHMENT2  },
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_RGB32F, format=Gl.GL_RGB, attachment=Gl.GL_COLOR_ATTACHMENT3  },
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_RGB32F, format=Gl.GL_RGB, attachment=Gl.GL_COLOR_ATTACHMENT4  },
            new FrameBufferAttachmentInfo{internalFormat=Gl.GL_DEPTH_COMPONENT32F, format=Gl.GL_DEPTH_COMPONENT, attachment=Gl.GL_DEPTH_ATTACHMENT  },
        };
        Init(infos, width, height);

        colors = new Texture[infos.Length - 1];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Texture { textureID = infos[i].textureID, width = width, height = height };
        depth = new Texture { textureID = infos[infos.Length - 1].textureID, width = width, height = height };
    }

    public Texture TexColor=> colors[(int)RenderTexture.Color];
    public Texture TexPosition => colors[(int)RenderTexture.Position];
    public Texture TexDiffuse => colors[(int)RenderTexture.Diffuse];
    public Texture TexNormal => colors[(int)RenderTexture.Normal];
    public Texture TexTexcoord => colors[(int)RenderTexture.Texcoord];


    //public Texture GetRenderTexture(RenderTexture renderTexture)
    //{
    //    return this.colors[(int)renderTexture];
    //}
}