using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices
{
    public abstract class Device
    {
        public string DeviceName { get; private set; }
        private Action<string, string, string> _deviceEventMethod;

        protected Device(string deviceName, Action<string, string, string> eventCallback)
        {
            DeviceName = deviceName;
            _deviceEventMethod = eventCallback;        
        }

        public abstract void OnInit();

        public void RegisterEvents(Action<string, string, string> method)
        {
            _deviceEventMethod = method;
        }

        public void DoNotificationCallback(string name, string eventInfo, string source)
        {
            _deviceEventMethod.Invoke(name, eventInfo, source);
        }
    }
}
