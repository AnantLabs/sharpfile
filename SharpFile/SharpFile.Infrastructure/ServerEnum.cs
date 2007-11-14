using System;
using System.Collections;
using System.Runtime.InteropServices;

//namespace ServerEnumerator {
namespace SharpFile.Infrastructure {
	/// <summary>
	/// The ServerEnum class is used to enumerate servers on the
	/// network.
	/// </summary>
	///
	public enum ResourceScope {
		RESOURCE_CONNECTED = 1,
		RESOURCE_GLOBALNET,
		RESOURCE_REMEMBERED,
		RESOURCE_RECENT,
		RESOURCE_CONTEXT
	};

	public enum ResourceType {
		RESOURCETYPE_ANY,
		RESOURCETYPE_DISK,
		RESOURCETYPE_PRINT,
		RESOURCETYPE_RESERVED
	};

	public enum ResourceUsage {
		RESOURCEUSAGE_CONNECTABLE = 0x00000001,
		RESOURCEUSAGE_CONTAINER = 0x00000002,
		RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
		RESOURCEUSAGE_SIBLING = 0x00000008,
		RESOURCEUSAGE_ATTACHED = 0x00000010,
		RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
	};

	public enum ResourceDisplayType {
		RESOURCEDISPLAYTYPE_GENERIC,
		RESOURCEDISPLAYTYPE_DOMAIN,
		RESOURCEDISPLAYTYPE_SERVER,
		RESOURCEDISPLAYTYPE_SHARE,
		RESOURCEDISPLAYTYPE_FILE,
		RESOURCEDISPLAYTYPE_GROUP,
		RESOURCEDISPLAYTYPE_NETWORK,
		RESOURCEDISPLAYTYPE_ROOT,
		RESOURCEDISPLAYTYPE_SHAREADMIN,
		RESOURCEDISPLAYTYPE_DIRECTORY,
		RESOURCEDISPLAYTYPE_TREE,
		RESOURCEDISPLAYTYPE_NDSCONTAINER
	};

	public class ServerEnum : IEnumerable {
		enum ErrorCodes {
			NO_ERROR = 0,
			ERROR_NO_MORE_ITEMS = 259
		};

		[StructLayout(LayoutKind.Sequential)]
		private class NETRESOURCE {
			public ResourceScope dwScope = 0;
			public ResourceType dwType = 0;
			public ResourceDisplayType dwDisplayType = 0;
			public ResourceUsage dwUsage = 0;
			public string lpLocalName = null;
			public string lpRemoteName = null;
			public string lpComment = null;
			public string lpProvider = null;
		};

		private ArrayList aData = new ArrayList();

		public int Count {
			get { return aData.Count; }
		}

		[DllImport("Mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
		private static extern ErrorCodes WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NETRESOURCE p, out IntPtr lphEnum);

		[DllImport("Mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
		private static extern ErrorCodes WNetCloseEnum(IntPtr hEnum);

		[DllImport("Mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
		private static extern ErrorCodes WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref uint lpBufferSize);

		private void EnumerateServers(NETRESOURCE pRsrc, ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType) {
			uint bufferSize = 16384;
			IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
			IntPtr handle = IntPtr.Zero;
			ErrorCodes result;
			uint cEntries = 1;

			result = WNetOpenEnum(scope, type, usage, pRsrc, out handle);

			if (result == ErrorCodes.NO_ERROR) {
				do {
					result = WNetEnumResource(handle, ref cEntries, buffer, ref	bufferSize);

					if (result == ErrorCodes.NO_ERROR) {
						Marshal.PtrToStructure(buffer, pRsrc);

						if (pRsrc.dwDisplayType == displayType)
							aData.Add(pRsrc.lpRemoteName);

						if ((pRsrc.dwUsage & ResourceUsage.RESOURCEUSAGE_CONTAINER) == ResourceUsage.RESOURCEUSAGE_CONTAINER)
							EnumerateServers(pRsrc, scope, type, usage, displayType);
					} else if (result != ErrorCodes.ERROR_NO_MORE_ITEMS)
						break;
				} while (result != ErrorCodes.ERROR_NO_MORE_ITEMS);

				WNetCloseEnum(handle);
			}

			Marshal.FreeHGlobal((IntPtr)buffer);
		}

		public ServerEnum(ResourceScope scope, ResourceType type, ResourceUsage usage, ResourceDisplayType displayType) {
			NETRESOURCE pRsrc = new NETRESOURCE();

			EnumerateServers(pRsrc, scope, type, usage, displayType);
		}
		#region IEnumerable Members

		public IEnumerator GetEnumerator() {
			return aData.GetEnumerator();
		}

		#endregion
	}
}