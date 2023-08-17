
namespace Network
{
    readonly struct SendPacketData
    {
        public readonly int ReceiverID;
        public readonly byte[] Data;
        public readonly int Size;
        public SendPacketData(int receiverID, byte[] data, int size)
        {
            ReceiverID = receiverID;
            Data = data;
            Size = size;
        }
    }
}
