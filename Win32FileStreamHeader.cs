using System.Runtime.InteropServices;

namespace AlternateDataStream
{
    [StructLayout(LayoutKind.Sequential)]
    struct Win32FileStreamHeader
    {
        public readonly int Id;
        public readonly int Attributes;
        public readonly LargeInteger Size;
        public readonly int NameSize;
    }
}
