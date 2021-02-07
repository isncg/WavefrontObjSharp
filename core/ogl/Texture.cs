using FreeImageAPI;
using OpenGL;
using System;

public class Texture
{
	public uint textureID;
    public static unsafe Texture Create(string filename)
    {
		//image format
		FREE_IMAGE_FORMAT fif = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
		//pointer to the image, once loaded
		FIBITMAP dib = FIBITMAP.Zero;
		//pointer to the image data
		IntPtr bits = IntPtr.Zero;
		//image width and height
		int width = 0;
		int height = 0;
		//OpenGL's image ID to map to
		uint gl_texID;

		//check the file signature and deduce its format
		fif = FreeImage.GetFileType(filename, 0);
		//if still unknown, try to guess the file format from the file extension
		if (fif == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			fif = FreeImage.GetFIFFromFilename(filename);
		//if still unkown, return failure
		if (fif == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
			return null;

		//check that the plugin has reading capabilities and load the file
		if (FreeImage.FIFSupportsReading(fif))
			dib = FreeImage.Load(fif, filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
		//if the image failed to load, return failure
		if (dib == FIBITMAP.Zero)
			return null;

		//retrieve the image data
		bits = FreeImage.GetBits(dib);
		//get the image width and height
		width = (int)FreeImage.GetWidth(dib);
		height = (int)FreeImage.GetHeight(dib);
		//if this somehow one of these failed (they shouldn't), return failure
		if ((bits == IntPtr.Zero) || (width == 0) || (height == 0))
			return null;

		//if this texture ID is in use, unload the current texture
		//if (m_texID.find(texID) != m_texID.end())
		//	glDeleteTextures(1, &(m_texID[texID]));

		//generate an OpenGL texture ID for this texture
		Gl.glGenTextures(1, &gl_texID);
		//store the texture ID mapping
		//m_texID[texID] = gl_texID;
		//bind to the new texture ID
		Gl.glBindTexture(Gl.GL_TEXTURE_2D, gl_texID);
		//store the texture data for OpenGL use
		int level = 0;
		int internal_format = Gl.GL_RGBA;
		int border = 0;
		int image_format = 0;
		uint bpp = FreeImage.GetBPP(dib);
		if (bpp == 32)
			image_format = Gl.GL_RGBA;
		else if (bpp == 24)
			image_format = Gl.GL_RGB;
		else
			return null;

		Gl.glTexImage2D(Gl.GL_TEXTURE_2D, level, internal_format, width, height,
			border, image_format, Gl.GL_UNSIGNED_BYTE, bits);

		//Free FreeImage's copy of the data
		FreeImage.Unload(dib);
		return new Texture { textureID = gl_texID };
	}
}