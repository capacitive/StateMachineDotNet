using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveStateMachine
{
    class StateMachineEventArgs
    {
        public string EventName { get; set; }
        public string EventInfo { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public StateMachineEventType EventType { get; set; }

        public StateMachineEventArgs(string eventName, string eventInfo, StateMachineEventType eventType, string source, string target)
        {
            EventName = eventName;
            EventInfo = eventInfo;
            TimeStamp = DateTime.Now;//auto-create when args are created
            Source = source;
            Target = target;
            EventType = eventType;
        }
    }

    enum StateMachineEventType
    {
        System, Command, Notification, External
    }
}
