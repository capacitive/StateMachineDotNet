using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConsumerProducer
{
    public class ThreadSyncSample
    {
        private static void ShowBlockingCollectionContents(BlockingCollection<int> q)
        {
            //lock (((ICollection)q).SyncRoot)//need a lock on the q to get exactly what the workerthread is doing at that moment
            //{
            foreach (int item in q)
            {
                Console.Write("{0} ", item);
            }
            Console.WriteLine();
        }

        static void Main()
        {
            BlockingCollection<int> BlockingCollection = new BlockingCollection<int>();//set BlockingCollection up
            CancellationTokenSource endTokenSource = new CancellationTokenSource();
            //SyncEvents syncEvents = new SyncEvents();//inst. syncevents

            //old school:
            //Console.WriteLine("Configuring worker threads...");
            //Producer producer1 = new Producer(BlockingCollection, endTokenSource);
            //Producer producer2 = new Producer(BlockingCollection, endTokenSource);
            //Consumer consumer = new Consumer(BlockingCollection, endTokenSource);
            //Thread producerThread1 = new Thread(producer1.ThreadRun);
            //Thread producerThread2 = new Thread(producer2.ThreadRun);
            //Thread consumerThread = new Thread(consumer.ThreadRun);

            //Console.WriteLine("Launching producer and consumer threads...");
            //producerThread1.Start();
            //producerThread2.Start();
            //consumerThread.Start();

            //state of the art:
            Console.WriteLine("Configuring worker threads...");
            Producer producer1 = new Producer(BlockingCollection, endTokenSource);
            Producer producer2 = new Producer(BlockingCollection, endTokenSource);
            Consumer consumer = new Consumer(BlockingCollection, endTokenSource);        
            //create 1 consumer, 2 producers:
            var tasks = new[]
            {
                new Task(producer1.ThreadRun),
                new Task(producer2.ThreadRun),
                new Task(consumer.ThreadRun)
            };
            Console.WriteLine("Starting tasks...");
            Array.ForEach(tasks, t => t.Start());

            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(2500);
                ShowBlockingCollectionContents(BlockingCollection);
            }

            Console.WriteLine("Signaling threads to terminate...");
            endTokenSource.Cancel();
            //syncEvents.ExitThreadEvent.Set();

            //join all threads to end gracefully:
            Task.WaitAll(tasks);
            //producerThread1.Join();
            //producerThread2.Join();
            //consumerThread.Join();

            Console.ReadLine();
        }

    }
}
