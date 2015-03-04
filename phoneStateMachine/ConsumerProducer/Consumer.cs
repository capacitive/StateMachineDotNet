using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ConsumerProducer
{
    public class Consumer
    {
        public Consumer(BlockingCollection<int> q, CancellationTokenSource e)
        {
            _queue = q;
            _endTokenSource = e;
            _count = 0;
        }
        // Consumer.ThreadRun
        public void ThreadRun()
        {
            //int count = 0;
            while (!_endTokenSource.IsCancellationRequested)//WaitHandle.WaitAny(_endTokenSource.EventArray) != 1)
            {
                //lock (((ICollection)_queue).SyncRoot) //BlockingCollection locks for you!
                //{
                _count++;
                int item = _queue.Take();
            }
            Console.WriteLine("Consumer Thread: consumed {0} items", _count);
        }
        private BlockingCollection<int> _queue;
        private CancellationTokenSource _endTokenSource;
        private int _count;
    }
}
