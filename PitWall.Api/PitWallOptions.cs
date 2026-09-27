namespace PitWall.Api;

public class PitWallOptions
{
    public string DataDir { get; set; } = "../data";
    public int SessionKey { get; set; }
    public double Speed { get; set; } = 1;
}
