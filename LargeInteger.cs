using System.Runtime.InteropServices;

namespace AlternateDataStream
{
    [StructLayout(LayoutKind.Sequential)]
    struct LargeInteger
    {
        public readonly int Low;
        public readonly int High;

        public long ToInt64() => (this.High * 0x100000000) + this.Low;
    }
}
