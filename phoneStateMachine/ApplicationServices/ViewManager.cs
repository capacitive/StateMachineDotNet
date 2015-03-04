using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace ApplicationServices
{
    public class ViewManager
    {
        private string[] _viewStates;
        private string DefaultViewState;
        //UI - make this a Dictionary<string, IUserInterface>, if you have to handle more than one
        private IUserInterface _UI;

        public event EventHandler<StateMachineEventArgs> ViewManagerEvent;
        public string CurrentView { get; private set; }
        public IViewStateConfiguration ViewStateConfiguration { get; set; }

        #region singleton implementation
        private static readonly Lazy<ViewManager> _viewManager = new Lazy<ViewManager>(() => new ViewManager());

        public static ViewManager Instance { get { return _viewManager.Value; }}

        private ViewManager()
        {
        }
        #endregion

        public void LoadViewStateConfiguration(IViewStateConfiguration viewStateConfiguration, IUserInterface userInterface)
        {
            ViewStateConfiguration = viewStateConfiguration;
            _viewStates = viewStateConfiguration.ViewStateList;
            _UI = userInterface;
            DefaultViewState = viewStateConfiguration.DefaultViewState;
        }

        public void ViewCommandHandler(object sender, StateMachineEventArgs args)
        {
            try
            {
                if (_viewStates.Contains(args.EventName))
                {
                    _UI.LoadViewState(args.EventName);
                    CurrentView = args.EventName;
                    RaiseViewManagerEvent("View Manager Command", "Successfully loaded view state:" + args.EventName);
                }
                else
                {
                    RaiseViewManagerEvent("View Manager Command", "View state not found!");
                }
            }
            catch (Exception exc)
            {
                RaiseViewManagerEvent("View Manager Command - Error", exc.ToString());
            }
        }

        /// <summary>
        /// handler method for special system events, e.g. initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SystemEventHandler(object sender, StateMachineEventArgs args)
        {
            //init:
            if (args.EventName == "OnInit")
            {
                _UI.LoadViewState(DefaultViewState);
                CurrentView = DefaultViewState;
            }
        }

        public void RaiseViewManagerEvent(string eventName, string eventInfo, StateMachineEventType eventType = StateMachineEventType.System)
        {
            var newVMArgs = new StateMachineEventArgs(eventName, "View amanager event: " + eventInfo, eventType, "View Manager");
            if (ViewManagerEvent != null) ViewManagerEvent(this, newVMArgs);
        }

        public void RaiseUICommand(string command, string info, string source, string target)
        {
            var newUIArgs = new StateMachineEventArgs(command, info, StateMachineEventType.Command, source, target);
            if (ViewManagerEvent != null) ViewManagerEvent(this, newUIArgs);
        }
    }
}
