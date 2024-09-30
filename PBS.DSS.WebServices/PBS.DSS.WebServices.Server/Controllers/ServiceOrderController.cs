using Microsoft.AspNetCore.Mvc;
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
            var msg = new ConnectReceiveMessage<ServiceOrder>(new ServiceOrder(), args.SerialNumber, "FetchServiceOrder");

            msg.LogMessage($"Attempting to Fetch Service Order for Serial {args.SerialNumber} with the following arguments:");
            msg.LogSerialized(args);
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveServiceOrderResponse(x, msg, args.SerialNumber));
                await cl.SendToServer(new ServiceOrderDSSRequest() { ServiceOrderRef = args.ServiceOrderRef });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("CalculateApprovedAWR")]
        public async Task<ActionResult<ServiceOrder>> CalculateApprovedAWR(ServiceOrder so, string serial)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(so, serial, "CalculateApprovedAWR");

            msg.LogMessage($"Attempting to Calculate Approved AWR totals for Serial {serial} for SO# {so.SONumber} ID: {so.Id}");
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveCalculateApprovedAWRResponse(x, msg));
                var reqRefs = so.RequestsMarkedForApproval.Select(x => x.RequestRef).ToList();
                await cl.SendToServer(new CalculateApprovedAWRRequest() { ServiceOrderRef = so.Id, ApprovedRequestRefs = reqRefs });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("ApproveAWR")]
        public async Task<ActionResult<ServiceOrder>> ApproveAWR(ServiceOrder so, string serial)
        {
            var msg = new ConnectReceiveMessage<ServiceOrder>(so, serial, "ApproveAWR");

            msg.LogMessage($"Attempting to Approve AWR for Serial {serial} for SO# {so.SONumber} ID: {so.Id}");
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveApprovedAWRResponse(x, msg));
                var reqRefs = so.RequestsMarkedForApproval.Select(x => x.RequestRef).ToList();
                await cl.SendToServer(new ServiceOrderApproveAWRRequest() { ServiceOrderRef = so.Id, ApprovedRequestRefs = reqRefs });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        #region Connect Message Handlers
        private static void ReceiveServiceOrderResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg, string serial)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(ServiceOrderDSSResponse)))
                TranscribeServiceOrder(msgHeader.RecieveMessage<ServiceOrderDSSResponse>(), msg, serial);

            msg.HasCompleted = true;
        }

        private static void ReceiveCalculateApprovedAWRResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(CalculateApprovedAWRResponse)))
                FillCalculatedAWRResponse(msgHeader.RecieveMessage<CalculateApprovedAWRResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveApprovedAWRResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<ServiceOrder> msg)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(ServiceOrderApproveAWRResponse)))
                VerifyAWRResponse(msgHeader.RecieveMessage<ServiceOrderApproveAWRResponse>(), msg);

            msg.HasCompleted = true;
        }
        #endregion

        #region Transcribe Service Order
        private static void TranscribeServiceOrder(ServiceOrderDSSResponse resp, ConnectReceiveMessage<ServiceOrder> msg, string serial)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            ConnectModelHelper.TranscribeServiceOrder(msg.Object, resp.ServiceOrder);
            ConnectModelHelper.TranscribeSOTimeline(msg.Object, resp.WorkItemTimelineTypes);
            ConnectModelHelper.TranscribeContact(msg.Object.ContactInfo, resp.Contact);
            ConnectModelHelper.TranscribeVehicle(msg.Object.Vehicle, resp.Vehicle);

            msg.Object.ShopBanner = WebAppointmentsIntegration.GetShopBanner(serial, resp.ServiceOrder.ShopRef);
            msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
        }
        #endregion

        #region Fill Calculated AWR Response
        private static void FillCalculatedAWRResponse(CalculateApprovedAWRResponse resp, ConnectReceiveMessage<ServiceOrder> msg)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.SubTotal = (double)resp.Subtotal;
            msg.Object.TaxTotal = (double)resp.Taxes;
            msg.Object.FeesTotal = (double)resp.Fees;
            msg.Object.GrandTotal = (double)resp.GrandTotal;

            msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
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
