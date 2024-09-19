using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.Appointments;
using PBS.ConnectHub.Library;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Server.Integrations;
using PBS.DSS.WebServices.Server.Extensions;

namespace PBS.DSS.WebServices.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        [HttpPost]
        [Route("FetchAppointment")]
        public async Task<Appointment> FetchAppointment(AppointmentFetchArgs args)
        {
            var appt = new Appointment();

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveAppointment(x, appt)))
            {
                var apptReq = new ConnectModels.FetchAppointmentRequest() { AppointmentRef = args.AppointmentRef };

                await cl.StartConnection();
                await cl.SendToServer(apptReq);
            }

            return appt;
        }

        #region Connect Message Handlers
        private static void ReceiveAppointment(MessageHeaderV2 msgHeader, Appointment appt)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(ConnectModels.FetchAppointmentResponse)))
                TranscribeAppointment(appt, msgHeader.RecieveMessage<ConnectModels.Appointment>());
        }

        private static void TranscribeAppointment(Appointment appt, ConnectModels.Appointment connectAppt)
        {
            if (connectAppt == null) return;

            appt.Id = connectAppt.WorkItemRef;
            appt.VehicleRef = connectAppt.VehicleRef;
            appt.ContactRef = connectAppt.ContactRef;
            appt.AppointmentNumber = connectAppt.AppointmentNumber;
            appt.AppointmentTimeUTC = connectAppt.AppointmentDateUTC;

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

        #region Args
        public class AppointmentFetchArgs
        {
            public Guid AppointmentRef { get; set; } = Guid.Empty;
            public string SerialNumber { get; set; } = string.Empty;
        }
        #endregion

    }
}
