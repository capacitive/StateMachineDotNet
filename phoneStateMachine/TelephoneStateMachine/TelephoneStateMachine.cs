using Capiche.ActiveStateMachine;

namespace TelephoneStateMachine
{
    public class TelephoneStateMachine : ActiveStateMachine
    {

        private TelephoneStateMachineConfiguration _config;

        public TelephoneStateMachine(TelephoneStateMachineConfiguration configuration)
            : base(configuration.TelephoneStateMachineStateList, configuration.MaxEntries)
        {
            _config = configuration;

            configuration.DoEventMappings(this, _config.TelephoneActivities);
        }
    }
}
