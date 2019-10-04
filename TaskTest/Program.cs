using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTest
{
    class Program
    {
        //private static Object _lock = new Object();

        private static AutoResetEvent wait = new AutoResetEvent(true);

        static void Main(string[] args)
        {
             
            for (int i = 1; i < 4; i++)
            {
                Task.Run(() =>
                {
                    ThreadProc();
                });
            }

            for (int i = 1; i < 4; i++)
            {
                Console.WriteLine("Press Enter to release a thread.");
                Console.ReadLine();
                wait.Set();
                
            } 

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

        private static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine("{0} waits on AutoResetEvent #2.", name);
            wait.WaitOne();
            Console.WriteLine("{0} is released from AutoResetEvent #2.", name);

            Console.WriteLine("{0} ends.", name);
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
