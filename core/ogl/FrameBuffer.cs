using OpenGL;

public abstract class FrameBufferAttachmentInfo
{
    public enum TextureFormat
    {
        GL_RGB32F = Gl.GL_RGB32F,
        GL_DEPTH_COMPONENT32F = Gl.GL_DEPTH_COMPONENT32F
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
        this.format = TextureFormat.GL_RGB32F;
        this.type = AttachmentType.GL_COLOR_ATTACHMENT;
    }
}

public class FrameBufferDepthAttachmentInfo: FrameBufferAttachmentInfo
{
    public FrameBufferDepthAttachmentInfo()
    {
        this.index = 0;
        this.format = TextureFormat.GL_DEPTH_COMPONENT32F;
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
        for(int i = 0; i < attachmentInfos.Length; i++)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)attachmentInfos[i].format, (int)width, (int)height, 
                            0, 
                            (int)attachmentInfos[i].format, Gl.GL_FLOAT, System.IntPtr.Zero);
            int attachment = (int)attachmentInfos[i].type;
            if (attachmentInfos[i].type == FrameBufferAttachmentInfo.AttachmentType.GL_COLOR_ATTACHMENT)
                attachment += attachmentInfos[i].index;
            Gl.glFramebufferTexture2D(Gl.GL_FRAMEBUFFER, attachment, Gl.GL_TEXTURE_2D, textures[i], 0);
            attachmentInfos[i].textureID = textures[i];
        }
        Gl.glBindFramebuffer((uint)lastFbo);
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, (uint)lastTexture2D);
    }

    public void Use()
    {

    }

    public static void UseDefault()
    {
        Gl.glBindFramebuffer(0);
    }
}