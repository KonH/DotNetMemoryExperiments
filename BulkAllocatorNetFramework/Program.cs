using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BulkAllocatorNetFramework {
	class Program {
		const int PageSize = 4048;

		static LinkedList<byte[]> Arrays = null;

		static void Main(string[] args) {
			if ( args.Length != 5 ) {
				Console.WriteLine("Usage: ");
				Console.WriteLine("{bytesToAllocate} {arrayCountPerIteration} {iterationCount} {iterationGapSec} {idleTimeSec}");
				return;
			}
			var bytesToAllocate = int.Parse(args[0]);
			var arrayCountPerIteration = int.Parse(args[1]);
			var iterationCount = int.Parse(args[2]);
			var iterationGap = TimeSpan.FromSeconds(int.Parse(args[3]));
			var idleTime = TimeSpan.FromSeconds(int.Parse(args[4]));
			var process = Process.GetCurrentProcess();
			while ( true ) {
				process.Refresh();
				var workingSetAtStart = process.WorkingSet64;
				var privateMemorySizeAtStart = process.PrivateMemorySize64;
				Arrays = new LinkedList<byte[]>();
				for ( var i = 0; i < iterationCount; i++ ) {
					Console.WriteLine($"Allocate Iteration {i}/{iterationCount - 1}: allocate {bytesToAllocate}x{arrayCountPerIteration} bytes");
					for ( var j = 0; j < arrayCountPerIteration; j++ ) {
						Arrays.AddLast(new byte[bytesToAllocate]);
					}
					Console.WriteLine($"Allocate Iteration: Waiting for ~{iterationGap.TotalSeconds} sec");
					Thread.Sleep(iterationGap);
				}
				ShowMemoryDiff(process, workingSetAtStart, privateMemorySizeAtStart);
				for ( var i = 0; i < iterationCount; i++ ) {
					Console.WriteLine($"Use Iteration {i}/{iterationCount - 1}: access allocated bytes");
					foreach ( var array in Arrays ) {
						for ( var j = 0; j < bytesToAllocate; j += PageSize ) {
							array[j] = 1;
						}
					}
					Console.WriteLine($"Use Iteration: Waiting for ~{iterationGap.TotalSeconds} sec");
					Thread.Sleep(iterationGap);
				}
				ShowMemoryDiff(process, workingSetAtStart, privateMemorySizeAtStart);
				Arrays = null;
				Console.WriteLine("GC.Collect()");
				GC.Collect();
				Console.WriteLine($"Idle: Waiting for ~{idleTime.TotalSeconds} sec");
				ShowMemoryDiff(process, workingSetAtStart, privateMemorySizeAtStart);
				Thread.Sleep(idleTime);
				Console.WriteLine();
			}
		}

		static void ShowMemoryDiff(Process process, long workingSetAtStart, long privateMemorySizeAtStart) {
			process.Refresh();
			var workingSetAfter = process.WorkingSet64;
			var privateMemorySizeAfter = process.PrivateMemorySize64;
			var workingSetDiffAfter = (workingSetAfter - workingSetAtStart);
			var privateMemorySizeDiffAfter = (privateMemorySizeAfter - privateMemorySizeAtStart);
			Console.WriteLine($"WorkingSetDiffAfter: {workingSetDiffAfter}, PrivateMemorySizeDiffAfter: {privateMemorySizeDiffAfter}");
		}
	}
}
