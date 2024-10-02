using Microsoft.AspNetCore.Mvc;
using PBS.ConnectHub.Library;
using PBS.ConnectHub.Library.Messages.DigitalServiceSuite;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Server.Integrations;
using PBS.DSS.WebServices.Server.Extensions;
using PBS.DSS.WebServices.Server.Utilities;
using PBS.ConnectHub.Library.Messages.Documents;

namespace PBS.DSS.WebServices.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        [HttpPost]
        [Route("FetchAppointment")]
        public async Task<ActionResult<Appointment>> FetchAppointment(AppointmentFetchArgs args)
        {
            var msg = new ConnectReceiveMessage<Appointment>(new Appointment(), args.SerialNumber, "FetchAppointment");

            msg.LogMessage($"Attempting to Fetch Appointment for Serial {args.SerialNumber} with the following arguments:");
            msg.LogSerialized(args);
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveAppointment(x, msg, args.SerialNumber));
                await cl.SendToServer(new AppointmentDSSRequest { AppointmentRef = args.AppointmentRef });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetObjectResult();
        }

        [HttpPost]
        [Route("CheckInAppointment")]
        public async Task<ActionResult> CheckInAppointment(Appointment appt)
        {
            var msg = new ConnectReceiveMessage<Appointment>(appt, appt.SerialNumber, "CheckInAppointment");

            msg.LogMessage($"Attempting to Check-In Appointment for Serial {appt.SerialNumber} for Appt# {appt.AppointmentNumber} ID: {appt.Id}");
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(appt.SerialNumber, (x) => ReceiveAppointmentCheckInResponse(x, msg));
                await cl.SendToServer(new AppointmentCheckInRequest { AppointmentRef = appt.Id, Odometer = appt.Odometer });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("CancelAppointment")]
        public async Task<ActionResult> CancelAppointment(Appointment appt)
        {
            var msg = new ConnectReceiveMessage<Appointment>(appt, appt.SerialNumber, "CancelAppointment");

            msg.LogMessage($"Attempting to Cancel Appointment for Serial {appt.SerialNumber} for Appt# {appt.AppointmentNumber} ID: {appt.Id}");
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(appt.SerialNumber, (x) => ReceiveAppointmentCancelResponse(x, msg));
                await cl.SendToServer(new AppointmentCancelRequest { AppointmentRef = appt.Id });

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("FetchAppointmentDocument")]
        public async Task<ActionResult<Attachment>> FetchAppointmentDocument(AppointmentDocumentFetchArgs args)
        {
            var att = new Attachment() { DocumentType = args.DocumentType, FileType = FileTypes.Document, Name = FileTypes.Document.ToString() };
            var msg = new ConnectReceiveMessage<Attachment>(att, args.SerialNumber, "FetchAppointmentDocument");

            if (args.DocumentType == Shared.Models.WorkItems.DocumentTypes.None) msg.GetObjectResult();

            msg.LogMessage($"Attempting to Fetch Appointment {args.DocumentType} with the following arguments:");
            msg.LogSerialized(args);
            msg.LogNewLine();

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveAppointmentDocumentResponse(x, msg, args.SerialNumber));
                var req = new DocumentRequest
                {
                    WorkItemId = args.AppointmentRef,
                    Department = Products.APPOINTMENT,
                    DesiredFormat = OutputFormats.PDF
                };

                switch (args.DocumentType)
                {
                    case Shared.Models.WorkItems.DocumentTypes.HardCopy:
                        req.DocumentType = ConnectHub.Library.Messages.Documents.DocumentTypes.HARDCOPY;
                        break;
                    case Shared.Models.WorkItems.DocumentTypes.CustomerCopy:
                        req.DocumentType = ConnectHub.Library.Messages.Documents.DocumentTypes.CUSTOMERCOPY;
                        break;
                }

                await cl.SendToServer(req);

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetObjectResult();
        }

        #region Connect Message Handlers
        private static void ReceiveAppointment(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg, string serial)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentDSSResponse)))
                TranscribeAppointment(msgHeader.RecieveMessage<AppointmentDSSResponse>(), msg, serial);

            msg.HasCompleted = true;
        }

        private static void ReceiveAppointmentCheckInResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentCheckInResponse)))
                ValidateAppointmentCheckIn(msgHeader.RecieveMessage<AppointmentCheckInResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveAppointmentCancelResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentCancelResponse)))
                ValidateAppointmentCancellation(msgHeader.RecieveMessage<AppointmentCancelResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveAppointmentDocumentResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Attachment> msg, string serial)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(DocumentResponse)))
                TranscribeDocument(msgHeader.RecieveMessage<DocumentResponse>(), msg, serial);

            msg.HasCompleted = true;
        }
        #endregion

        #region Transcribe Appointment
        private static void TranscribeAppointment(AppointmentDSSResponse resp, ConnectReceiveMessage<Appointment> msg, string serial)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.DropOffInstructions = resp.DropOffInstructions;
            msg.Object.SelfCheckInEnabled = resp.IsSelfCheckInEnabled;

            ConnectModelHelper.TranscribeAppointment(msg.Object, resp.Appointment);
            ConnectModelHelper.TranscribeContact(msg.Object.ContactInfo, resp.Contact);
            ConnectModelHelper.TranscribeVehicle(msg.Object.Vehicle, resp.Vehicle);

            msg.Object.ShopBanner = WebAppointmentsIntegration.GetShopBanner(serial, resp.Appointment.ShopRef);
            msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
        }
        #endregion

        #region Appointment Check In
        private static void ValidateAppointmentCheckIn(AppointmentCheckInResponse resp, ConnectReceiveMessage<Appointment> msg)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.IsCheckedIn = true;
            msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
        }
        #endregion

        #region Appointment Cancellation
        private static void ValidateAppointmentCancellation(AppointmentCancelResponse resp, ConnectReceiveMessage<Appointment> msg)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.IsCheckedIn = false;
            msg.Object.IsCanceled = false;

            msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
        }
        #endregion

        #region "Transcribe Document"
        private static void TranscribeDocument(DocumentResponse resp, ConnectReceiveMessage<Attachment> msg, string serial)
        {
            msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

            if (resp.Document?.Pages == null || resp.Document.Pages.Count == 0)
            {
                msg.HasError = true;
                msg.ErrorMessage = "Received a null or empty response from ConnectHub";
                return;
            }

            msg.Object.Content = resp.Document.Pages.First().Content;
            msg.LogMessage($"Successfully Received Response Message");
        }
        #endregion
    }
}
