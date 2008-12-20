using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace Common.Win32 {
    /// <summary>
    /// SafeHandle class for holding find handles
    /// </summary>
    public sealed class SafeFindHandle : SafeHandleMinusOneIsInvalid {
        /// <summary>
        /// Constructor
        /// </summary>
        public SafeFindHandle()
            : base(true) {
        }

        /// <summary>
        /// Release the find handle
        /// </summary>
        /// <returns>true if the handle was released</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            return Kernel32.FindClose(handle);
        }
    }
}