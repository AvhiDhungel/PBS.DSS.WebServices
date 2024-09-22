using Microsoft.AspNetCore.Mvc;
using ConnectModels = PBS.ConnectHub.Library.Messages.Appointments;
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
    public class AppointmentController : Controller
    {
        [HttpPost]
        [Route("FetchAppointment")]
        public async Task<Appointment> FetchAppointment(AppointmentFetchArgs args)
        {
            var appt = new Appointment();

            using (var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveAppointment(x, appt)))
            {
                var apptReq = new AppointmentDSSRequest { AppointmentRef = args.AppointmentRef };

                await cl.StartConnection();
                await cl.SendToServer(apptReq);
            }

            return appt;
        }

        #region Connect Message Handlers
        private static void ReceiveAppointment(MessageHeaderV2 msgHeader, Appointment appt)
        {
            if (msgHeader.IsConnectResponseMatch(typeof(AppointmentDSSResponse)))
                TranscribeAppointment(appt, msgHeader.RecieveMessage<AppointmentDSSResponse>());
        }

        private static void TranscribeAppointment(Appointment appt, AppointmentDSSResponse resp)
        {
            appt.ShopBanner = resp.ShopBanner;

            TranscribeAppointment(appt, resp);
        }

        private static void TranscribeAppointment(Appointment appt, ConnectModels.Appointment connectAppt)
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
    }
}
