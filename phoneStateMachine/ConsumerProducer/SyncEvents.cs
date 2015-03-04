namespace ConsumerProducer
{
    using System;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    public class SyncEvents //aka traffic lights
    {
        public SyncEvents()
        {
            /*instantiated every time producer adds item into BlockingCollection,
             * switches its state as it gets signalled,
             * resets its atate to the original one
             * provides gated access to BlockingCollection - only single thread can pass,
               shows producer has entered new item in the BlockingCollection*/
            _newItemEvent = new AutoResetEvent(false);

            //doesn't auto-reset as in above,
            //use as "turn off switch" - stop threads at end of program execution
            _exitThreadEvent = new ManualResetEvent(false);
            //put both events above into this array:
            _eventArray = new WaitHandle[2];
            _eventArray[0] = _newItemEvent;
            _eventArray[1] = _exitThreadEvent;
        }

        public EventWaitHandle ExitThreadEvent
        {
            get { return _exitThreadEvent; }
        }
        public EventWaitHandle NewItemEvent
        {
            get { return _newItemEvent; }
        }
        public WaitHandle[] EventArray
        {
            get { return _eventArray; }
        }

        private EventWaitHandle _newItemEvent;
        private EventWaitHandle _exitThreadEvent;
        private WaitHandle[] _eventArray;
    }
}
