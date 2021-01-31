class OglVertexAttribute
{
    public uint index;
    public int size;
    public int type;
    public bool normalized;
    public int stride;
    public int offset;
}

public enum OglVertexAttributeType
{
    Float3,
    Float4
}