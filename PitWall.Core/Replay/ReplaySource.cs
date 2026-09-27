using System.Runtime.CompilerServices;
using PitWall.Core.Models;

namespace PitWall.Core.Replay;

public class ReplaySource(IEnumerable<PositionUpdate> updates, double speed, TimeProvider time) : IRaceEventSource
{
    public async IAsyncEnumerable<PositionUpdate> ReadAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        var ordered = updates.OrderBy(u => u.Date).ToList();
        if (ordered.Count == 0)
            yield break;

        var first = ordered[0].Date;
        var startedAt = time.GetUtcNow();

        foreach (var update in ordered)
        {
            var target = startedAt + ReplayTiming.OffsetFromStart(first, update.Date, speed);
            var wait = target - time.GetUtcNow();

            if (wait > TimeSpan.Zero)
                await Task.Delay(wait, time, ct);

            yield return update;
        }
    }
}
