using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace AlternateDataStream
{
    sealed class SafeHGlobalHandle : SafeHandle
    {
        public static SafeHGlobalHandle Allocate(int size) => new SafeHGlobalHandle(Marshal.AllocHGlobal(size), size);

        public static SafeHGlobalHandle Invalid() => new SafeHGlobalHandle();

        #region constructors

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private SafeHGlobalHandle(IntPtr toManage, int size)
            : base(IntPtr.Zero, true)
        {
            this.Size = size;
            base.SetHandle(toManage);
        }

        private SafeHGlobalHandle()
            : base(IntPtr.Zero, true)
        {
        }

        #endregion

        #region properties

        public override bool IsInvalid { get => base.handle == IntPtr.Zero; }

        public int Size { get; private set; }

        #endregion

        #region SafeHandle stuffs

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(base.handle);
            return true;
        }

        #endregion
    }
}
