using PitWall.Core.Models;

namespace PitWall.Core.Data;

public interface ISessionData
{
    Task<List<Driver>> GetDriversAsync(int sessionKey, CancellationToken ct = default);
    Task<List<PositionUpdate>> GetPositionUpdatesAsync(int sessionKey, CancellationToken ct = default);
}
