using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class MonteCarloMultiThreading : MonteCarlo
{
    public override int shots(int interval)
	{
		progress = 0;

		int hits = 0;

		var rangePartitioner = Partitioner.Create(0, interval);

		Parallel.ForEach(rangePartitioner, (range) =>
        {
			int localHits = 0;
            // Loop over each range element without a delegate invocation.
            for (int i = range.Item1; i < range.Item2; i++)
            {
				if (shoot()) localHits++;
            }
			Interlocked.Add(ref hits, localHits);
			Interlocked.Add(ref progress, range.Item2 - range.Item1);
        });

		return hits;
	}
}