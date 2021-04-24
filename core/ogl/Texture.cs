using FreeImageAPI;
using OpenGL;
using System;
using System.Collections.Generic;
using WavefrontObjSharp;

public enum TextureFormat : int
{
    GL_DEPTH_COMPONENT = Gl.GL_DEPTH_COMPONENT,
    GL_DEPTH_STENCIL = Gl.GL_DEPTH_STENCIL,
    GL_RED = Gl.GL_RED,
    GL_RG = Gl.GL_RG,
    GL_RGB = Gl.GL_RGB,
    GL_RGBA = Gl.GL_RGBA
}

/*
Sized Internal Format   Base Internal Format    Red Bits    Green Bits  Blue Bits   Alpha Bits
GL_R8                   GL_RED	                8	 	 	 	 
GL_R8_SNORM             GL_RED	                s8	 	 	 	 
GL_R16                  GL_RED	                16	 	 	 	 
GL_R16_SNORM	        GL_RED	                s16	 	 	 	 
GL_RG8                  GL_RG	                8	        8	 	 	 
GL_RG8_SNORM	        GL_RG	                s8	        s8	 	 	 
GL_RG16                 GL_RG	                16	        16	 	 	 
GL_RG16_SNORM           GL_RG	                s16	        s16	 	 	 
GL_R3_G3_B2             GL_RGB	                3	        3	        2	 	 
GL_RGB4                 GL_RGB	                4	        4	        4	 	 
GL_RGB5                 GL_RGB	                5	        5	        5	 	 
GL_RGB8                 GL_RGB	                8	        8	        8	 	 
GL_RGB8_SNORM           GL_RGB	                s8	        s8	        s8	 	 
GL_RGB10                GL_RGB	                10	        10	        10	 	 
GL_RGB12                GL_RGB	                12	        12	        12	 	 
GL_RGB16_SNORM          GL_RGB	                16	        16	        16	 	 
GL_RGBA2                GL_RGB	                2	        2	        2	        2	 
GL_RGBA4	            GL_RGB	                4	        4	        4	        4	 
GL_RGB5_A1	            GL_RGBA	                5	        5	        5	        1	 
GL_RGBA8	            GL_RGBA	                8	        8	        8	        8	 
GL_RGBA8_SNORM	        GL_RGBA	                s8	        s8	        s8	        s8	 
GL_RGB10_A2	            GL_RGBA	                10	        10	        10	        2	 
GL_RGB10_A2UI	        GL_RGBA	                ui10	    ui10	    ui10	    ui2	 
GL_RGBA12	            GL_RGBA	                12	        12	        12	        12	 
GL_RGBA16	            GL_RGBA	                16	        16	        16	        16	 
GL_SRGB8	            GL_RGB	                8	        8	        8	 	     
GL_SRGB8_ALPHA8	        GL_RGBA	                8	        8	        8	        8	 
GL_R16F                 GL_RED	                f16	 	     	 	     
GL_RG16F    	        GL_RG	                f16	        f16	 	     	 
GL_RGB16F   	        GL_RGB	                f16	        f16	        f16	 	     
GL_RGBA16F  	        GL_RGBA	                f16	        f16	        f16	        f16	 
GL_R32F                 GL_RED	                f32	         	 	     	 
GL_RG32F    	        GL_RG	                f32	        f32	 	     	 
GL_RGB32F   	        GL_RGB	                f32	        f32	        f32	 	     
GL_RGBA32F  	        GL_RGBA	                f32	        f32	        f32	        f32	 
GL_R11F_G11F_B10F	    GL_RGB	                f11	        f11	        f10	 	     
GL_RGB9_E5              GL_RGB	                9	        9	        9	 	    5
GL_R8I                  GL_RED	                i8	 	     	 	     
GL_R8UI                 GL_RED	                ui8	 	     	 	     
GL_R16I                 GL_RED	                i16	 	     	 	     
GL_R16UI                GL_RED	                ui16	     	 	     	 
GL_R32I                 GL_RED	                i32	 	     	 	     
GL_R32UI                GL_RED	                ui32	     	 	     	 
GL_RG8I                 GL_RG	                i8	        i8	 	     	 
GL_RG8UI	            GL_RG	                ui8	        ui8	 	     	 
GL_RG16I	            GL_RG	                i16	        i16	 	     	 
GL_RG16UI	            GL_RG	                ui16	    ui16	     	 	     
GL_RG32I	            GL_RG	                i32	        i32	 	     	 
GL_RG32UI	            GL_RG	                ui32	    ui32	     	 	     
GL_RGB8I	            GL_RGB	                i8	        i8	        i8	 	     
GL_RGB8UI	            GL_RGB	                ui8	        ui8	        ui8	 	     
GL_RGB16I	            GL_RGB	                i16	        i16	        i16	 	     
GL_RGB16UI	            GL_RGB	                ui16	    ui16	    ui16	     	 
GL_RGB32I	            GL_RGB	                i32	        i32	        i32	 	     
GL_RGB32UI	            GL_RGB	                ui32	    ui32	    ui32	     	 
GL_RGBA8I	            GL_RGBA	                i8	        i8	        i8	        i8	 
GL_RGBA8UI	            GL_RGBA	                ui8	        ui8	        ui8	        ui8	 
GL_RGBA16I	            GL_RGBA	                i16	        i16	        i16	        i16	 
GL_RGBA16UI	            GL_RGBA	                ui16	    ui16	    ui16        ui16	 
GL_RGBA32I	            GL_RGBA	                i32	        i32	        i32	        i32	 
GL_RGBA32UI	            GL_RGBA	                ui32	    ui32	    ui32	    ui32
*/
public enum SizedTextureFormat : int
{
    GL_R8 = Gl.GL_R8,
    GL_R8_SNORM = Gl.GL_R8_SNORM,
    GL_R16 = Gl.GL_R16,
    GL_R16_SNORM = Gl.GL_R16_SNORM,
    GL_RG8 = Gl.GL_RG8,
    GL_RG8_SNORM = Gl.GL_RG8_SNORM,
    GL_RG16 = Gl.GL_RG16,
    GL_RG16_SNORM = Gl.GL_RG16_SNORM,
    GL_R3_G3_B2 = Gl.GL_R3_G3_B2,
    GL_RGB4 = Gl.GL_RGB4,
    GL_RGB5 = Gl.GL_RGB5,
    GL_RGB8 = Gl.GL_RGB8,
    GL_RGB8_SNORM = Gl.GL_RGB8_SNORM,
    GL_RGB10 = Gl.GL_RGB10,
    GL_RGB12 = Gl.GL_RGB12,
    GL_RGB16_SNOR = Gl.GL_RGB16_SNORM,
    GL_RGBA2 = Gl.GL_RGBA2,
    GL_RGBA4 = Gl.GL_RGBA4,
    GL_RGB5_A1 = Gl.GL_RGB5_A1,
    GL_RGBA8 = Gl.GL_RGBA8,
    GL_RGBA8_SNOR = Gl.GL_RGBA8_SNORM,
    GL_RGB10_A2 = Gl.GL_RGB10_A2,
    GL_RGB10_A2UI = Gl.GL_RGB10_A2UI,
    GL_RGBA12 = Gl.GL_RGBA12,
    GL_RGBA16 = Gl.GL_RGBA16,
    GL_SRGB8 = Gl.GL_SRGB8,
    GL_SRGB8_ALPH = Gl.GL_SRGB8_ALPHA8,
    GL_R16F = Gl.GL_R16F,
    GL_RG16F = Gl.GL_RG16F,
    GL_RGB16F = Gl.GL_RGB16F,
    GL_RGBA16F = Gl.GL_RGBA16F,
    GL_R32F = Gl.GL_R32F,
    GL_RG32F = Gl.GL_RG32F,
    GL_RGB32F = Gl.GL_RGB32F,
    GL_RGBA32F = Gl.GL_RGBA32F,
    GL_R11F_G11F_ = Gl.GL_R11F_G11F_B10F,
    GL_RGB9_E5 = Gl.GL_RGB9_E5,
    GL_R8I = Gl.GL_R8I,
    GL_R8UI = Gl.GL_R8UI,
    GL_R16I = Gl.GL_R16I,
    GL_R16UI = Gl.GL_R16UI,
    GL_R32I = Gl.GL_R32I,
    GL_R32UI = Gl.GL_R32UI,
    GL_RG8I = Gl.GL_RG8I,
    GL_RG8UI = Gl.GL_RG8UI,
    GL_RG16I = Gl.GL_RG16I,
    GL_RG16UI = Gl.GL_RG16UI,
    GL_RG32I = Gl.GL_RG32I,
    GL_RG32UI = Gl.GL_RG32UI,
    GL_RGB8I = Gl.GL_RGB8I,
    GL_RGB8UI = Gl.GL_RGB8UI,
    GL_RGB16I = Gl.GL_RGB16I,
    GL_RGB16UI = Gl.GL_RGB16UI,
    GL_RGB32I = Gl.GL_RGB32I,
    GL_RGB32UI = Gl.GL_RGB32UI,
    GL_RGBA8I = Gl.GL_RGBA8I,
    GL_RGBA8UI = Gl.GL_RGBA8UI,
    GL_RGBA16I = Gl.GL_RGBA16I,
    GL_RGBA16UI = Gl.GL_RGBA16UI,
    GL_RGBA32I = Gl.GL_RGBA32I,
    GL_RGBA32UI = Gl.GL_RGBA32UI,
}

public class ActiveTextures
{
    public const int Count = Gl.GL_ACTIVE_TEXTURE - Gl.GL_TEXTURE0;
    private static ulong dirtyMask = 0;// 0:avaliable 1:in use
    private static Texture[] activeTextures = new Texture[Count];
    private static Dictionary<Texture, int> activeTextureDict = new Dictionary<Texture, int>();

    public static int Get(Texture texture)
    {
        if (activeTextureDict.TryGetValue(texture, out int id))
            return id;
        return -1;
    }

    public static Texture Get(int index)
    {
        return index >= 0 && index < Count ? activeTextures[index] : null;
    }

    public static void Clear()
    {
        dirtyMask = 0;
    }


    public static void Clear(bool hard)
    {
        Clear();
        if (hard)
        {
            activeTextureDict.Clear();
            for (int i = 0; i < Count; i++)
                activeTextures[i] = null;
        }
    }

    public static int Activate(Texture texture)
    {
        if (texture.activeID >= 0 && texture.activeID < Count && activeTextures[texture.activeID] == texture)
        {
            if (((1ul << texture.activeID) & dirtyMask) == 0ul)
            {
                dirtyMask |= (1ul << texture.activeID);
                Gl.glActiveTexture(texture.activeID + Gl.GL_TEXTURE0);
                Gl.glBindTexture(texture.Target, texture.textureID);
            }
            return texture.activeID;
        }

        if (activeTextureDict.TryGetValue(texture, out int id))
        {
            if (((1ul << id) & dirtyMask) == 0ul)
            {
                dirtyMask |= (1ul << id);
                Gl.glActiveTexture(id + Gl.GL_TEXTURE0);
                Gl.glBindTexture(texture.Target, texture.textureID);
            }
            texture.activeID = id;
            return id;
        }

        if (activeTextureDict.Count < Count)
        {
            for (int i = 0; i < Count; i++)
            {
                if (activeTextures[i] == null)
                {
                    activeTextures[i] = texture;
                    activeTextureDict[texture] = i;
                    dirtyMask |= (1ul << i);
                    Gl.glActiveTexture(i + Gl.GL_TEXTURE0);
                    Gl.glBindTexture(texture.Target, texture.textureID);
                    texture.activeID = i;
                    return i;
                }
            }
        }


        for (int i = 0; i < Count; i++)
        {
            if (((1ul << i) & dirtyMask) == 0ul)
            {
                activeTextures[i] = texture;
                activeTextureDict[texture] = i;
                dirtyMask |= (1ul << i);
                Gl.glActiveTexture(i + Gl.GL_TEXTURE0);
                Gl.glBindTexture(texture.Target, texture.textureID);
                texture.activeID = i;
                return i;
            }
        }
        texture.activeID = -1;
        return -1;
    }

    public static void Deactivate(Texture texture)
    {
        if (activeTextureDict.TryGetValue(texture, out var id))
        {
            activeTextureDict.Remove(texture);
            activeTextures[id] = null;
        }
    }
}


public class Texture
{
    public uint textureID;
    public uint width;
    public uint height;
    public int activeID = 0;
    public virtual int Target => Gl.GL_TEXTURE_2D;
 
    public static unsafe Texture Create(string filename)
    {
        filename = Utils.GetDataFilePath(filename);
        Console.WriteLine("Loading texture " + filename);
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
        dib = FreeImage.ConvertTo32Bits(dib);
        //retrieve the image data
        bits = FreeImage.GetBits(dib);
        //get the image width and height
        width = (int)FreeImage.GetWidth(dib);
        height = (int)FreeImage.GetHeight(dib);
        //if this somehow one of these failed (they shouldn't), return failure
        if ((bits == IntPtr.Zero) || (width == 0) || (height == 0))
            return null;

        int pixelCount = width * height;
        byte* p = (byte*)bits;
        byte t = 0;
        for (int i = 0; i < pixelCount; i++)
        {
            int k = i * 4;
            t = p[k + 0];
            p[k + 0] = p[k + 2];
            p[k + 2] = t;
        }

        //int bytesize = pixelCount * 4;
        //byte[] test = new byte[bytesize];
        //for(int i = 0; i < bytesize; i++)
        //{
        //    test[i] = p[i];
        //}

        //generate an OpenGL texture ID for this texture
        Gl.glGenTextures(1, &gl_texID);
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gl_texID);
        //store the texture data for OpenGL use

        //uint bpp = FreeImage.GetBPP(dib);

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, width, height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, bits);


        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
        //Free FreeImage's copy of the data
        FreeImage.Unload(dib);


        var err = Gl.GetError();
        Console.WriteLine("[Texture:Create] " + err);
        return new Texture { textureID = gl_texID };
    }
}