using System;
using System.Collections.Generic;


public class Bytes: List<byte>
{
    public unsafe void AddF(float value)
    {
        byte* p = (byte*)(&value);
        for(int i = 0; i < sizeof(float); i++)
        {
            this.Add(p[i]);
        }
    }
    public unsafe void AddI(int value)
    {
        byte* p = (byte*)(&value);
        for (int i = 0; i < sizeof(int); i++)
        {
            this.Add(p[i]);
        }
    }

    public unsafe void AddUI(uint value)
    {
        byte* p = (byte*)(&value);
        for (int i = 0; i < sizeof(uint); i++)
        {
            this.Add(p[i]);
        }
    }

    public void AddRange(IEnumerable<float> values)
    {
        foreach (var v in values)
            AddF(v);
    }

    public void AddRange(IEnumerable<int> values)
    {
        foreach (var v in values)
            AddI(v);
    }

    public void AddRange(IEnumerable<uint> values)
    {
        foreach (var v in values)
            AddUI(v);
    }
}
