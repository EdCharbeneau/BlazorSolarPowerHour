using System.Text;

namespace BlazorSolarPowerHour.Models;

public static class DataUtilities
{
    public static string GetTopicValue(this ArraySegment<byte> bytes) => Encoding.ASCII.GetString(bytes.ToArray());
}