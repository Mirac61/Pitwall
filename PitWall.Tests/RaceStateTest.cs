using PitWall.Core.Data;
using PitWall.Core.Models;
using PitWall.Core.State;

namespace PitWall.Tests;

public class RaceStateTests
{
    [Fact]
    public void LaterUpdateOverwritesEarlier()
    {
        var lec = new Driver(16, "LEC", "Ferrari", "E8002D");
        var state = new RaceState([lec]);
        var t = DateTimeOffset.Parse("2024-05-26T13:00:00Z");

        state.Apply(new PositionUpdate(t, 16, 2));
        state.Apply(new PositionUpdate(t.AddSeconds(5), 16, 1));

        Assert.Equal(1, state.GetStandings().Single().Position);
    }

    [Fact]
    public async Task LeclercWinsMonaco2024()
    {
        var data = new FileSessionData(Path.Combine(AppContext.BaseDirectory, "TestData"));
        var state = new RaceState(await data.GetDriversAsync(9523));

        var updates = await data.GetPositionUpdatesAsync(9523);
        foreach (var u in updates.OrderBy(u => u.Date))
            state.Apply(u);

        var top3 = state.GetStandings().Take(3).Select(s => s.Driver.NameAcronym);
        Assert.Equal(new[] { "LEC", "PIA", "SAI" }, top3);
    }
}
