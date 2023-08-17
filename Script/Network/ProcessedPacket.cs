namespace Network
{
    struct ProcessedPacket
    {
        public ProcessedPacket(ushort eventCode, int playerID, byte[] data)
        {
            EventCode = eventCode;
            PlayerID = playerID;
            Data = data;
            SendPacketTime = global::System.DateTime.Now;
        }
        public ushort EventCode;
        public int PlayerID;
        public byte[] Data;
        public global::System.DateTime SendPacketTime;

        public bool IsOverTime(float storageTime)
        {
            int time = (global::System.DateTime.Now - SendPacketTime).Seconds;
            return time > storageTime;
        }
    }
}
