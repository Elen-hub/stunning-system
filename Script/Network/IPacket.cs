namespace Network
{
    public interface IPacket
    {
        int GetByteSize { get; }
        void EnqueueByte();
    }
}