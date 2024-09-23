using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.ServiceOrders;
using PBS.ConnectHub.Library;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.WebServices.Server.Integrations;
using PBS.DSS.WebServices.Server.Extensions;
using PBS.ConnectHub.Library.Messages.DigitalServiceSuite;

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
            var hasCompleted = false;

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveServiceOrderResponse(x, so, out hasCompleted)))
            {
                await cl.SendToServer(new ServiceOrderDSSRequest() { ServiceOrderRef = args.ServiceOrderRef });

                while (!hasCompleted) Thread.Sleep(500);
            }

            return so;
        }

        #region Connect Message Handlers
        private static void ReceiveServiceOrderResponse(MessageHeaderV2 msgHeader, ServiceOrder so, out bool hasCompleted)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(ServiceOrderDSSResponse)))
                TranscribeServiceOrder(so, msgHeader.RecieveMessage<ServiceOrderDSSResponse>());

            hasCompleted = true;
        }

        private static void TranscribeServiceOrder(ServiceOrder so, ServiceOrderDSSResponse resp)
        {
            so.ShopBanner = resp.ShopBanner;

            TranscribeServiceOrder(so, resp);
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
                req.EstimatedLabour = (double)connectReq.EstimatedLabour;
                req.EstimatedParts = (double)connectReq.EstimatedParts;

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
    }
}
