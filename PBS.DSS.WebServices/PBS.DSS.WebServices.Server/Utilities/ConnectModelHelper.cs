using ConnectSO = PBS.ConnectHub.Library.Messages.ServiceOrders;
using ConnectAppt = PBS.ConnectHub.Library.Messages.Appointments;
using Connect = PBS.ConnectHub.Library.Messages;
using PBS.ConnectHub.Library;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.Shared.Models;
using PBS.Blazor.Framework.Extensions;

namespace PBS.DSS.WebServices.Server.Utilities
{
    public class ConnectModelHelper
    {
        #region Service Order
        public static void TranscribeServiceOrder(ServiceOrder so, ConnectSO.ServiceOrder connectSO)
        {
            if (connectSO == null) return;

            so.Id = connectSO.WorkItemRef;
            so.VehicleRef = connectSO.VehicleRef;
            so.ContactRef = connectSO.ContactRef;
            so.SONumber = connectSO.ServiceOrderNumber;
            so.AdvisorName = connectSO.CSRDisplay;
            so.AdditionalComments = connectSO.SONote;
            so.IsOpen = !connectSO.DateClosedUTC.HasValue;

            so.SubTotal = (double)connectSO.SubTotal;
            so.TaxTotal = (double)connectSO.Taxes;
            so.FeesTotal = (double)connectSO.Fees;
            so.GrandTotal = (double)connectSO.GrandTotal;

            foreach (var connectReq in connectSO.Requests)
            {
                var req = new RequestLine();

                req.RequestRef = connectReq.RequestRef;
                req.OpCodeRef = connectReq.OpCodeRef;
                req.OpCode = connectReq.OpCode;
                req.Description = connectReq.RequestDescription;
                req.EstimatedLabour = (double)connectReq.EstimatedLabour;
                req.EstimatedParts = (double)connectReq.EstimatedParts;
                req.IsInspection = connectReq.RequestLineType.IsInSet(ConnectSO.RequestLineTypes.MemoInspection, ConnectSO.RequestLineTypes.BillableInspection);

                switch (connectReq.RequestLineStatus)
                {
                    case ConnectSO.RequestLineStatuses.APPROVED:
                        req.AWRStatus = AWRStatuses.Approved;
                        break;
                    case ConnectSO.RequestLineStatuses.PENDING:
                        req.AWRStatus = AWRStatuses.Pending;
                        break;
                    case ConnectSO.RequestLineStatuses.DEFERRED:
                        req.AWRStatus = AWRStatuses.Deferred;
                        break;
                    case ConnectSO.RequestLineStatuses.DECLINED:
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

        public static void TranscribeSOTimeline(ServiceOrder so, List<ConnectSO.WorkItemTimelineTypes> entries)
        {
            foreach (var entry in entries.OrderDescending())
            {
                if (entry == ConnectSO.WorkItemTimelineTypes.AWRSent) so.SentForApproval = true;

                if (entry == ConnectSO.WorkItemTimelineTypes.ReadyForPickup && so.Timeline < Shared.Enums.ServiceOrderTimeline.Ready)
                    so.Timeline = Shared.Enums.ServiceOrderTimeline.Ready;
                else if (entry == ConnectSO.WorkItemTimelineTypes.AllJobsComplete && so.Timeline < Shared.Enums.ServiceOrderTimeline.Finalizing)
                    so.Timeline = Shared.Enums.ServiceOrderTimeline.Finalizing;
                else if (entry == ConnectSO.WorkItemTimelineTypes.JobStarted && so.Timeline < Shared.Enums.ServiceOrderTimeline.InProgress)
                    so.Timeline = Shared.Enums.ServiceOrderTimeline.InProgress;
                else if (entry == ConnectSO.WorkItemTimelineTypes.InspectionStarted && so.Timeline < Shared.Enums.ServiceOrderTimeline.Inspection)
                    so.Timeline = Shared.Enums.ServiceOrderTimeline.Inspection;
            }
        }
        #endregion

        #region Appointment
        public static void TranscribeAppointment(Appointment appt, ConnectAppt.Appointment connectAppt)
        {
            if (connectAppt == null) return;

            appt.Id = connectAppt.WorkItemRef;
            appt.VehicleRef = connectAppt.VehicleRef;
            appt.ContactRef = connectAppt.ContactRef;
            appt.AppointmentNumber = connectAppt.AppointmentNumber;
            appt.AppointmentTime = connectAppt.AppointmentDateUTC;
            appt.PickupDate = connectAppt.PickupTimeUTC;
            appt.Odometer = connectAppt.OdomIn;
            appt.IsCheckedIn = connectAppt.Status == ConnectAppt.Appointment.AppointmentStatuses.CHECKEDIN;
            appt.IsCanceled = connectAppt.Status == ConnectAppt.Appointment.AppointmentStatuses.DELETED;

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

        #region Contact
        public static void TranscribeContact(Contact c, Connect.Contact connectC)
        {
            if (connectC == null) return;

            c.Id = connectC.ContactId;
            c.FirstName = connectC.FirstName;
            c.LastName = connectC.LastName;
            c.Phone = connectC.CellPhone;
            c.Email = connectC.EmailAddress;
        }
        #endregion

        #region Vehicle
        public static void TranscribeVehicle(Vehicle v, Connect.Vehicle connectV)
        {
            if (connectV == null) return;

            v.Id = connectV.VehicleRef;
            v.Year = connectV.Year;
            v.Make = connectV.Make;
            v.Model = connectV.Model;
            v.Trim = connectV.Trim;
            v.VIN = connectV.VIN;
        }
        #endregion
    }
}
