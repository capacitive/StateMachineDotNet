using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ApplicationServices
{
    public class LogManager
    {
        #region singleton implementation
        private static readonly Lazy<LogManager> _logManager = new Lazy<LogManager>(() => new LogManager());

        public static LogManager Instance { get { return _logManager.Value; } }

        private LogManager()
        {
        }
        #endregion

        public void LogEventHandler(object sender, StateMachineEventArgs args)
        {
            if (args.EventType != StateMachineEventType.Notification)
            {
                Debug.Print(args.TimeStamp + " SystemEvent:" + args.EventName + 
                    " - Info: " + args.EventInfo + " - StateMachineArgumentType: " + args.EventType + " - Source: " + args.Source + " - Target: " + args.Target);
            }
            else
            {
                Debug.Print(args.TimeStamp + " Notification:" + args.EventName +
                    " - Info: " + args.EventInfo + " - StateMachineArgumentType: " + args.EventType + " - Source: " + args.Source + " - Target: " + args.Target);
            }
        }
    }
}
