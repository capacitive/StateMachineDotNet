using System.Collections.Generic;
using ActiveStateMachine;
using ApplicationServices;

namespace TelephoneStateMachine
{
    public class TelephoneStateMachineConfiguration
    {
        //list of valid states:
        public Dictionary<string, State> TelephoneStateList { get; set; }
        //list of activities in the system:
        public TelephoneActivities TelephoneActivities { get; set; }
        //max # of entries in trigger queue:
        public int MaxEntries = 50;
        public EventManager TelephoneEventManager;
        public ViewManager TelephoneViewManager;
        public DeviceManager TelephoneDeviceManager;
        public LogManager TelephoneLogManager;

        public TelephoneStateMachineConfiguration()
        {
            BuildConfig();
        }

        private void BuildConfig()
        {
            
        }
    }
}
