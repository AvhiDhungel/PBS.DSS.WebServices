using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.ServiceOrders;
using PBS.ConnectHub.Library;
using PBS.ConnectHub.Library.Messages.DigitalServiceSuite;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Server.Utilities;
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
        public async Task<ActionResult<ServiceOrder>> FetchServiceOrder(ServiceOrderFetchArgs args)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(new ServiceOrder());

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveServiceOrderResponse(x, msg)))
            {
                await cl.SendToServer(new ServiceOrderDSSRequest() { ServiceOrderRef = args.ServiceOrderRef });

                msg.WaitForCompletion();
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

                msg.WaitForCompletion();
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

                msg.WaitForCompletion();
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

            ConnectModelHelper.TranscribeServiceOrder(msg.Object, resp.ServiceOrder);
            ConnectModelHelper.TranscribeContact(msg.Object.ContactInfo, resp.Contact);
            ConnectModelHelper.TranscribeVehicle(msg.Object.Vehicle, resp.Vehicle);
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
