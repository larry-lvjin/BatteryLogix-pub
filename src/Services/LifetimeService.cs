using System.IO;
namespace BatteryLogix.Services;
public static class LifetimeService
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(518400);
    public static bool IsValid()
    {
        try
        {
            var path = Environment.ProcessPath;
            if (path is null) return false;
            var age = DateTime.UtcNow - File.GetLastWriteTimeUtc(path);
            return age >= TimeSpan.Zero && age <= Lifetime;
        }
        catch { return false; }
    }
}
