using PBS.ConnectHub.Library;
using CHubClient = PBS.ConnectHubV2.Client.Client;

namespace PBS.Blazor.ServerFramework.Integrations
{
    public class ConnectHubIntegration
    {
        private static string ConnectHub_DevURL = "https://pbsconnecthubv2-test1.azurewebsites.net";
        private static string ConnectHub_ProdURL = "https://connecthubv2.pbssystems.com";

        private static readonly Guid MessageId = Guid.NewGuid();

        public static async Task<CHubClient> GetConnectHubClient(string serialNumber, Action<MessageHeaderV2> handler)
        {
            return await CHubClient.Fetch(ConnectHub_DevURL, handler, MessageId, serialNumber, false);
        }

        public static Guid GetMessageId() { return MessageId; }
    }
}
