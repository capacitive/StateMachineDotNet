using System;
using Capiche.Common;

namespace TelephoneStateMachine
{
    public class TelephoneActivities
    {
        //events to communicate from state machine to managers - wiring will be done via the event manager
        public event EventHandler<StateMachineEventArgs> TelephoneUIEvent;
        public event EventHandler<StateMachineEventArgs> TelephoneDeviceEvent;

        #region device events
        public void ActionBellRings()
        {
            RaiseDeviceEvent("Bell", "Rings");
        }

        public void ActionBellSilent()
        {
            RaiseDeviceEvent("Bell", "Silent");
        }

        public void ActionLineOff()
        {
            RaiseDeviceEvent("PhoneLine", "OffInternal");
        }

        public void ActionLineActive()
        {
            RaiseDeviceEvent("PhoneLine", "ActiveInternal");
        } 
        #endregion

        #region view actions
        public void ActionViewPhoneRings()
        {
            RaiseTelephoneUIEvent("ViewPhoneRings");
        }

        public void ActionViewPhoneIdle()
        {
            RaiseTelephoneUIEvent("ViewPhoneIdle");
            System.Media.SystemSounds.Beep.Play();
        }

        public void ActionViewTalking()
        {
            RaiseTelephoneUIEvent("ViewTalking");
        }

        public void ActionErrorPhoneRings()
        {
            RaiseTelephoneUIEvent("ViewErrorPhoneRings");
        }
        #endregion

        #region event methods
        private void RaiseTelephoneUIEvent(string command)
        {
            var teleArgs = new StateMachineEventArgs(command, "UI command", StateMachineEventType.Command, "State machine action", "ViewManager");
            TelephoneUIEvent(this, teleArgs);
        }

        private void RaiseDeviceEvent(string target, string command)
        {
            var teleArgs = new StateMachineEventArgs(command, "Device command", StateMachineEventType.Command, "State machine action", target);
            TelephoneDeviceEvent(this, teleArgs);
        } 
        #endregion
    }
}
