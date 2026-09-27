namespace PitWall.Core.Replay;

public static class ReplayTiming
{
    public static TimeSpan OffsetFromStart(DateTimeOffset first, DateTimeOffset eventDate, double speed)
    {
        if (speed <= 0)
            throw new ArgumentOutOfRangeException(nameof(speed), "Geschwindigkeit muss größer 0 sein");

        return (eventDate - first) / speed;
    }
}
