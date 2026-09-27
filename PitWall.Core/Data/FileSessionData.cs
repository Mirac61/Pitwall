using System.Text.Json;
using PitWall.Core.Models;

namespace PitWall.Core.Data;

public class FileSessionData(string dataDir) : ISessionData
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public Task<List<Driver>> GetDriversAsync(int sessionKey, CancellationToken ct = default)
        => LoadAsync<Driver>(sessionKey, "drivers.json", ct);

    public Task<List<PositionUpdate>> GetPositionUpdatesAsync(int sessionKey, CancellationToken ct = default)
        => LoadAsync<PositionUpdate>(sessionKey, "position.json", ct);

    private async Task<List<T>> LoadAsync<T>(int sessionKey, string fileName, CancellationToken ct = default)
    {
        var path = Path.Combine(dataDir, sessionKey.ToString(), fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Keine Daten für Session {sessionKey}", path);

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions, ct) ?? [];
    }
}
