using PitWall.Core.Models;

namespace PitWall.Api;

public class LiveRace
{
    private volatile IReadOnlyList<Standing> _standings = [];
    public IReadOnlyList<Standing> Current => _standings;
    public void Update(IReadOnlyList<Standing> standings) => _standings = standings;
}
