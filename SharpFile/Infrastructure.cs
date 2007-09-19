using System.Collections.Generic;
using System.IO;
using System.Management;
using System;
using System.Windows.Forms;

namespace SharpFile
{
    public static class Infrastructure
    {
        public static List<DataInfo> GetFiles(string path)
        {
            List<DataInfo> dataInfos = new List<DataInfo>();
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(path + "\\");

            System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            dataInfos.AddRange(
                Array.ConvertAll<System.IO.DirectoryInfo, DataInfo>(directoryInfos, delegate(System.IO.DirectoryInfo di)
                {
                    return new DirectoryInfo(di);
                })
            );

            System.IO.FileInfo[] fileInfos = directoryInfo.GetFiles();
            dataInfos.AddRange(
                Array.ConvertAll<System.IO.FileInfo, DataInfo>(fileInfos, delegate(System.IO.FileInfo fi)
                {
                    return new FileInfo(fi);
                })
            );

            return dataInfos;
        }

        public static List<DriveInfo> GetDrives()
        {
            List<DriveInfo> driveInfos = new List<DriveInfo>();

            foreach (ManagementObject managementObject in GetManagementObjects("Win32_LogicalDisk"))
            {
                if (int.Parse(managementObject["DriveType"].ToString()) >= 3 && 
                    int.Parse(managementObject["DriveType"].ToString()) <= 4)
                {
                    string name = managementObject["Name"].ToString();
                    int driveType = int.Parse(managementObject["DriveType"].ToString());
                    string description = managementObject["Description"] != null ? 
                        managementObject["Description"].ToString() : string.Empty;
                    string providerName = managementObject["ProviderName"] != null ? 
                        managementObject["ProviderName"].ToString() : "";
                    long freeSpace = managementObject["FreeSpace"] != null ? 
                        long.Parse(managementObject["FreeSpace"].ToString()) : 0;
                    long size = managementObject["Size"] != null ? 
                        long.Parse(managementObject["Size"].ToString()) : 0;

                    DriveInfo driveInfo = new DriveInfo(name, providerName, driveType, description, size, freeSpace);
                    driveInfos.Add(driveInfo);
                }
            }

            return driveInfos;
        }

        public static ManagementObjectCollection GetManagementObjects(string path)
        {
            ManagementClass managementClass = new ManagementClass(path);
            ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
            return managementObjectCollection;

            /*
            ManagementBaseObject[] managementBaseObjects = null;
            managementObjectCollection.CopyTo(managementBaseObjects, 0);

            List<ManagementBaseObject> managementObjects = new List<ManagementBaseObject>();
            managementObjects.AddRange(managementBaseObjects);
            return managementObjects;
            */
        }
    }
}
