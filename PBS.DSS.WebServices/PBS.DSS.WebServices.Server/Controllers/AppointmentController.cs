using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.Appointments;
using PBS.ConnectHub.Library;
using PBS.ConnectHub.Library.Messages.DigitalServiceSuite;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Server.Integrations;
using PBS.DSS.WebServices.Server.Extensions;
using PBS.DSS.WebServices.Server.Utilities;

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
            var msg = new ConnectReceiveMessage<Appointment>(new Appointment());

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveAppointment(x, msg)))
            {
                await cl.SendToServer(new AppointmentDSSRequest { AppointmentRef = args.AppointmentRef });

                msg.WaitForCompletion();
            }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("CheckInAppointment")]
        public async Task<ActionResult<Appointment>> CheckInAppointment(Appointment appt, string serial)
        {
            var msg = new ConnectReceiveMessage<Appointment>(appt);

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveAppointmentCheckInResponse(x, msg)))
            {
                await cl.SendToServer(new AppointmentCheckInRequest { AppointmentRef = appt.Id, Odometer = appt.Odometer });

                msg.WaitForCompletion();
            }

            return msg.GetResult();
        }

        [HttpPost]
        [Route("CancelAppointment")]
        public async Task<ActionResult<Appointment>> CancelAppointment(Appointment appt, string serial)
        {
            var msg = new ConnectReceiveMessage<Appointment>(appt);

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(serial, (x) => ReceiveAppointmentCancelResponse(x, msg)))
            {
                await cl.SendToServer(new AppointmentCancelRequest { AppointmentRef = appt.Id });

                msg.WaitForCompletion();
            }

            return msg.GetResult();
        }

        #region Connect Message Handlers
        private static void ReceiveAppointment(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentDSSResponse)))
                TranscribeAppointment(msgHeader.RecieveMessage<AppointmentDSSResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveAppointmentCheckInResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentCheckInResponse)))
                ValidateAppointmentCheckIn(msgHeader.RecieveMessage<AppointmentCheckInResponse>(), msg);

            msg.HasCompleted = true;
        }

        private static void ReceiveAppointmentCancelResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<Appointment> msg)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentCancelResponse)))
                ValidateAppointmentCancellation(msgHeader.RecieveMessage<AppointmentCancelResponse>(), msg);

            msg.HasCompleted = true;
        }
        #endregion

        #region Transcribe Appointment
        private static void TranscribeAppointment(AppointmentDSSResponse resp, ConnectReceiveMessage<Appointment> msg)
        {
            msg.Object.ShopBanner = resp.ShopBanner;

            TranscribeAppointment(resp.Appointment, msg.Object);
        }

        private static void TranscribeAppointment(ConnectModels.Appointment connectAppt, Appointment appt)
        {
            if (connectAppt == null) return;

            appt.Id = connectAppt.WorkItemRef;
            appt.VehicleRef = connectAppt.VehicleRef;
            appt.ContactRef = connectAppt.ContactRef;
            appt.AppointmentNumber = connectAppt.AppointmentNumber;
            appt.AppointmentTime = connectAppt.AppointmentDateUTC;

            foreach (var connectReq in connectAppt.Requests)
            {
                var req = new RequestLine();

                req.RequestRef = connectReq.RequestRef;
                req.OpCodeRef = connectReq.OpCodeRef;
                req.OpCode = connectReq.OpCode;
                req.Description = connectReq.RequestDescription;

                appt.Requests.Add(req);
            }
        }
        #endregion

        #region Appointment Check In
        private static void ValidateAppointmentCheckIn(AppointmentCheckInResponse resp, ConnectReceiveMessage<Appointment> msg)
        {
            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.IsCheckedIn = true;
        }
        #endregion

        #region Appointment Cancellation
        private static void ValidateAppointmentCancellation(AppointmentCancelResponse resp, ConnectReceiveMessage<Appointment> msg)
        {
            if (!resp.Success) { msg.HasError = true; msg.ErrorMessage = resp.Message; return; }

            msg.Object.IsCheckedIn = false;
            msg.Object.IsCanceled = false;
        }
        #endregion
    }
}
