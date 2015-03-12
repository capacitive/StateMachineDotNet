using System;
using System.Collections.Generic;
using Capiche.ActiveStateMachine;
using Capiche.ApplicationServices;

namespace TelephoneStateMachine
{
    /// <summary>
    /// Configuration class for telephone state machine sample
    /// The sample shows the inner workings of a state machine and therefore has educational character
    /// A lot of issues have been simplified, for example there are only a few action and no guard actions.
    /// </summary>
    public  class TelephoneStateMachineConfiguration {

        // Public members
        // List of valid states for this state machine
        public Dictionary<String, State> TelephoneStateMachineStateList { get; set; }
        // List of activities in the system
        public TelephoneActivities TelephoneActivities { get; set; }

        // Max number of entries in trigger queue
        public int MaxEntries { get; set; }
       // Event Manager
        public EventManager TelephoneEventManager;
        // View Manager
        public ViewManager TelephoneViewManager;
        // Device Manager
        public DeviceManager TelephoneDeviceManager;
        // Logger
        public LogManager TelephoneLogManager;


        /// <summary>
        /// Constructor
        /// </summary>
        public TelephoneStateMachineConfiguration()
        {
            BuildConfig();
        }


        /// <summary>
        /// Build telephone state configuration
        /// </summary>
        private void BuildConfig()
        {
            // Set the maximum queue capacity
            MaxEntries = 50;

            ////////////////////////////////
            // Transitions and actions
            ////////////////////////////////

            // Create the object holding implementation of all system actions
            TelephoneActivities = new TelephoneActivities();

            #region create actions and map action methods into the corresponding action object
            //device actions:
            var actionBellRings = new StateMachineAction("ActionBellRings", TelephoneActivities.ActionBellRings);
            var actionBellSilent = new StateMachineAction("ActionBellSilent", TelephoneActivities.ActionBellSilent);
            var actionLineOff = new StateMachineAction("ActionLineOff", TelephoneActivities.ActionLineOff);
            var actionLineActive = new StateMachineAction("ActionLineActive", TelephoneActivities.ActionLineActive);
            // View actions
            var actionViewPhoneRings = new StateMachineAction("ActionViewPhoneRings", TelephoneActivities.ActionViewPhoneRings);
            var actionViewPhoneIdle = new StateMachineAction("ActionViewPhoneIdle", TelephoneActivities.ActionViewPhoneIdle);
            var actionViewTalking = new StateMachineAction("ActionViewTalking", TelephoneActivities.ActionViewTalking);
            // Error action
            var actionViewErrorPhoneRings = new StateMachineAction("ActionViewErrorPhoneRings", TelephoneActivities.ActionErrorPhoneRings);


            //  Create transitions and corresponding triggers, states need to be added. 
            var emptyList = new List<StateMachineAction>(); // To avoid null reference exceptions, use an empty list where no actions are used.
            
            // transition IncomingCall
            var ICActions = new List<StateMachineAction>();
            ICActions.Add(actionViewPhoneRings);
            var transIncomingCall = new Transition("TransitionIncomingCall", "StatePhoneIdle", "StatePhoneRings", emptyList, ICActions, "OnLineExternalActive");
            
            // transition ErrorPhoneRings - self-transition on PhoneRings state
            var EPRActions = new List<StateMachineAction>();
            EPRActions.Add(actionViewErrorPhoneRings);
            var transErrorPhoneRings = new Transition("TransitionErrorPhoneRings", "StatePhoneRings", "StatePhoneRings", emptyList, EPRActions, "OnBellBroken");//source & target both 'StatePhoneRings'
            
            // transition CallBlocked
            var CBActions = new List<StateMachineAction>();
            CBActions.Add(actionViewPhoneIdle); // Go back to Phone Idle state
            var transCallBlocked = new Transition("TransitionCallBlocked", "StatePhoneRings", "StatePhoneIdle", emptyList, CBActions, "OnReceiverDown");
            
            // transition CallAccepted
            var CAActions = new List<StateMachineAction>();
            CAActions.Add(actionViewTalking);
            var transCallAccepted = new Transition("TransitionCallAccepted", "StatePhoneRings", "StateTalking", emptyList, CAActions, "OnReceiverUp");
            
            //transition CallEnded
            var CEActions = new List<StateMachineAction>();
            CEActions.Add(actionViewPhoneIdle);
            var transCallEnded = new Transition("TransitionCallEnded", "StateTalking", "StatePhoneIdle", emptyList, CEActions, "OnReceiverDown"); 
            #endregion

            #region States Assemble!
            // State: PhoneIdle
            var transitionsPhoneIdle = new Dictionary<String, Transition>();
            var entryActionsPhoneIdle = new List<StateMachineAction>();
            var exitActionsPhoneIdle = new List<StateMachineAction>();
            transitionsPhoneIdle.Add("TransitionIncomingCall", transIncomingCall);
            // Always specify all action lists, even empty ones, do not pass null into a state -> Lists are read via foreach, which will return an error, if they are null!
            var phoneIdle = new State("StatePhoneIdle", transitionsPhoneIdle, entryActionsPhoneIdle, exitActionsPhoneIdle, true);

            // State: PhoneRings 
            //entry and exit actions:
            var entryActionsPhoneRings = new List<StateMachineAction> {actionBellRings};
            var exitActionsPhoneRings = new List<StateMachineAction> {actionBellSilent};
            //transitions
            var transitionsPhoneRings = new Dictionary<String, Transition>();
            transitionsPhoneRings.Add("TransitionCallBlocked", transCallBlocked);
            transitionsPhoneRings.Add("TransitionCallAccepted", transCallAccepted);
            transitionsPhoneRings.Add("TransitionErrorPhoneRings", transErrorPhoneRings);
            var phoneRings = new State("StatePhoneRings", transitionsPhoneRings, entryActionsPhoneRings, exitActionsPhoneRings);

            //Talking
            var transitionsTalking = new Dictionary<string, Transition> { { "TransitionCallEnded", transCallEnded } };
            var entryActionsTalking = new List<StateMachineAction>();
            entryActionsTalking.Add(actionLineActive);
            var exitActionsTalking = new List<StateMachineAction>();
            exitActionsTalking.Add(actionLineOff);
            var talking = new State("StateTalking", transitionsTalking, entryActionsTalking, exitActionsTalking);

            TelephoneStateMachineStateList = new Dictionary<string, State>
            {
                {"StatePhoneIdle", phoneIdle},
                {"StatePhoneRings", phoneRings},
                {"StateTalking", talking}
            };

            #endregion

            #region Application Services
            TelephoneEventManager = EventManager.Instance;
            TelephoneViewManager = ViewManager.Instance;
            TelephoneLogManager = LogManager.Instance;
            TelephoneDeviceManager = DeviceManager.Instance; 
            #endregion
        }

        public void DoEventMappings(TelephoneStateMachine telephoneStateMachine, TelephoneActivities telephoneActivities)
        {
            #region register events
            //use case impl.
            TelephoneEventManager.RegisterEvent("TelephoneUIEvent", telephoneActivities);
            TelephoneEventManager.RegisterEvent("TelephoneDeviceEvent", telephoneActivities);
            //framework/infra. events:
            TelephoneEventManager.RegisterEvent("StateMachineEvent", telephoneStateMachine);
            TelephoneEventManager.RegisterEvent("UINotification", TelephoneViewManager);
            TelephoneEventManager.RegisterEvent("DeviceManagerNotification", TelephoneDeviceManager);
            TelephoneEventManager.RegisterEvent("EventManagerEvent", TelephoneEventManager);
            TelephoneEventManager.RegisterEvent("ViewManagerEvent", TelephoneViewManager);
            TelephoneEventManager.RegisterEvent("DeviceManagerEvent", TelephoneDeviceManager); 
            #endregion

            //subscribe event handlers to registered with the event manager
            #region event mappings
            //logging
            TelephoneEventManager.SubscribeEvent("DeviceManagerNotification", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("EventManagerEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("ViewManagerEvent", "LogEventHandler", TelephoneLogManager);
            TelephoneEventManager.SubscribeEvent("DeviceManagerEvent", "LogEventHandler", TelephoneLogManager); 

            // Notifications / Triggers;
            TelephoneEventManager.SubscribeEvent("DeviceManagerNotification", "InternalNotificationHandler", telephoneStateMachine);

            //system event listeners in managers
            TelephoneEventManager.SubscribeEvent("TelephoneUIEvent", "ViewCommandHandler", TelephoneViewManager);
            TelephoneEventManager.SubscribeEvent("TelephoneDeviceEvent", "DeviceCommandHandler", TelephoneDeviceManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "SystemEventHandler", TelephoneViewManager);
            TelephoneEventManager.SubscribeEvent("StateMachineEvent", "SystemEventHandler", TelephoneDeviceManager);
            TelephoneEventManager.SubscribeEvent("ViewManagerEvent", "DeviceCommandHandler", TelephoneDeviceManager);//send UI button clicks to device manager

            #endregion
        }
    }
}
