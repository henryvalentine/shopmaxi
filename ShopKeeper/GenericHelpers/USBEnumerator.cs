
using System.Collections.Generic;
using System.Management;
namespace ShopKeeper.GenericHelpers
{
    public static class USBEnumerator
    {
       public class USBDeviceInfo
        {
            public USBDeviceInfo(string deviceId, string pnpdeviceId, string description)
            {
                DeviceId = deviceId;
                PnpdeviceId = pnpdeviceId;
                Description = description;
            }
            public string DeviceId { get; private set; }
            public string PnpdeviceId { get; private set; }
            public string Description { get; private set; }
        }

       public  static List<USBDeviceInfo> GetUSBDevices()
        {
          var devices = new List<USBDeviceInfo>();

          //Win32_PnPEntity  Win32_USBHub  Win32_Printer

          ManagementObjectCollection collection;
          using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_Printer"))
            collection = searcher.Get();      

          foreach (var device in collection)
          {
            devices.Add(new USBDeviceInfo(
            (string)device.GetPropertyValue("DeviceID"),
            (string)device.GetPropertyValue("PNPDeviceID"),
            (string)device.GetPropertyValue("Description")
            ));
          }

          collection.Dispose();
          return devices;
        }
  
        //Console.WriteLine("Device ID: {0}, PNP Device ID: {1}, Description: {2}",
         
    }

}