using Fusion;
using System.Collections.Generic;

namespace Network
{
    public  class ReliableHelper
    {
        public static NetworkRunner Runner;
        public static int LimitedSendSize => NetworkProjectConfig.Global.Network.MtuDefault;
        public const int EventCodeSize = sizeof(short) * 2;
        public const int Vector2Size = sizeof(float) * 2;
        public const int Vector3Size = sizeof(float) * 3;
        public const int BooleanSize = sizeof(bool);
        public const int IntSize = sizeof(int);
        public const int UIntSize = sizeof(uint);
        public const int ShortSize = sizeof(short);
        public const int UShortSize = sizeof(ushort);
        public const int LongSize = sizeof(long);
        public const int ULongSize = sizeof(ulong);
        public const int FloatSize = sizeof(float);
        public const int CharSize = sizeof(char);
        public const int ByteSize = sizeof(byte);
        public const int Vector2IntSize = sizeof(int) * 2;
        public const int Vector3IntSize = sizeof(int) * 3;
        public static int GetByteSize(ICollection<int> val)
        {
            if (val != null) return IntSize + val.Count * IntSize;
            else return IntSize;
        }
        public static int GetByteSize(ICollection<uint> val)
        {
            if (val != null) return IntSize + val.Count * UIntSize;
            else return IntSize;
        }
        public static int StringSize(int length)
        {
            return IntSize + CharSize * length;
        }
    }
}
