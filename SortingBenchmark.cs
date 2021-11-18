using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SortingBenchmarkUI
{
    class SortingBenchmark
    {
        Mutex _mutex = new();

        public delegate void DSorter(int[] arr, Comparison<int> compare);

        public event EventHandler<NewResultEventArgs> OnNewResult;

        public Dictionary<string, DSorter> Sorters { get; set; }

        public void Run(IEnumerable<int> arrSizes, int range)
        {
            foreach (var sorter in Sorters)
                foreach (int size in arrSizes)
                    ThreadPool.QueueUserWorkItem(
                        BenchmarkThreadWorker,
                        new WorkerArgs
                        {
                            SorterName = sorter.Key,
                            Sorter = sorter.Value,
                            ArrSize = size,
                            ArrRange = range
                        },
                        false);
        }

        void BenchmarkThreadWorker(WorkerArgs args)
        {
            Stopwatch watch = new();
            watch.Start();
            args.Sorter(GetRandomSequence(args.ArrSize, args.ArrRange), (a, b) => a - b);
            watch.Stop();

            _mutex.WaitOne();
            OnNewResult?.Invoke(this, new NewResultEventArgs
            {
                Sorter = args.SorterName,
                ArrSize = args.ArrSize.ToString(),
                ElapsedMilliseconds = watch.ElapsedMilliseconds.ToString()
            });
            _mutex.ReleaseMutex();
        }

        int[] GetRandomSequence(int size, int range)
        {
            Random rd = new();
            int[] res = new int[size];
            for (int i = 0; i < size; ++i)
                res[i] = rd.Next(range);
            return res;
        }

        struct WorkerArgs
        {
            public string SorterName;
            public DSorter Sorter;
            public int ArrSize;
            public int ArrRange;
        }

        public class NewResultEventArgs : EventArgs
        {
            public string Sorter;
            public string ArrSize;
            public string ElapsedMilliseconds;
        }
    }
}
