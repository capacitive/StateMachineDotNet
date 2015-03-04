using System;
using ApplicationServices;

namespace TelephoneStateMachine
{
    public class DeviceBell : Device
    {
        public bool Ringing { get; set; }

        #region device functions
        public void Rings()
        {
            Ringing = true;
            System.Media.SystemSounds.Hand.Play();
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
