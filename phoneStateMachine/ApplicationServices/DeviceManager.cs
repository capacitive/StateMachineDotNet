using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ApplicationServices
{
    public class DeviceManager
    {
        #region singleton implementation
        private static readonly Lazy<DeviceManager> _deviceManager = new Lazy<DeviceManager>(() => new DeviceManager());

        public static DeviceManager Instance { get { return _deviceManager.Value; } }

        private DeviceManager()
        {
            DeviceList = new Dictionary<string, object>();
        }
        #endregion

        private Dictionary<string, object> DeviceList { get; set; }

        /// <summary>
        /// event manager event used for logging
        /// </summary>
        public event EventHandler<StateMachineEventArgs> DeviceManagerEvent;
        public event EventHandler<StateMachineEventArgs> DeviceManagerNotification;

        //add a system device
        public void AddDevice(string name, object device)
        {
            DeviceList.Add(name, device);
            RaiseDeviceManagerEvent("Added device", name);
        }

        //remove a system device
        public void RemoveDevice(string name)
        {
            DeviceList.Remove(name);
            RaiseDeviceManagerEvent("Removed device", name);
        }

        public void CommandEventHandler(object sender, StateMachineEventArgs args)
        {
            if (args.EventType != StateMachineEventType.Command) return;

            try
            {
                if (!DeviceList.Keys.Contains(args.Target)) return;

                //by convention, device commands and method names must match:
                var device = DeviceList[args.Target];

                MethodInfo deviceMethod = device.GetType().GetMethod("OnInit");
                deviceMethod.Invoke(device, new object[] { });
                RaiseDeviceManagerEvent("DeviceCommand", "Successful device command: " + args.Target + " - " + args.EventName);
            }
            catch (Exception exc)
            {
                RaiseDeviceManagerEvent("DeviceCommand - Error", exc.ToString());
            }
        }

        public void SystemEventHandler(object sender, StateMachineEventArgs args)
        {
            if (args.EventName == "OnInit" && args.EventType == StateMachineEventType.Command)
            {
                foreach (var device in DeviceList)
                {
                    try
                    {
                        MethodInfo initMethod = device.Value.GetType().GetMethod("OnInit");
                        initMethod.Invoke(device.Value, new object[] { });
                        RaiseDeviceManagerEvent("DeviceCommand - Initialization of device", device.Key);
                    }
                    catch (Exception exc)
                    {
                        RaiseDeviceManagerEvent("DeviceCommand - Initialization error device" + device.Key, exc.ToString());
                    }
                }
            }
        }

        private void RaiseDeviceManagerEvent(string eventName, string eventInfo)
        {
            var newArgs = new StateMachineEventArgs(eventName, "Devicemanager event: " + eventInfo, StateMachineEventType.System, "Device Manager");
            if (DeviceManagerEvent != null) DeviceManagerEvent(this, newArgs);
        }

        public void RaiseDeviceManagerNotification(string command, string info, string source)
        {
            var newArgs = new StateMachineEventArgs(command, info, StateMachineEventType.Notification, source, "State Machine");
            if (DeviceManagerNotification != null) DeviceManagerNotification(this, newArgs);
        }

        public void LoadDeviceConfiguration(IDeviceConfiguration deviceConfiguration)
        {
            DeviceList = deviceConfiguration.Devices;
        }
    }
}
