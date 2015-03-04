using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveStateMachine
{
    public class StateMachineAction
    {
        #region public members
        public string Name { get; private set; } 
        #endregion

        #region private members
        //delegate pointing to the implementation of the method to be executed
        private Action _method;
        #endregion

        /// <summary>
        /// ctor for state machine action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public StateMachineAction(string name, Action method)
        {
            Name = name;
            _method = method;
        }

        /// <summary>
        /// Method running the action.
        /// Will be called e.g. by the state machine, when a transition is executed.
        /// Could also be used as a guard, entry or exit action
        /// </summary>
        public void Execute()
        {
            //invoke the state machine action method:
            _method.Invoke();
        }
    }
}
