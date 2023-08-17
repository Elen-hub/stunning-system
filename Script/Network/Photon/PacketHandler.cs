public struct PacketHandler
{
    public PacketHandler(short eventCode, int playerID, byte[] data)
    {
        EventCode = eventCode;
        PlayerID = playerID;
        Data = data;
        SendPacketTime = global::System.DateTime.Now;
    }
    public short EventCode;
    public int PlayerID;
    public byte[] Data;
    public global::System.DateTime SendPacketTime;

    public bool IsOverTime(float storageTime)
    {
        int time = (System.DateTime.Now - SendPacketTime).Seconds;
        return time > storageTime;
    }
}