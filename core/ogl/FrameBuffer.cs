using OpenGL;
using System;
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


public abstract class FrameBufferAttachmentInfo
{
    public enum TextureFormat
    {
        GL_RGB = Gl.GL_RGB,
        GL_DEPTH_COMPONENT = Gl.GL_DEPTH_COMPONENT
    }

    public enum AttachmentType
    {
        GL_COLOR_ATTACHMENT = Gl.GL_COLOR_ATTACHMENT0,
        GL_DEPTH_ATTACHMENT = Gl.GL_DEPTH_ATTACHMENT,
        GL_STENCIL_ATTACHMENT = Gl.GL_STENCIL_ATTACHMENT
    }

    public TextureFormat format;
    public AttachmentType type;
    public int index = 0;
    public uint textureID;
}

public class FrameBufferColorAttachmentInfo: FrameBufferAttachmentInfo
{
    public FrameBufferColorAttachmentInfo(int index)
    {
        this.index = index;
        this.format = TextureFormat.GL_RGB;
        this.type = AttachmentType.GL_COLOR_ATTACHMENT;
    }
}

public class FrameBufferDepthAttachmentInfo: FrameBufferAttachmentInfo
{
    public FrameBufferDepthAttachmentInfo()
    {
        this.index = 0;
        this.format = TextureFormat.GL_DEPTH_COMPONENT;
        this.type = AttachmentType.GL_DEPTH_ATTACHMENT;
    }
}

public class FrameBuffer
{
    public uint fbo;
    public Texture[] colors;
    public Texture depth;

    public void Init(int colorBufferCount, bool hasDepth, uint width, uint height)
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

    private void Init(FrameBufferAttachmentInfo[] attachmentInfos, uint width, uint height)
    {
        var lastFbo = Gl.glGetInteger(Gl.GL_FRAMEBUFFER_BINDING);
        var lastTexture2D = Gl.glGetInteger(Gl.GL_TEXTURE_BINDING_2D);
        fbo = Gl.glGenFramebuffer();
        Gl.glBindFramebuffer(fbo);       

        var textures = Gl.glGenTextures(attachmentInfos.Length);
        for (int i = 0; i < attachmentInfos.Length; i++)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]); var err1 = Gl.GetError();
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)attachmentInfos[i].format, (int)width, (int)height, 0, (int)attachmentInfos[i].format, Gl.GL_FLOAT, System.IntPtr.Zero); var err2 = Gl.GetError();

            int attachment = (int)attachmentInfos[i].type;
            if (attachmentInfos[i].type == FrameBufferAttachmentInfo.AttachmentType.GL_COLOR_ATTACHMENT)
                attachment += attachmentInfos[i].index;
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);
            var err3 = Gl.GetError();
            Gl.glFramebufferTexture2D(Gl.GL_FRAMEBUFFER, attachment, Gl.GL_TEXTURE_2D, textures[i], 0); var err4 = Gl.GetError();
            attachmentInfos[i].textureID = textures[i];

            if ((err1 | err2 | err3 | err4) != 0)
            {
                Console.WriteLine(string.Format("[Framebuffer:Init] attachmentInfos[{0}] bind:{1}, img:{2}, texparam:{3}, attach:{4}", i, err1, err2, err3, err4));
            }

        }

        var status = Gl.glCheckFramebufferStatus(Gl.GL_FRAMEBUFFER);
        if(status!= (int)FrameBufferStatus.COMPLETE)
        {
            var err = Gl.GetError();
            Console.WriteLine(string.Format(
                "[FrameBuffer] {0}, {1}({2}), err: {3}", fbo, status, (FrameBufferStatus)status, err
                ));
        }
        Gl.glBindFramebuffer((uint)lastFbo);
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, (uint)lastTexture2D);
    }

    public void Use()
    {
        Gl.glBindFramebuffer(fbo); var err1 = Gl.GetError();
        if (err1 != 0)
        {
            Console.WriteLine(string.Format("[Framebuffer:Use] {0}, {1}",fbo, err1));
        }
    }

    public static void UseDefault()
    {
        Gl.glBindFramebuffer(0); var err1 = Gl.GetError();
        if (err1 != 0)
        {
            Console.WriteLine(string.Format("[Framebuffer:UseDefault] {0}", err1));
        }
    }
}