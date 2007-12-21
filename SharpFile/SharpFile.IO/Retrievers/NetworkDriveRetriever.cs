using System.Management;
using System.Collections.Generic;
using SharpFile.IO.ParentResources;
using System;
using System.Runtime.InteropServices;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
    public class NetworkDriveRetriever : IResourceRetriever {
        private IChildResourceRetriever childResourceRetriever;
        private bool isDetailInfoRetrievable = false;

        public IEnumerable<IResource> Get() {
            SelectQuery query = 
                new SelectQuery("SELECT Name, ProviderName FROM Win32_LogicalDisk WHERE DriveType = 4");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in searcher.Get()) {
                bool isFileSystemWatchSupported = false;
                string name = mo["Name"].ToString() + @"\";
                string providerName = mo["ProviderName"].ToString();
                System.IO.DriveInfo driveInfo = new System.IO.DriveInfo(name);

                if (isDetailInfoRetrievable) {
                    isFileSystemWatchSupported = getDetailedInformation(providerName);
                }

                NetworkDriveInfo networkDriveInfo = 
                    new NetworkDriveInfo(driveInfo, this.ChildResourceRetriever, providerName, isFileSystemWatchSupported);

                yield return networkDriveInfo;
            }
        }

        private bool getDetailedInformation(string providerName) {
            bool isFileSystemWatchSupported = false;
            string rootProviderName = providerName;

            if (rootProviderName.Split('\\').Length > 3) {
                rootProviderName = @"\\" + rootProviderName.Split('\\')[2] + @"\";
            } else {
                rootProviderName += @"\";
            }

            // Connect to the remote computer.
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Timeout = new TimeSpan(500);

            // Point to machine.
            ManagementScope scope = new ManagementScope(rootProviderName + "root\\cimv2", connectionOptions);

            SelectQuery operatingSystemQuery = new SelectQuery("SELECT Caption FROM Win32_OperatingSystem");
            EnumerationOptions enumerationOptions = new EnumerationOptions();
            enumerationOptions.ReturnImmediately = true;
            enumerationOptions.Timeout = new TimeSpan(500);

            ManagementObjectSearcher operatingSystemSearcher =
                new ManagementObjectSearcher(scope, operatingSystemQuery, enumerationOptions);

            string operatingSystem = string.Empty;

            try {
                foreach (ManagementObject operatingSystemManagementObject in operatingSystemSearcher.Get()) {
                    operatingSystem = operatingSystemManagementObject["Caption"].ToString();
                    break;
                }
            } catch (COMException ex) {
                string blob = ex.Message;
                // Log some exception somewhere, but I don't really care. 
                // We just assume we can't get an operating system from this computer.
            }

            switch (operatingSystem) {
                case "Windows":
                    isFileSystemWatchSupported = true;
                    break;
            }

            return isFileSystemWatchSupported;
        }

        public IChildResourceRetriever ChildResourceRetriever {
            get {
                return childResourceRetriever;
            }
            set {
                childResourceRetriever = value;
            }
        }
    }
}
