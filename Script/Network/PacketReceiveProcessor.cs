using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    class PacketReceiveProcessor : ReliableHelper
    {
        NetworkProcessorConfigure _configure;
        Dictionary<ushort, BaseEventReceiver> _eventReceiver = new Dictionary<ushort, BaseEventReceiver>();
        Queue<ProcessedPacket> _frameSkipQueue = new Queue<ProcessedPacket>();
        byte[] _processingSnapshotData;
        public byte[] ProcessingSnapshotData => _processingSnapshotData;
        public PacketReceiveProcessor(NetworkProcessorConfigure configure)
        {
            _configure = configure;
        }

        public void AddReceiveHandler(BaseEventReceiver receiver)
        {
            if(!_eventReceiver.ContainsKey(receiver.EventCode))
                _eventReceiver.Add(receiver.EventCode, receiver);
        }
        public void OnReliableDataReceived(PlayerRef player, byte[] byteArray)
        {
            OnUpdateCurrentFrameArray(player, byteArray);
        }
        void OnUpdateCurrentFrameArray(PlayerRef player, byte[] byteArray)
        {
            int size = 0;
            while (size < byteArray.Length - sizeof(int))
            {
                int length = BitConverter.ToInt32(byteArray, size);
                if (length != 0)
                {
                    size += IntSize;
                    _processingSnapshotData = new byte[length];
                    Array.Copy(byteArray, size, _processingSnapshotData, 0, length);
                    size += length;
                    ushort code = BitConverter.ToUInt16(_processingSnapshotData, 0);
                    if (!_eventReceiver[code].ReceiveEvent(player, _processingSnapshotData))
                        _frameSkipQueue.Enqueue(new ProcessedPacket(code, player, _processingSnapshotData));
                }
                else
                    break;
            }
        }
        public void OnUpdatePrevFrameQueue()
        {
            int queueCount = _frameSkipQueue.Count;
            if (queueCount > 0)
            {
                for (int i = 0; i < queueCount; ++i)
                {
                    ProcessedPacket val = _frameSkipQueue.Dequeue();
                    if (!_eventReceiver[val.EventCode].ReceiveEvent(val.PlayerID, val.Data))
                    {
                        if (!val.IsOverTime(_configure.FalseDataStorageTime))
                            _frameSkipQueue.Enqueue(val);
                        else
                        {
                            System.Text.StringBuilder builder = new System.Text.StringBuilder(val.Data.Length);
                            for (int j = 0; j < val.Data.Length; ++j)
                                builder.Append(val.Data[j]);

                            if(_configure.DebugMode)
                                Debug.LogError($"[Timeout] 패킷 대기 큐 삭제 Code: {val.EventCode} Data: {builder}");
                        }
                    }
                }
            }
        }
    }
}