using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public abstract class BaseEventReceiver : ReliableHelper
    {
        public delegate bool OnEventDelegate(int playerID);
        Dictionary<short, OnEventDelegate> _eventDelegateDic = new Dictionary<short, OnEventDelegate>();
        int _startIndex;
        byte[] _byteArray;
        public ushort EventCode;
        public void MappingReceiveEvent(Enum key, OnEventDelegate function)
        {
            short parseKey = Convert.ToInt16(key);
            if (!_eventDelegateDic.ContainsKey(parseKey))
                _eventDelegateDic.Add(parseKey, function);
        }
        public bool ReceiveEvent(int playerID, byte[] eventData)
        {
            _startIndex = EventCodeSize;
            short code = BitConverter.ToInt16(eventData, sizeof(short));
            _byteArray = eventData;
            return _eventDelegateDic[code](playerID);
        }
        public short GetInt16()
        {
            short convert = BitConverter.ToInt16(_byteArray, _startIndex);
            _startIndex += ShortSize;
            return convert;
        }
        public ushort GetUInt16()
        {
            ushort convert = BitConverter.ToUInt16(_byteArray, _startIndex);
            _startIndex += UShortSize;
            return convert;
        }
        public int GetInt32()
        {
            int convert = BitConverter.ToInt32(_byteArray, _startIndex);
            _startIndex += IntSize;
            return convert;
        }
        public int[] GetInt32Array()
        {
            int length = GetInt32();
            if (length == 0)
                return null;

            int[] convert = new int[length];
            for (int i = 0; i < convert.Length; ++i)
            {
                convert[i] = BitConverter.ToInt32(_byteArray, _startIndex);
                _startIndex += IntSize;
            }
            return convert;
        }
        public uint GetUInt32()
        {
            uint convert = BitConverter.ToUInt32(_byteArray, _startIndex);
            _startIndex += UIntSize;
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
                convert[i] = BitConverter.ToUInt32(_byteArray, _startIndex);
                _startIndex += IntSize;
            }
            return convert;
        }
        public long GetInt64()
        {
            long convert = BitConverter.ToInt64(_byteArray, _startIndex);
            _startIndex += LongSize;
            return convert;
        }
        public ulong GetUInt64()
        {
            ulong convert = BitConverter.ToUInt64(_byteArray, _startIndex);
            _startIndex += ULongSize;
            return convert;
        }
        public float GetFloat()
        {
            float convert = BitConverter.ToSingle(_byteArray, _startIndex);
            _startIndex += FloatSize;
            return convert;
        }
        public bool GetBoolean()
        {
            bool convert = BitConverter.ToBoolean(_byteArray, _startIndex);
            _startIndex += BooleanSize;
            return convert;
        }
        public string GetString()
        {
            int length = GetInt32();
            System.Text.StringBuilder builder = new System.Text.StringBuilder(length);
            for (int i = 0; i < length; ++i)
            {
                builder.Append(BitConverter.ToChar(_byteArray, _startIndex));
                _startIndex += sizeof(char);
            }
            return builder.ToString();
        }
        public Vector2 GetVector2() => new Vector2(GetFloat(), GetFloat());
        public Vector3 GetVector3() => new Vector3(GetFloat(), GetFloat(), GetFloat());
        public Vector2Int GetVector2Int() => new Vector2Int(GetInt32(), GetInt32());
        public Vector3Int GetVector3Int() => new Vector3Int(GetInt32(), GetInt32(), GetInt32());
    }
}
