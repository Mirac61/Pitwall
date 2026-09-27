using PitWall.Core.Data;
using PitWall.Core.Replay;

namespace PitWall.Tests;

public class ReplayTests
{
    private static readonly DateTimeOffset T0 = DateTimeOffset.Parse("2024-05-26T13:00:00Z");

    [Fact]
    public void OffsetAtNormalSpeed()
        => Assert.Equal(TimeSpan.FromSeconds(10),
            ReplayTiming.OffsetFromStart(T0, T0.AddSeconds(10), 1));

    [Fact]
    public void OffsetAtTenTimesSpeed()
        => Assert.Equal(TimeSpan.FromSeconds(1),
            ReplayTiming.OffsetFromStart(T0, T0.AddSeconds(10), 10));

    [Fact]
    public void RejectsZeroSpeed()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => ReplayTiming.OffsetFromStart(T0, T0, 0));

    [Fact]
    public async Task ReplaysAllUpdatesInOrder()
    {
        var data = new FileSessionData(Path.Combine(AppContext.BaseDirectory, "TestData"));
        var updates = await data.GetPositionUpdatesAsync(9523);

        // Sehr hoher Faktor: ganzes Rennen in Millisekunden
        var source = new ReplaySource(updates, 1_000_000, TimeProvider.System);

        var received = new List<DateTimeOffset>();
        await foreach (var u in source.ReadAsync())
            received.Add(u.Date);

        Assert.Equal(127, received.Count);
        Assert.Equal(received.OrderBy(d => d), received);
    }
}
