using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.ServiceOrders;
using PBS.ConnectHub.Library;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Server.Integrations;
using PBS.DSS.WebServices.Server.Extensions;

namespace PBS.DSS.WebServices.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceOrderController : Controller
    {
        [HttpPost]
        [Route("FetchServiceOrder")]
        public async Task<ServiceOrder> FetchServiceOrder(ServiceOrderFetchArgs args)
        {
            var so = new ServiceOrder();

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveServiceOrder(x, so)))
            {
                var soReq = new ConnectModels.FetchServiceOrderRequest() { WorkItemRef = args.ServiceOrderRef };

                await cl.StartConnection();
                await cl.SendToServer(soReq);
            }

            return so;
        }

        #region Connect Message Handlers
        private static void ReceiveServiceOrder(MessageHeaderV2 msgHeader, ServiceOrder so)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(ConnectModels.FetchServiceOrderResponse)))
                TranscribeServiceOrder(so, msgHeader.RecieveMessage<ConnectModels.ServiceOrder>());
        }

        private static void TranscribeServiceOrder(ServiceOrder so, ConnectModels.ServiceOrder connectSO)
        {
            if (connectSO == null) return;

            so.Id = connectSO.WorkItemRef;
            so.VehicleRef = connectSO.VehicleRef;
            so.ContactRef = connectSO.ContactRef;
            so.SONumber = connectSO.ServiceOrderNumber;

            foreach (var connectReq in connectSO.Requests)
            {
                var req = new RequestLine();

                req.RequestRef = connectReq.RequestRef;
                req.OpCodeRef = connectReq.OpCodeRef;
                req.OpCode = connectReq.OpCode;
                req.Description = connectReq.RequestDescription;
                req.EstimatedLabour = connectReq.EstimatedLabour;
                req.EstimatedParts = connectReq.EstimatedParts;

                switch (connectReq.RequestLineStatus)
                {
                    case ConnectModels.RequestLineStatuses.APPROVED:
                        req.AWRStatus = AWRStatuses.Approved;
                        break;
                    case ConnectModels.RequestLineStatuses.PENDING:
                        req.AWRStatus = AWRStatuses.Pending;
                        break;
                    case ConnectModels.RequestLineStatuses.DEFERRED:
                        req.AWRStatus = AWRStatuses.Deferred;
                        break;
                    case ConnectModels.RequestLineStatuses.DECLINED:
                        req.AWRStatus = AWRStatuses.Declined;
                        break;
                }

                switch (connectReq.Severity)
                {
                    case RecommendedServiceSeverity.Low:
                        req.Priority = RecommendedPriority.Low;
                        break;
                    case RecommendedServiceSeverity.Medium:
                        req.Priority = RecommendedPriority.Medium;
                        break;
                    case RecommendedServiceSeverity.High:
                    case RecommendedServiceSeverity.Critical:
                        req.Priority = RecommendedPriority.High;
                        break;
                }

                so.Requests.Add(req);
            }
        }
        #endregion

        #region Args
        public class ServiceOrderFetchArgs
        {
            public Guid ServiceOrderRef { get; set; } = Guid.Empty;
            public string SerialNumber { get; set; } = string.Empty;
        }
        #endregion

    }
}
