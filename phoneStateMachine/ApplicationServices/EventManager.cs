using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Common;

namespace ApplicationServices
{
    public class EventManager
    {
        //collection of registered events
        private Dictionary<string, object> EventList;

        /// <summary>
        /// event manager event used for logging
        /// </summary>
        public event EventHandler<StateMachineEventArgs> EventManagerEvent;

        #region singleton implementation
        private static readonly Lazy<EventManager> _eventManager = new Lazy<EventManager>(() => new EventManager());

        public static EventManager Instance { get { return _eventManager.Value; }}

        private EventManager()
        {
            EventList = new Dictionary<string, object>();
        }
        #endregion

        /// <summary>
        /// registration of an event used in the system
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="source"></param>
        public void RegisterEvent(string eventName, object source)
        {
            EventList.Add(eventName, source);
        }

        /// <summary>
        /// Subscription method maps handler method in a sink object to an event of the source object.
        /// method signatures between the delegate and handler need to match
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerMethodName"></param>
        /// <param name="sink"></param>
        /// <returns></returns>
        public bool SubscribeEvent(string eventName, string handlerMethodName, object sink)
        {
            try
            {
                //get event from list:
                var evt = EventList[eventName];
                //determine meta data from event and handler:
                var eventInfo = evt.GetType().GetEvent(eventName);
                var methodInfo = sink.GetType().GetMethod(handlerMethodName);
                //create new delegate mapping event to handler:
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, sink, methodInfo);
                eventInfo.AddEventHandler(evt, handler);
                return true;
            }
            catch (Exception exc)
            {
                var message = "Exception thrown while subscribing to handler.  Event:" + eventName + " - Handler: " + handlerMethodName;
                RaiseEventManagerEvent("EventManagerSystemEvent", message, StateMachineEventType.System);
                return false;
            }
        }

        private void RaiseEventManagerEvent(string eventName, string eventInfo, StateMachineEventType eventType)
        {
            var newArgs = new StateMachineEventArgs(eventName, eventInfo, eventType, "Event Manager");
            if (EventManagerEvent != null) EventManagerEvent(this, newArgs);
        }
    }
}
