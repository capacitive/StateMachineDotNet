using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ConsumerProducer
{
    public class Producer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="q">simple random integer BlockingCollection for producer threads</param>
        /// <param name="e"></param>
        public Producer(BlockingCollection<int> q, CancellationTokenSource e)
        {
            _queue = q;
            _endTokenSource = e;
            _count = 0;
        }
        // Producer.ThreadRun
        public void ThreadRun()
        {
            Random r = new Random();
            //spins a thread and puts random numbers in the sync BlockingCollection:
            while (!_endTokenSource.IsCancellationRequested)//ExitThreadEvent.WaitOne(0, false))
            {
                //lock (((ICollection)_queue).SyncRoot)
                //{
                while (_queue.Count < 20)
                {
                    _count++;
                    _queue.Add(r.Next(0, 100));
                    //_endTokenSource.NewItemEvent.Set();                    
                }
            }
            Console.WriteLine("Producer thread: produced {0} items", _count);
        }
        private BlockingCollection<int> _queue;
        private CancellationTokenSource _endTokenSource;
        private int _count;
    }
}
