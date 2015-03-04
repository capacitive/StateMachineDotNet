using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveStateMachine
{
    class Transition
    {
        public string Name { get; private set; }
        public string SourceStateName { get; private set; }
        public string TargetStateName { get; private set; }
        public List<StateMachineAction> GuardList { get; private set; }
        public List<StateMachineAction> TransitionActionList { get; private set; }
        public string Trigger { get; private set; }

        public Transition(string name, string sourceStateName, string targetStateName, List<StateMachineAction> guardList, List<StateMachineAction> transitionActionList, string trigger)
        {
            Name = name;
            SourceStateName = sourceStateName;
            TargetStateName = targetStateName;
            GuardList = guardList;
            TransitionActionList = transitionActionList;
            Trigger = trigger;
        }
    }
}
