using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class NetworkMainProcessor
    {
        PacketReceiveProcessor _receiveProcessor;
        public byte[] ProcessingSnapshotData => _receiveProcessor.ProcessingSnapshotData;
        public NetworkMainProcessor(NetworkProcessorConfigure configure)
        {
            _receiveProcessor = new PacketReceiveProcessor(configure);
        }
        public void JoinPlayer(int playerID)
        {
            PacketSendProcessor.JoinPlayer(playerID);
        }
        public void LeavePlayer(int playerID)
        {
            PacketSendProcessor.LeavePlayer(playerID);
        }
        public void AddReceiveHandler(BaseEventReceiver receiver)
        {
            _receiveProcessor.AddReceiveHandler(receiver);
        }
        public void OnReliableDataReceived(PlayerRef player, byte[] byteArray)
        {
            _receiveProcessor.OnReliableDataReceived(player, byteArray);
        }
        public void Update()
        {
            PacketSendProcessor.Update();
            _receiveProcessor.OnUpdatePrevFrameQueue();
        }
    }
}