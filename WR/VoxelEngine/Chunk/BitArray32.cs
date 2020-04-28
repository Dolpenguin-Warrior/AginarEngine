using System.Collections;
using System.Collections.Generic;

public struct BitArray32
{
    public uint Bits;
    public uint Data { get { return Bits; } }

    public BitArray32(uint bits)
    {
        Bits = bits;
    }

    public int Length
    {
        get
        {
            return 32;
        }
    }

    public bool this[int index]
    {
        get
        {
            uint mask = 1u << index;
            return (Bits & mask) == mask;
        }
        set
        {
            uint mask = 1u << index;
            if (value)
            {
                Bits |= mask;
            }
            else
            {
                Bits &= ~mask;
            }
        }
    }

    public void SetBit(int index)
    {
        uint mask = 1u << index;
        Bits |= mask;
    }

    public void UnsetBit(int index)
    {
        uint mask = 1u << index;
        Bits &= ~mask;
    }

    public uint GetBits(uint mask)
    {
        return Bits & mask;
    }

    public void SetBits(uint mask)
    {
        Bits |= mask;
    }

    public void UnsetBits(uint mask)
    {
        Bits &= ~mask;
    }

    public override bool Equals(object obj)
    {
        return obj is BitArray32 && Bits == ((BitArray32)obj).Bits;
    }

    public bool Equals(BitArray32 arr)
    {
        return Bits == arr.Bits;
    }

    public override int GetHashCode()
    {
        return Bits.GetHashCode();
    }

    public override string ToString()
    {
        const string header = "BitArray32{";
        const int headerLen = 11; // must be header.Length
        char[] chars = new char[headerLen + 32 + 1];
        int i = 0;
        for (; i < headerLen; ++i)
        {
            chars[i] = header[i];
        }
        for (uint num = 1u << 31; num > 0; num >>= 1, ++i)
        {
            chars[i] = (Bits & num) != 0 ? '1' : '0';
        }
        chars[i] = '}';
        return new string(chars);
    }


    
}