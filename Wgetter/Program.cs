using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Wgetter
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            // If only one url provided, duplicate it
            var urls = args.ToList().GoesToEleven();

            // If this next line is commented out, then the Task based 
            // example will take longer than the sync one.  This is because the thread
            // pool needs to be built up/populated with worker threads.
            // Test2 will run slowly, but once it HAS run, we've got a nicely
            // filled thread pool, and so the Task based example runs faster than
            // the sync one (yay!) but not as fast as the async/await one.
            // This is because of the overhead in thread switching etc.
            // Remember: for I/O bound tasks (like this!) use async/await, for
            // CPU bound tasks, use Task.Run().

            //Time("Test2", () => Test2(urls));

            // Synchronously			
            Time("Synchronous", () =>
            {
                var result = urls
                    .Select(x => GetUrlSynch(x))
                    .Sum();

                return result;
            });

            // Using Task
            Time("Using old-skool tasks", () =>
            {
                var tasks = urls
                    .Select(x => Task.Run(() => GetUrlSynch(x)))
                    .ToArray();

                Task.WaitAll(tasks);

                return tasks.Sum(x => x.Result);
            });

            // Async/await
            Time("Async/Await", () =>
            {
                // Not using LINQ, as want to show calling async methods and
                // then waiting on the results.
                var tasks = new List<Task<int>>();

                foreach (var url in urls)
                {
                    var task = GetUrlAsync(url);
                    tasks.Add(task);
                }

                Task.WhenAll(tasks.ToArray());

                return tasks.Sum(x => x.Result);
            });
        }

        private static void Time(string what, Func<int> act)
        {
            var timer = new Stopwatch();

            timer.Start();

            var result = act();

            Console.WriteLine("Total elapsed milliseconds for {0}: {1}.  Result: {2}",
                what, timer.ElapsedMilliseconds, result);
        }

        private static int GetUrlSynch(string url)
        {
            using (var client = new WebClient())
            {
                var fred = client.DownloadString(url);

                return fred.Length;
            }
        }

        private static async Task<int> GetUrlAsync(string url)
        {
            using (var client = new WebClient())
            {
                var fred = client.DownloadStringTaskAsync(url);

                await fred;

                return fred.Result == null ? 0 : fred.Result.Length;
            }
        }

        private static int Test1(List<string> urls)
        {
            var result = 0;

            foreach (var url in urls)
            {
                result += GetUrlSynch(url);
            }

            return result;
        }

        private static int Test2(List<string> urls)
        {
            var tasks = new List<Task<int>>();

            foreach (var url in urls)
            {
                var task = Task.Run(() => GetUrlSynch(url));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return tasks.Sum(x => x.Result);
        }
    }
}