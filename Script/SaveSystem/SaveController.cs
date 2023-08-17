using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveController
{
    byte[] _saveByteArray;
    int _currentPosition;
    public SaveController(int capacity)
    {
        _saveByteArray = new byte[capacity];
        _currentPosition = 0;
    }
    public SaveController(byte[] byteArr)
    {
        _saveByteArray = byteArr;
        _currentPosition = 0;
    }
    public void SetCapacity(int capacity)
    {
        if (_saveByteArray != null)
            if (_saveByteArray.Length != capacity)
                _saveByteArray = new byte[capacity];
    }
    public bool IsPossibleParsing => _saveByteArray.Length > _currentPosition;
    public byte[] GetByteArray => _saveByteArray;
    #region EnqueueByte
    public void CopyBytes(params byte[] byteArr)
    {
        Buffer.BlockCopy(byteArr, 0, _saveByteArray, _currentPosition, byteArr.Length);
        _currentPosition += byteArr.Length;
    }
    public void CopyBytes(string str)
    {
        CopyBytes(str.Length);
        for (int i = 0; i < str.Length; ++i)
            CopyBytes(BitConverter.GetBytes(str[i]));
    }
    public void CopyBytes(bool val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(short val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(ushort val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(int val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(ICollection<int> val)
    {
        if (val != null)
        {
            CopyBytes(BitConverter.GetBytes(val.Count));
            foreach (var element in val)
                CopyBytes(BitConverter.GetBytes(element));
        }
        else
            CopyBytes(BitConverter.GetBytes(0));
    }
    public void CopyBytes(uint val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(ICollection<uint> val)
    {
        if (val != null)
        {
            CopyBytes(BitConverter.GetBytes(val.Count));
            foreach (var element in val)
                CopyBytes(BitConverter.GetBytes(element));
        }
        else
            CopyBytes(BitConverter.GetBytes(0));
    }
    public void CopyBytes(long val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(ulong val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(float val)
    {
        CopyBytes(BitConverter.GetBytes(val));
    }
    public void CopyBytes(Vector2 val)
    {
        CopyBytes(BitConverter.GetBytes(val.x));
        CopyBytes(BitConverter.GetBytes(val.y));
    }
    public void CopyBytes(Vector3 val)
    {
        CopyBytes(BitConverter.GetBytes(val.x));
        CopyBytes(BitConverter.GetBytes(val.y));
        CopyBytes(BitConverter.GetBytes(val.z));
    }
    public void CopyBytes(Vector2Int val)
    {
        CopyBytes(BitConverter.GetBytes(val.x));
        CopyBytes(BitConverter.GetBytes(val.y));
    }
    public void CopyBytes(Vector3Int val)
    {
        CopyBytes(BitConverter.GetBytes(val.x));
        CopyBytes(BitConverter.GetBytes(val.y));
        CopyBytes(BitConverter.GetBytes(val.z));
    }
    #endregion
    #region Parse Byte
    public short GetInt16()
    {
        short convert = BitConverter.ToInt16(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(short);
        return convert;
    }
    public ushort GetUInt16()
    {
        ushort convert = BitConverter.ToUInt16(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(ushort);
        return convert;
    }
    public int GetInt32()
    {
        int convert = BitConverter.ToInt32(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(int);
        return convert;
    }
    public int[] GetInt32Array()
    {
        int length = GetInt32();
        if (length == 0)
            return null;

        int[] convert = new int[length];
        for (int i = 0; i < convert.Length; ++i)
            convert[i] = GetInt32();

        return convert;
    }
    public uint GetUInt32()
    {
        uint convert = BitConverter.ToUInt32(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(uint);
        return convert;
    }
    public uint[] GetUInt32Array()
    {
        int length = GetInt32();
        if (length == 0)
            return null;

        uint[] convert = new uint[length];
        for (int i = 0; i < convert.Length; ++i)
        {
            convert[i] = BitConverter.ToUInt32(_saveByteArray, _currentPosition);
            _currentPosition += sizeof(uint);
        }
        return convert;
    }
    public long GetInt64()
    {
        long convert = BitConverter.ToInt64(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(long);
        return convert;
    }
    public ulong GetUInt64()
    {
        ulong convert = BitConverter.ToUInt64(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(ulong);
        return convert;
    }
    public float GetFloat()
    {
        float convert = BitConverter.ToSingle(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(float);
        return convert;
    }
    public bool GetBoolean()
    {
        bool convert = BitConverter.ToBoolean(_saveByteArray, _currentPosition);
        _currentPosition += sizeof(bool);
        return convert;
    }
    public byte GetByte()
    {
        byte returnByte = _saveByteArray[_currentPosition];
        _currentPosition += 1;
        return returnByte;
    }
    public string GetString()
    {
        int length = GetInt32();
        if (length == 0)
            return string.Empty;

        System.Text.StringBuilder builder = new System.Text.StringBuilder(length);
        for (int i = 0; i < length; ++i)
        {
            builder.Append(BitConverter.ToChar(_saveByteArray, _currentPosition));
            _currentPosition += sizeof(char);
        }
        return builder.ToString();
    }
    public Vector2 GetVector2() => new Vector2(GetFloat(), GetFloat());
    public Vector3 GetVector3() => new Vector3(GetFloat(), GetFloat(), GetFloat());
    public Vector2Int GetVector2Int() => new Vector2Int(GetInt32(), GetInt32());
    public Vector3Int GetVector3Int() => new Vector3Int(GetInt32(), GetInt32(), GetInt32());
    #endregion
}
