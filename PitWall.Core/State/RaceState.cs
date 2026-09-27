using PitWall.Core.Models;

namespace PitWall.Core.State;

public class RaceState(IEnumerable<Driver> drivers)
{
    private readonly Dictionary<int, Driver> _drivers =
        drivers.ToDictionary(d => d.DriverNumber);

    private readonly Dictionary<int, int> _positions = new();

    public DateTimeOffset? LastUpdate { get; private set; }

    public void Apply(PositionUpdate update)
    {
        _positions[update.DriverNumber] = update.Position;
        LastUpdate = update.Date;
    }

    public List<Standing> GetStandings() =>
        _positions
            .Where(p => _drivers.ContainsKey(p.Key))
            .OrderBy(p => p.Value)
            .Select(p => new Standing(p.Value, _drivers[p.Key]))
            .ToList();
}
