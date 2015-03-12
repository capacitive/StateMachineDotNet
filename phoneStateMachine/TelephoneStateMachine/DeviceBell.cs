using System;
using Capiche.ApplicationServices;

namespace TelephoneStateMachine
{
    public class DeviceBell : Device
    {
        public bool Ringing { get; set; }

        #region device functions
        public void Rings()
        {
            try
            {
                throw (new SystemException("OnBellBroken"));
                Ringing = true;
                System.Media.SystemSounds.Hand.Play();
            }
            catch (Exception exc)
            {
                DoNotificationCallback(exc.Message == "OnBellBroken" ? "OnBellBroken" : "CompleteFailure", exc.Message, "Bell");
            }
        }

        public void Silent()
        {
            Ringing = false;
        } 
        #endregion

        public DeviceBell(string deviceName, Action<string, string, string> eventCallback) : base(deviceName, eventCallback) { }

        public override void OnInit()
        {
            Ringing = false;
        }
    }
}
