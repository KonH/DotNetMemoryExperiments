# DotNetMemoryExperiments

## BulkAllocator

Sample project to investigate memory usage in extenral instruments (allocates byte arrays and calls GC.Collect):

```
BulkAllocator.exe {bytesToAllocate} {arrayCountPerIteration} {iterationCount} {iterationGapSec} {idleTimeSec}
```
