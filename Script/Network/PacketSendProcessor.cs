using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Network
{
    public static class PacketSendProcessor
    {
        static Dictionary<int, byte[]> _currentPacketDic = new Dictionary<int, byte[]>() { { -1, null }, };
        static Dictionary<int, int> _packetCountDic = new Dictionary<int, int>() { { -1, 0 }, };
        static HashSet<int> _isUsingBytePlayerHash = new HashSet<int>();
        static Queue<byte[]> _bytePool = new Queue<byte[]>();
        static Queue<SendPacketData> _packetDataQueue = new Queue<SendPacketData>();
        static Queue<int> _targetQueue = new Queue<int>();

        public static void JoinPlayer(int playerID)
        {
            if (!_currentPacketDic.ContainsKey(playerID))
                _currentPacketDic.Add(playerID, null);
            if (!_packetCountDic.ContainsKey(playerID))
                _packetCountDic.Add(playerID, 0);
        }
        public static void LeavePlayer(int playerID)
        {
            if (_currentPacketDic.ContainsKey(playerID))
            {
                if (_currentPacketDic[playerID] != null)
                {
                    // EnqueueBytePool(_currentPacketDic[playerID], _currentPacketDic[playerID].Length);
                    _bytePool.Enqueue(_currentPacketDic[playerID]);
                }
                _currentPacketDic.Remove(playerID);
            }
            if (_packetCountDic.ContainsKey(playerID))
                _packetCountDic.Remove(playerID);
        }
        static void EnqueueBytePool(byte[] data, int size)
        {
            Array.Clear(data, 0, size);
            _bytePool.Enqueue(data);
        }
        static byte[] GetByteArray()
        {
            return _bytePool.Count > 0 ? _bytePool.Dequeue() : new byte[ReliableHelper.LimitedSendSize];
        }
        public static void GeneratePacketOption(ushort eventCode, Enum eventFunctionCode, int capacity, ReceiverOption receiverOption)
        {
            _targetQueue.Clear();
            IEnumerator<PlayerRef> enumerator = ReliableHelper.Runner.ActivePlayers.GetEnumerator();
            switch (receiverOption)
            {
                case ReceiverOption.All:
                    while (enumerator.MoveNext())
                        _targetQueue.Enqueue(enumerator.Current);
                    break;
                case ReceiverOption.Other:
                    while (enumerator.MoveNext())
                        if (ReliableHelper.Runner.LocalPlayer.PlayerId != enumerator.Current.PlayerId)
                            _targetQueue.Enqueue(enumerator.Current);
                    break;
                case ReceiverOption.Host:
                    _targetQueue.Enqueue(-1);
                    break;
            }
            OnGeneratePacketOption(eventCode, eventFunctionCode, capacity);
        }
        public static void GeneratePacketOption(ushort eventCode, Enum eventFunctionCode, int capacity, ICollection<int> targets)
        {
            _targetQueue.Clear();
            foreach (var player in targets)
                _targetQueue.Enqueue(player);

            OnGeneratePacketOption(eventCode, eventFunctionCode, capacity);
        }
        public static void GeneratePacketOption(ushort eventCode, Enum eventFunctionCode, int capacity, ICollection<PlayerRef> targets)
        {
            _targetQueue.Clear();
            foreach (var player in targets)
                _targetQueue.Enqueue(player);

            OnGeneratePacketOption(eventCode, eventFunctionCode, capacity);
        }
        public static void GeneratePacketOption(ushort eventCode, Enum eventFunctionCode, int capacity, int target)
        {
            _targetQueue.Clear();
            _targetQueue.Enqueue(target);
            OnGeneratePacketOption(eventCode, eventFunctionCode, capacity);
        }
        static void OnGeneratePacketOption(ushort eventColumnCode, Enum eventFunctionCode, int capacity)
        {
            DebugUtility.Log($"[PushSendQueue] EventTypeCode: {eventColumnCode} EventFunctionCode: {eventFunctionCode} Capacity: {capacity}");
            foreach (var playerNum in _targetQueue)
            {
                if (!_isUsingBytePlayerHash.Contains(playerNum))
                    _isUsingBytePlayerHash.Add(playerNum);

                if (_currentPacketDic[playerNum] != null)
                {
                    if (_packetCountDic[playerNum] + ReliableHelper.IntSize + ReliableHelper.EventCodeSize + capacity > ReliableHelper.LimitedSendSize)
                    {
                        _packetDataQueue.Enqueue(new SendPacketData(playerNum, _currentPacketDic[playerNum], _packetCountDic[playerNum]));
                        _packetCountDic[playerNum] = 0;
                        _currentPacketDic[playerNum] = GetByteArray();
                    }
                }
                else
                {
                    _currentPacketDic[playerNum] = GetByteArray();
                    _packetCountDic[playerNum] = 0;
                }
            }
            CopyBytes(capacity +ReliableHelper.EventCodeSize);
            CopyBytes(eventColumnCode);
            CopyBytes(Convert.ToInt16(eventFunctionCode));
        }
        static void ClearPacketData()
        {
            foreach (var playerNum in _isUsingBytePlayerHash)
            {
                byte[] byteArr = _currentPacketDic[playerNum];
                if (byteArr != null)
                {
                    int size = _packetCountDic[playerNum];
                    SendPacketData data = new SendPacketData(playerNum, byteArr, size);
                    _packetDataQueue.Enqueue(data);
                    _currentPacketDic[playerNum] = null;
                    _packetCountDic[playerNum] = 0;
                }
            }
            _isUsingBytePlayerHash.Clear();
        }
        public static void Update()
        {
            ClearPacketData();
            while (_packetDataQueue.Count > 0)
            {
                SendPacketData data = _packetDataQueue.Dequeue();
                System.Text.StringBuilder builder = new System.Text.StringBuilder(data.Data.Length);
                for (int j = 0; j < data.Size; ++j)
                    builder.Append(data.Data[j]);

                DebugUtility.Log($"[SendPacketData] Size: {data.Size} Data: {builder}");

                byte[] sendData = new byte[data.Size];
                Buffer.BlockCopy(data.Data, 0, sendData, 0, data.Size);
                if (data.ReceiverID != -1)  ReliableHelper.Runner.SendReliableDataToPlayer(data.ReceiverID, sendData);
                else ReliableHelper.Runner.SendReliableDataToServer(sendData);

                _bytePool.Enqueue(data.Data);
                // EnqueueBytePool(data.Data, data.Size);
            }
        }
        static void CopyBytes(byte[] byteArr)
        {
            foreach (var targetID in _targetQueue)
            {
                Buffer.BlockCopy(byteArr, 0, _currentPacketDic[targetID], _packetCountDic[targetID], byteArr.Length);
                _packetCountDic[targetID] += byteArr.Length;
            }
        }
        public static void CopyBytes(string str)
        {
            CopyBytes(str.Length);
            for (int i = 0; i < str.Length; ++i)
                CopyBytes(BitConverter.GetBytes(str[i]));
        }
        public static void CopyBytes(ICollection<int> val)
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
        public static void CopyBytes(bool val) => CopyBytes(BitConverter.GetBytes(val));
        public static void CopyBytes(short val) => CopyBytes(BitConverter.GetBytes(val));
        public static void CopyBytes(ushort val) => CopyBytes(BitConverter.GetBytes(val));
        public static void CopyBytes(int val) => CopyBytes(BitConverter.GetBytes(val));
        public static void CopyBytes(uint val)
        {
            CopyBytes(BitConverter.GetBytes(val));
        }
        public static void CopyBytes(ICollection<uint> val)
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
        public static void CopyBytes(long val)
        {
            CopyBytes(BitConverter.GetBytes(val));
        }
        public static void CopyBytes(ulong val)
        {
            CopyBytes(BitConverter.GetBytes(val));
        }
        public static void CopyBytes(float val)
        {
            CopyBytes(BitConverter.GetBytes(val));
        }
        public static void CopyBytes(Vector2 val)
        {
            CopyBytes(BitConverter.GetBytes(val.x));
            CopyBytes(BitConverter.GetBytes(val.y));
        }
        public static void CopyBytes(Vector3 val)
        {
            CopyBytes(BitConverter.GetBytes(val.x));
            CopyBytes(BitConverter.GetBytes(val.y));
            CopyBytes(BitConverter.GetBytes(val.z));
        }
        public static void CopyBytes(Vector2Int val)
        {
            CopyBytes(BitConverter.GetBytes(val.x));
            CopyBytes(BitConverter.GetBytes(val.y));
        }
        public static void CopyBytes(Vector3Int val)
        {
            CopyBytes(BitConverter.GetBytes(val.x));
            CopyBytes(BitConverter.GetBytes(val.y));
            CopyBytes(BitConverter.GetBytes(val.z));
        }
    }
}