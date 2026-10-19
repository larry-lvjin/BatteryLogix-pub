namespace BatteryLogix.Models;
public sealed record BatteryRecord(string Time, int Voltage, int Percent, int BatteryInfo, int Current, int? Temperature);
