using OpenGL;

public class FrameBufferAttachmentInfo
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
}


public class FrameBuffer
{
    public uint fbo;
    public Texture[] colors;
    public Texture depth;

    public FrameBuffer(FrameBufferAttachmentInfo[] attachmentInfos, int width, int height)
    {
        var lastFbo = Gl.glGetInteger(Gl.GL_FRAMEBUFFER_BINDING);
        var lastTexture2D = Gl.glGetInteger(Gl.GL_TEXTURE_BINDING_2D);
        fbo = Gl.glGenFramebuffer();
        Gl.glBindFramebuffer(fbo);
        var textures = Gl.glGenTextures(attachmentInfos.Length);
        for(int i = 0; i < attachmentInfos.Length; i++)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)attachmentInfos[i].format, width, height, 
                            0, 
                            (int)attachmentInfos[i].format, Gl.GL_FLOAT, System.IntPtr.Zero);
            int attachment = (int)attachmentInfos[i].type;
            if (attachmentInfos[i].type == FrameBufferAttachmentInfo.AttachmentType.GL_COLOR_ATTACHMENT)
                attachment += attachmentInfos[i].index;
            Gl.glFramebufferTexture2D(Gl.GL_FRAMEBUFFER, attachment, Gl.GL_TEXTURE_2D, textures[i], 0);
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