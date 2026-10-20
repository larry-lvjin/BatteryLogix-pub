using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using BatteryLogix.Models;

namespace BatteryLogix.Services;
public static partial class LogParser
{
    [GeneratedRegex(@"(?<time>\d{2}-\d{2}#\d{2}:\d{2}:\d{2})(?:\.\d+)?[\s\S]*?bat\s+info\s+voltage\s*=\s*(?<voltage>-?\d+)\s*,\s*percent\s*=\s*(?<percent>-?\d+)\s*,\s*g_bat_info\s*=\s*(?<battery>-?\d+)\s*,\s*current\s*=\s*(?<current>-?\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex MainLineRegex();
    [GeneratedRegex(@"temp_info\s*=\s*(?<temperature>-?\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex TemperatureRegex();

    public static IEnumerable<BatteryRecord> Parse(string path)
    {
        // 实现细节未公开（算法核心）
        throw new NotImplementedException("该方法的实现不在开源范围内");
    }
    private static int Value(Match match, string name) => int.Parse(match.Groups[name].Value, CultureInfo.InvariantCulture);
}
