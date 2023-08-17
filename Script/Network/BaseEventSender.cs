using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Network
{
    public abstract class BaseEventSender : ReliableHelper
    {
        public ushort EventCode;
        public void GeneratePacketOption(Enum eventFunctionCode, int capacity, ReceiverOption receiverOption) => PacketSendProcessor.GeneratePacketOption(EventCode, eventFunctionCode, capacity, receiverOption);
        public void GeneratePacketOption(Enum eventFunctionCode, int capacity, ICollection<int> targets) => PacketSendProcessor.GeneratePacketOption(EventCode, eventFunctionCode, capacity, targets);
        public void GeneratePacketOption(Enum eventFunctionCode, int capacity, ICollection<PlayerRef> targets) => PacketSendProcessor.GeneratePacketOption(EventCode, eventFunctionCode, capacity, targets);
        public void GeneratePacketOption(Enum eventFunctionCode, int capacity, int target) => PacketSendProcessor.GeneratePacketOption(EventCode, eventFunctionCode, capacity, target);
        public static void CopyBytes(string str) => PacketSendProcessor.CopyBytes(str);
        public static void CopyBytes(bool val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(short val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(ushort val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(int val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(ICollection<int> val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(uint val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(ICollection<uint> val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(long val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(ulong val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(float val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(Vector2 val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(Vector3 val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(Vector2Int val) => PacketSendProcessor.CopyBytes(val);
        public static void CopyBytes(Vector3Int val) => PacketSendProcessor.CopyBytes(val);
    }
}
