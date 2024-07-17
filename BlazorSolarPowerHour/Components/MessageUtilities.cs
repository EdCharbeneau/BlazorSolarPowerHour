using System.Text;

namespace BlazorSolarPowerHour.Components
{
    public static class MessageUtilities
    {
        public static string GetTopicValue(this ArraySegment<byte> bytes) => Encoding.ASCII.GetString(bytes.ToArray());
    }
}
