using PBS.Blazor.ClientFramework.Services;
using static PBS.Blazor.ClientFramework.Services.ControllerAPIService;

namespace PBS.DigitalServiceSuite.Client.ServiceExtensions
{
    public static partial class ControllerAPIServiceExtensions
    {
        #region Service Order
        public static async Task<APIResponse<ServiceOrder>> FetchServiceOrder(this ControllerAPIService s, ServiceOrderFetchArgs args)
        {
            return await s.Post<ServiceOrder, ServiceOrderFetchArgs>(args, "ServiceOrder", "FetchServiceOrder") ?? new();
        }
        #endregion

        #region Appointment
        public static async Task<APIResponse<Appointment>> FetchAppointment(this ControllerAPIService s, AppointmentFetchArgs args)
        {
            return await s.Post<Appointment, AppointmentFetchArgs>(args, "Appointment", "FetchAppointment") ?? new();
        }
        #endregion

        #region Document
        public static async Task<APIResponse<Attachment>> FetchServiceOrderDocument(this ControllerAPIService s, ServiceOrderDocumentFetchArgs args)
        {
            return await s.Post<Attachment, ServiceOrderDocumentFetchArgs>(args, "ServiceOrder", "FetchServiceOrderDocument") ?? new();
        }

        public static async Task<APIResponse<Attachment>> FetchAppointmentDocument(this ControllerAPIService s, AppointmentDocumentFetchArgs args)
        {
            return await s.Post<Attachment, AppointmentDocumentFetchArgs>(args, "Appointment", "FetchAppointmentDocument") ?? new();
        }
        #endregion
    }
}
