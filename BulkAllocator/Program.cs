using System;
using System.Collections.Generic;
using System.Threading;

namespace BulkAllocator {
	class Program {
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

			while ( true ) {
				Arrays = new LinkedList<byte[]>();
				for ( var i = 0; i < iterationCount; i++ ) {
					Console.WriteLine($"Iteration {i}/{iterationCount - 1}: allocate {bytesToAllocate}x{arrayCountPerIteration} bytes");
					for ( var j = 0; j < arrayCountPerIteration; j++ ) {
						Arrays.AddLast(new byte[bytesToAllocate]);
					}
					Console.WriteLine($"Iteration: Waiting for ~{iterationGap.TotalSeconds} sec");
					Thread.Sleep(iterationGap);
				}
				Arrays = null;
				Console.WriteLine("GC.Collect()");
				GC.Collect();
				Console.WriteLine($"Idle: Waiting for ~{idleTime.TotalSeconds} sec");
				Thread.Sleep(idleTime);
			}
		}
	}
}
