using PitWall.Core.Models;

namespace PitWall.Core;

public interface IRaceEventSource
{
    IAsyncEnumerable<PositionUpdate> ReadAsync(CancellationToken ct = default);
}
