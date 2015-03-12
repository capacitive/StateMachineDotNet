using System;
using Capiche.ApplicationServices;

namespace TelephoneStateMachine
{
    public class DeviceReceiver: Device
    {
        public bool ReceiverLifted { get; set; }

        #region device functions

        public void OnReceiverUp()
        {
            ReceiverLifted = true;
            DoNotificationCallback("OnReceiverUp", "Receiver lifted", "Receiver");
        }

        public void OnReceiverDown()
        {
            ReceiverLifted = false;
            DoNotificationCallback("OnReceiverDown", "Receiver down", "Receiver");
        }

        #endregion

        public DeviceReceiver(string deviceName, Action<string, string, string> eventCallback) : base(deviceName, eventCallback)
        {
        }

        public override void OnInit()
        {
            ReceiverLifted = false;
        }
    }
}
