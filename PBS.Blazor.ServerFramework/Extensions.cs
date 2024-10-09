using PBS.ConnectHub.Library;

namespace PBS.Blazor.ServerFramework
{
    public static partial class Extensions
    {
        public static string ParsedMessageType(this MessageHeaderV2 msg)
        {
            return msg.MessageType.Split(",").First().Split(".").LastOrDefault() ?? string.Empty;
        }

        public static string ParsedMessageType(this Type type)
        {
            return type.Name.Split(".").LastOrDefault() ?? string.Empty;
        }

        public static bool IsConnectResponseMatch(this MessageHeaderV2 msg, Type type)
        {
            return msg.ParsedMessageType() == type.ParsedMessageType();
        }
    }
}
