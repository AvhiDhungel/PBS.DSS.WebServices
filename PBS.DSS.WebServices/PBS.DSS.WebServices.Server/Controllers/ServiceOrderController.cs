using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.ServiceOrders;
using PBS.ConnectHub.Library;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.WebServices.Server.Utilities;
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
        public async Task<ActionResult<ServiceOrder>> FetchServiceOrder(ServiceOrderFetchArgs args)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(new ServiceOrder());

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveServiceOrderResponse(x, msg)))
            {
                await cl.SendToServer(new ServiceOrderDSSRequest() { ServiceOrderRef = args.ServiceOrderRef });

                while (!msg.HasCompleted) Thread.Sleep(500);
            }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("CalculateApprovedAWR")]
        public async Task<ActionResult<ServiceOrder>> CalculateApprovedAWR(ServiceOrder so, string serial)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(so);

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveCalculateApprovedAWRResponse(x, msg)))
            {
                var reqRefs = so.RequestsMarkedForApproval.Select(x => x.RequestRef).ToList();
                await cl.SendToServer(new CalculateApprovedAWRRequest() { ServiceOrderRef = so.Id, ApprovedRequestRefs = reqRefs });

                while (!msg.HasCompleted) Thread.Sleep(500);
            }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("ApproveAWR")]
        public async Task<ActionResult<ServiceOrder>> ApproveAWR(ServiceOrder so, string serial)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(so);

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveApprovedAWRResponse(x, msg)))
            {
                var reqRefs = so.RequestsMarkedForApproval.Select(x => x.RequestRef).ToList();
                await cl.SendToServer(new ServiceOrderApproveAWRRequest() { ServiceOrderRef = so.Id, ApprovedRequestRefs = reqRefs });

                while (!msg.HasCompleted) Thread.Sleep(500);
            }

            return msg.GetResult();
        }

        #region Connect Message Handlers
        private static void ReceiveServiceOrderResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(ServiceOrderDSSResponse)))
                TranscribeServiceOrder(msgHeader.RecieveMessage<ServiceOrderDSSResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveCalculateApprovedAWRResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(CalculateApprovedAWRResponse)))
                FillCalculatedAWRResponse(msgHeader.RecieveMessage<CalculateApprovedAWRResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveApprovedAWRResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(ServiceOrderApproveAWRResponse)))
                VerifyAWRResponse(msgHeader.RecieveMessage<ServiceOrderApproveAWRResponse>(), msg);

            msg.HasCompleted = true;
        }
        #endregion

        #region Transcribe Service Order
        private static void TranscribeServiceOrder(ServiceOrderDSSResponse resp, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.ShopBanner = resp.ShopBanner;
            TranscribeServiceOrder(msg.Object, resp.ServiceOrder);
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

        #region Fill Calculated AWR Response
        private static void FillCalculatedAWRResponse(CalculateApprovedAWRResponse resp, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.SubTotal = (double)resp.Subtotal;
            msg.Object.TaxTotal = (double)resp.Taxes;
            msg.Object.FeesTotal = (double)resp.Fees;
            msg.Object.GrandTotal = (double)resp.GrandTotal;
        }
        #endregion

        #region "AWR Approved Response"
        private static void VerifyAWRResponse(ServiceOrderApproveAWRResponse resp, ConnectReceiveMessage<ServiceOrder> msg)
        {
            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.RequestsMarkedForApproval.ToList().ForEach(req => req.AWRStatus = AWRStatuses.Approved);
        }
        #endregion
    }
}
