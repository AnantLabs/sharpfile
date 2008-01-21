using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.IO.ParentResources;

namespace SharpFile.IO.Retrievers {
    public class NetworkDriveRetriever : IResourceRetriever {
        private ChildResourceRetrievers childResourceRetrievers;
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
                    new NetworkDriveInfo(driveInfo, this.ChildResourceRetrievers, providerName, isFileSystemWatchSupported);

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
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Access is unauthorized for {0}.",
                    providerName);
            }

            switch (operatingSystem) {
                case "Windows":
                    isFileSystemWatchSupported = true;
                    break;
            }

            return isFileSystemWatchSupported;
        }

        public ChildResourceRetrievers ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }
    }
}
