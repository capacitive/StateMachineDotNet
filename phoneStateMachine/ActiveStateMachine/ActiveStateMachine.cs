using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveStateMachine
{
    /// <summary>
    /// base class for ANY state machine implementation
    /// </summary>
    class ActiveStateMachine
    {
        #region public members
        public Dictionary<string, State> StateList { get; private set; }
        public BlockingCollection<string> TriggerQueue { get; private set; }
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }
        public EngineState StateMachineEngine { get; private set; }
        public event EventHandler<StateMachineEventArgs> StateMachineEvent; 
        #endregion

        private Task _queueWorkerTask;
        private readonly State _initialState;
        private ManualResetEvent _resumer;
        private CancellationTokenSource _tokenSource;

        public ActiveStateMachine(Dictionary<string, State> stateList, int queueCapacity)
        {
            //cnfigure state machine
            StateList = stateList;
            //set initial state:
            _initialState = new State("InitialState", null, null, null);
            //collection taking in all triggers. Is thread-safe, blocking as well as FIFO
            //limiting its capacity protects against DOS-style errors or attacks
            TriggerQueue = new BlockingCollection<string>(queueCapacity);

            //init
            InitStateMachine();
            //raise an event
            RaiseStateMachineSystemEvent("StateMachine: Initialized", "System ready to start");
            StateMachineEngine = EngineState.Initialized;
        }

        public void InitStateMachine()
        {
            PreviousState = _initialState;

            foreach (var state in StateList)
            {
                if (state.Value.IsDefaultState)
                {
                    CurrentState = state.Value;
                    RaiseStateMachineSystemCommand("OnInit", "StateMachineInitialized");
                }
            }

            //this is the synchronization object fro resuming - passing true means non-blocking (signaled):
            _resumer = new ManualResetEvent(true);
        }

        public void Start()
        {
            _tokenSource = new CancellationTokenSource();
            _queueWorkerTask = Task.Factory.StartNew(QueueWorkerMethod, _tokenSource, TaskCreationOptions.LongRunning);

            //set engine state:
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Started", "System running.");
        }

        public void Pause()
        {
            //set engine state:
            StateMachineEngine = EngineState.Paused;
            _resumer.Reset();
            RaiseStateMachineSystemEvent("StateMachine: Paused", "System waiting.");
        }

        public void Resume()
        {
            //worker task exists, resume from where it was paused
            _resumer.Set();
            //set engine state:
            StateMachineEngine = EngineState.Running;
            RaiseStateMachineSystemEvent("StateMachine: Running", "System running.");
        }

        /// <summary>
        /// ends queue processing
        /// </summary>
        public void Stop()
        {
            //cancel processing
            _tokenSource.Cancel();
            //wait for thread to return:
            _queueWorkerTask.Wait();
            //free resources:
            _queueWorkerTask.Dispose();
            //set engine state:
            StateMachineEngine = EngineState.Stopped;
            RaiseStateMachineSystemEvent("StateMachine: Stopped", "System execution stopped.");
        }

        /// <summary>
        /// worker method for trigger queue
        /// </summary>
        /// <param name="obj"></param>
        private void QueueWorkerMethod(object obj)
        {
            //blocks execution until it's reset.  Used to pause the state machine:
            _resumer.WaitOne();

            //block the queue and loop through all triggers available.  Blocking queue guarantees FIFO and the GetConsumingEnumerable
            //automatically removes triggers from the queue.
            try
            {
                foreach (var trigger in TriggerQueue.GetConsumingEnumerable())
                {
                    if (_tokenSource.IsCancellationRequested)
                    {
                        RaiseStateMachineSystemEvent("StateMachine: QueueWorker", "Processing cancelled!");
                        return;
                    }

                    //compare trigger:
                    foreach (
                        var transition in
                            CurrentState.StateTransitionList.Where(transition => trigger == transition.Value.Trigger))
                    {
                        ExecuteTransition(transition.Value);
                    }
                }
            }
            catch (Exception exc)
            {
                RaiseStateMachineSystemEvent("StateMachine: QueueWorker", "Processing cancelled! Exception: " + exc);
                Start();
            }

        }

        protected virtual void ExecuteTransition(Transition transition)
        {
            //default transition validation
            if (CurrentState.StateName != transition.SourceStateName)
            {
                string message = String.Format("Transition has wrong source state {0}, when system is in {1}",
                    transition.SourceStateName, CurrentState.StateName);
                RaiseStateMachineSystemEvent("StateMachine: Default guard execute transition.", message);
                return;
            }

            if (!StateList.ContainsKey(transition.TargetStateName))
            {
                string message = String.Format("Transition has wrong target state {0}, when system is in {1}. State not in global config.",
                    transition.TargetStateName, CurrentState.StateName);
                RaiseStateMachineSystemEvent("StateMachine: Default guard execute transition.", message);
                return;
            }

            //run all exit actions of the old state:
            CurrentState.ExitActions.ForEach(a => a.Execute());

            //run all guards of the transition:
            transition.GuardList.ForEach(g => g.Execute());
            string info = transition.GuardList.Count + " guard actios executed.";
            RaiseStateMachineSystemEvent("StateMachine: ExecuteTransition", info);

            //run all actions of the transition:
            transition.TransitionActionList.ForEach(t => t.Execute());

            //IMPORTANT: state change
            info = transition.TransitionActionList.Count + " transition actions executed.";
            RaiseStateMachineSystemEvent("StateMachine: Begin state change...", info);

            //1st resolve the target state with the help of its name:
            var targetState = GetStateFromStateList(transition.TargetStateName);

            //transition successful - change state:
            PreviousState = CurrentState;
            CurrentState = targetState;

            //run all entry actins of the new state:
            foreach (var entryAction in CurrentState.EntryActions)
            {
                entryAction.Execute();
            }
            RaiseStateMachineSystemEvent("StateMachine: State change completed successfully.",
                String.Format("Previous state: {0} - New state: {1}", PreviousState.StateName, CurrentState.StateName));
        }    

        /// <summary>
        /// Enter a trigger into the queue
        /// </summary>
        /// <param name="newTrigger"></param>
        private void EnterTrigger(string newTrigger)
        {
            //put trigger in queue:
            try
            {
                TriggerQueue.Add(newTrigger);
            }
            catch (Exception exc)
            {
                RaiseStateMachineSystemEvent("ActiveStateMachine - error entering trigger", newTrigger + " - " + exc);
            }
            //raise an event:
            RaiseStateMachineSystemEvent("ActiveStateMachine - Trigger entered", newTrigger);
        }

        private State GetStateFromStateList(string targetStateName)
        {
            return StateList[targetStateName];
        }

        #region event infrastructure
        private void RaiseStateMachineSystemEvent(string eventName, string eventInfo)
        {
            if (StateMachineEvent != null) StateMachineEvent(this, new StateMachineEventArgs(eventName, eventInfo, StateMachineEventType.System, "State machine", ""));
        }

        private void RaiseStateMachineSystemCommand(string eventName, string eventInfo)
        {
            if (StateMachineEvent != null) StateMachineEvent(this, new StateMachineEventArgs(eventName, eventInfo, StateMachineEventType.Command, "State machine", ""));
        }

        /// <summary>
        /// Event handler for internal events triggering the state machine
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void InternalNotificationHandler(object sender, StateMachineEventArgs args)
        {
            EnterTrigger(args.EventName);
        }
        #endregion
    }

    public enum EngineState
    {
        Running,
        Stopped,
        Paused,
        Initialized
    }
}
