using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DeviceInfo
{
    public static class DeviceProcess
    {
        private const string KeyFilePath = "C:\\test\\device_keys.txt";



        public static string PcName()
        {
            return Environment.UserName.ToString();
        }

        public static string PcNameDomain()
        {
            return Environment.UserDomainName.ToString();
        }

        public static string IP()
        {
            return Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        }

        public static string Version()
        {
            return Application.ProductVersion.ToString();
        }

        public static string ModemIp()
        {
            var webClient = new WebClient();
            string dnsString = webClient.DownloadString("http://checkip.dyndns.org");
            dnsString = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(dnsString).Value;
            webClient.Dispose();
            return dnsString.ToString();
        }

        public static string getCPUInfo()
        {
            string processorInfo = null;
            string processorSerial = null;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * FROM WIN32_Processor");
            ManagementObjectCollection mObject = searcher.Get();

            foreach (ManagementObject obj in mObject)
            {
                processorSerial = obj["ProcessorId"].ToString();
            }
            processorInfo = processorSerial;

            return processorInfo;
        }

        public static string getHddSerial()
        {
            String hddSerial = null;

            ManagementObjectSearcher hddSearcher = new ManagementObjectSearcher("Select * FROM WIN32_DiskDrive");
            ManagementObjectCollection mObject = hddSearcher.Get();

            foreach (ManagementObject obj in mObject)
            {
                hddSerial = (string)obj["SerialNumber"];
            }
            return hddSerial;
        }

        public static string Mac()
        {
            ManagementClass manager = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject obj in manager.GetInstances())
            {
                if ((bool)obj["IPEnabled"])
                {
                    return obj["MacAddress"].ToString();
                }
            }

            return String.Empty;
        }

        public static string getRamSize()
        {
            string ramSizeInfo = null;
            ManagementObjectSearcher ramSearcher = new ManagementObjectSearcher("Select * From Win32_ComputerSystem");

            foreach (ManagementObject mObject in ramSearcher.Get())
            {
                double Ram_Bytes = (Convert.ToDouble(mObject["TotalPhysicalMemory"]));
                double ramgb = Ram_Bytes / 1073741824;
                double ramSize = Math.Ceiling(ramgb);
                ramSizeInfo = ramSize.ToString() + " GB";
            }
            return ramSizeInfo;
        }

        public static string getVideoControllerInfo()
        {
            string videoControllerInfo = null;
            string name = null;
            string ram = null;
            string horizontalResolution = null;
            string verticalResolution = null;
            string deviceID = null;

            ManagementObjectSearcher vidSearcher = new ManagementObjectSearcher("Select * from Win32_VideoController Where availability='3'");

            foreach (ManagementObject mObject in vidSearcher.Get())
            {
                name = mObject["name"].ToString();
                ram = (Convert.ToDouble(mObject["AdapterRam"]) / 1073741824).ToString();
                deviceID = (string)mObject["DeviceID"];
                horizontalResolution = mObject["CurrentHorizontalResolution"].ToString();
                verticalResolution = mObject["CurrentVerticalResolution"].ToString();
            }
            videoControllerInfo = name + "\r\n Ram Miktarı : " + ram + " GB \r\n ID : " + deviceID + "\r\n Çözünürlük :" + horizontalResolution + " x " + verticalResolution;

            return videoControllerInfo;
        }




        //**************************************
        static string GetSavedDeviceKey()
        {
            if (File.Exists(KeyFilePath))
            {
                return File.ReadAllText(KeyFilePath).Trim();
            }
            return "";
        }
        public static void GenerateAndSaveDeviceKey()
        {
            string key = GetSavedDeviceKey();

            if (string.IsNullOrWhiteSpace(key))
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random rnd = new Random();
                string deviceKey = new string(Enumerable.Repeat(chars, 30).Select(s => s[rnd.Next(s.Length)]).ToArray());

                using (StreamWriter streamWriter = new StreamWriter(KeyFilePath))
                {
                    streamWriter.Write(deviceKey);
                }
            }


        }

      
    }
}

