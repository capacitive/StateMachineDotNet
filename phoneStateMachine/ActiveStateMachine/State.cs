using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveStateMachine
{
    class State
    {
        public string StateName { get; private set; }
        public Dictionary<string, Transition> StateTransitionList { get; private set; }
        public List<StateMachineAction> EntryActions { get; private set; }
        public List<StateMachineAction> ExitActions { get; private set; }
        public bool IsDefaultState { get; private set; }

        public State(string stateName, Dictionary<string, Transition> stateTransitionList, List<StateMachineAction> entryActions, List<StateMachineAction> exitActions)
        {
            StateName = stateName;
            StateTransitionList = stateTransitionList;
            EntryActions = entryActions;
            ExitActions = exitActions;
        }
    }
}
