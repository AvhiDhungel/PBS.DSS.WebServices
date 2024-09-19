using PBS.ConnectHub.Library;
using CHubClient = PBS.ConnectHubV2.Client.Client;

namespace PBS.DSS.WebServices.Server.Integrations
{
    public class ConnectHubIntegration
    {
        private const string _ConnectHubURLTest = "https://pbsmessagehubtest.azurewebsites.net";
        private const string _ConnectHubURL = "https://pbsmessagehub.azurewebsites.net";

        private static readonly Guid MessageId = Guid.NewGuid();

        public static async Task<CHubClient> GetConnectHubClient(string serialNumber, Action<MessageHeaderV2> handler)
        {
            return await CHubClient.Fetch(_ConnectHubURL, handler, MessageId, serialNumber, false);
        }

        public static Guid GetMessageId() { return MessageId; }
    }
}
