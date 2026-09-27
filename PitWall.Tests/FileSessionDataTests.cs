using PitWall.Core.Data;

namespace PitWall.Tests;

public class FileSessionDataTests
{
    private readonly FileSessionData _data =
        new(Path.Combine(AppContext.BaseDirectory, "TestData"));

    [Fact]
    public async Task LoadsAllDrivers()
    {
        var drivers = await _data.GetDriversAsync(9523);
        Assert.Equal(20, drivers.Count);
    }

    [Fact]
    public async Task MapsSnakeCaseFields()
    {
        var drivers = await _data.GetDriversAsync(9523);
        var lec = drivers.Single(d => d.DriverNumber == 16);

        Assert.Equal("LEC", lec.NameAcronym);
        Assert.Equal("Ferrari", lec.TeamName);
        Assert.False(string.IsNullOrEmpty(lec.TeamColour));
    }

    [Fact]
    public async Task LoadsAllPositionUpdates()
    {
        var positions = await _data.GetPositionUpdatesAsync(9523);
        Assert.Equal(127, positions.Count);
    }

    [Fact]
    public async Task ThrowsForUnknownSession()
    {
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _data.GetDriversAsync(1));
    }
}
