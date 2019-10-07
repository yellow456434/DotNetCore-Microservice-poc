using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTest
{
    class Program
    {
        //private static Object _lock = new Object();
        
        private static BlockingCollection<Tuple<int, AutoResetEvent>> waitDict = new BlockingCollection<Tuple<int, AutoResetEvent>>();
        private static Dictionary<int, AutoResetEvent> waitDictErr = new Dictionary<int, AutoResetEvent>();
        private static ConcurrentDictionary<int, AutoResetEvent> waitDict2 = new ConcurrentDictionary<int, AutoResetEvent>();
        static void Main(string[] args)
        {

            var logFile = System.IO.File.Create("/Users/willymbp/Desktop/aaa.txt");
            var logWriter = new System.IO.StreamWriter(logFile);
            logWriter.WriteLine("test");
            logWriter.Dispose();

            //for (int i = 1; i < 10; i++)
            //{
            //    var idx = i;
            //    Task.Run(() =>
            //    {
            //        ThreadProc(idx);
            //    });
            //}

            //for (int i = 1; i < 10; i++)
            //{
            //    Console.WriteLine("Press Enter to release a thread.");
            //    Console.ReadLine();
            //    //waitDictErr[i].Set();
            //    waitDict2[i].Set();
            //    //var wait = waitDict.Where(x => x.Item1 == i).First();
            //    // wait.Item2.Set();
            //}

            #region task thread test
            //try
            //{
            //    var c = new List<Task>();

            //    for (var i = 1; i < 3; i++)
            //    {
            //        var aa = i;
            //        Console.WriteLine("ggggg:" + aa);

            //        var a = Task.Run(() =>
            //        {
            //            Producer(aa);
            //            Consumer();
            //        });

            //        c.Add(a);
            //    }

            //    Task.WaitAll(c.ToArray());
            //}
            //catch (Exception ex)
            //{

            //}
            #endregion
        }
        private static BlockingCollection<int> data = new BlockingCollection<int>(); //共享資料

        private static void ThreadProc(int idx)
        {
            AutoResetEvent wait = new AutoResetEvent(false);

            // waitDict.Add(new Tuple<int, AutoResetEvent>(idx, wait));
            //waitDictErr.Add(idx, wait);
            waitDict2.GetOrAdd(idx, wait);

            Console.WriteLine("waits on AutoResetEvent #2.");
            wait.WaitOne();
            Console.WriteLine("released from AutoResetEvent #2.");

            Console.WriteLine("ends.");
        }

        private static void Producer(int i) //生產者
        {

            for (int ctr = 1; ctr < 3; ctr++)
            {
                Thread.Sleep(100);
                //lock (_lock)
                //{
                Console.WriteLine("ccccc:" + i * ctr);
                data.Add(i * ctr);
                //}
            }

        }
        private static void Consumer() //消費者
        {
            //lock (_lock)
            //{
            foreach (var item in data)
            {
                Console.WriteLine(item);
                Thread.Sleep(100);
            }
            //}

        }
    }
}
