using System;
using ApplicationServices;

namespace TelephoneStateMachine
{
    public class DevicePhoneLine : Device
    {
        public bool LineActiveExternal { get; set; }
        public bool LineActiveInternal { get; set; }

        #region device functions

        //Simulation.  Would not normally be public - called by an internal driver of TCP operation
        public void ActiveExternal()
        {
            LineActiveExternal = true;
            DoNotificationCallback("OnLineExternalActive", "Phone line set to active", DeviceName);
        }

        public void ActiveInternal()
        {
            LineActiveInternal = true;
        }

        public void OffInternal()
        {
            LineActiveInternal = false;
            System.Media.SystemSounds.Hand.Play();
        }

        #endregion

        public DevicePhoneLine(string deviceName, Action<string, string, string> eventCallback) : base(deviceName, eventCallback)
        {
        }

        public override void OnInit()
        {
            LineActiveExternal = false;
            LineActiveInternal = false;
        }
    }
}
