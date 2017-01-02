using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WgetterNot
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			// Synchronously
			Time("Synchronous", () => TotalUrlsSynch(args.ToList()));

			// Using Task
			Time("Using old-skool tasks", () => TotalUrlsWithTask(args.ToList()));

			// Async/await
			Time("Async/Await", () =>
			{
				// Note: cannot 'await' on an async method from a console app's main method
				// (as that cannot be flagged as async).
				// Solution is to instead get the Task<>'s, and then wait on them.
				var result = TotalUrlsAsync(args.ToList());
				result.Wait();
			});
		}

		private static void Time(string what, Action act)
		{
			var timer = new Stopwatch();

			timer.Start();
			act();

			Console.WriteLine("Total elapsed milliseconds for {0}: {1}", what, timer.ElapsedMilliseconds);
		}

		private static int TotalUrlsSynch(List<string> urls)
		{
			int result = 0;

			foreach (var url in urls)
			{
				result += GetUrlSynch(url);
			}

			return result;
		}

		private static async Task<int> TotalUrlsAsync(List<string> urls)
		{
			// Could also do in a foreach loop, but then have to manage collection of tasks.
			// LINQ is possiibly the best C# feature ever, and another example of Greenspan's 
			// tenth law in action......
			var tasks = urls.Select(x => GetUrlAsync(x)).ToArray();
			var results = await Task.WhenAll(tasks);

			return results.Sum();
		}

		private static int TotalUrlsWithTask(List<string> urls)
		{
			var tasks = new List<Task<int>>();

			foreach (var url in urls)
			{
				var t = Task.Run(() =>
				{
					var len = GetUrlSynch(url);
					return len;
				});

				tasks.Add(t);
			}

			Task.WaitAll(tasks.ToArray());

			return tasks.Sum(x => x.Result);
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
	}
}


