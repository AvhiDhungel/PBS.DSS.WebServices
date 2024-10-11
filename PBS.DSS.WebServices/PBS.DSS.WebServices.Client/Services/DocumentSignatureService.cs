using PBS.Blazor.ClientFramework.Services;
using PBS.Blazor.Framework.Extensions;
using PBS.DSS.Shared.Args;
using PBS.DSS.Shared.Criteria;
using PBS.DSS.Shared.Enums;
using PBS.DSS.Shared.Models.States;
using PBS.DSS.Shared.Models.WorkItems;
using PBS.DSS.WebServices.Client.Pages.App;
using PBS.DSS.WebServices.Client.ServiceExtensions;
using System.Net.Http.Headers;

namespace PBS.DSS.WebServices.Client.Services
{
    public sealed class DocumentSignatureService(SharedStateService<SharedState> sharedStateService, ControllerAPIService controllerAPIService)
    {
        private readonly SharedStateService<SharedState> _sharedStateService = sharedStateService;
        private readonly ControllerAPIService _controllerAPIService = controllerAPIService;

        private SharedState SharedState => _sharedStateService.SharedState;

        #region Fetch
        public async Task<byte[]?> FetchDocument(DocumentTypes docType)
        {
            if (SharedState.WorkItemType == WorkItemTypes.ServiceOrder)
                return await FetchServiceOrderDocument(docType);
            else if (SharedState.WorkItemType == WorkItemTypes.Appointment)
                return await FetchAppointmentDocument(docType);
            else
                return null;
        }

        public async Task<byte[]?> FetchServiceOrderDocument(DocumentTypes docType)
        {
            var docArgs = new ServiceOrderDocumentFetchArgs();
            docArgs.ServiceOrderRef = SharedState.WorkItemRef;
            docArgs.SerialNumber = SharedState.SerialNumber;
            docArgs.DocumentType = docType;

            var resp = await _controllerAPIService.FetchServiceOrderDocument(docArgs);

            return resp.HasError || resp.ResponseObject == null ? null : resp.ResponseObject.Content;
        }

        public async Task<byte[]?> FetchAppointmentDocument(DocumentTypes docType)
        {
            var docArgs = new AppointmentDocumentFetchArgs();
            docArgs.AppointmentRef = SharedState.WorkItemRef;
            docArgs.SerialNumber = SharedState.SerialNumber;
            docArgs.DocumentType = docType;

            var resp = await _controllerAPIService.FetchAppointmentDocument(docArgs);

            return resp.HasError || resp.ResponseObject == null ? null : resp.ResponseObject.Content;
        }
        #endregion

        #region Sign
        public async Task<bool> SignDocument(DocumentTypes docType, byte[] sig)
        {
            var signature = _sharedStateService.GetSignature(docType);
            signature.Bytes = sig;

            return await SignDocument(docType, signature);
        }

        public async Task<bool> SignDocument(DocumentTypes docType, Signature signature)
        {
            _sharedStateService.SetSignature(docType, signature);

            await _sharedStateService.SaveToSession();

            if (signature.ActionTypes == SignatureActionTypes.AppointmentCheckIn)
                return await CheckInAppointment(signature);
            else if (signature.ActionTypes == SignatureActionTypes.SignRecommendedServices)
                return await ConfirmAWR(signature);
            else if (signature.ActionTypes == SignatureActionTypes.SignInvoice)
                return await SignSOInvoice(signature);

            return false;
        }
        #endregion

        #region Service Order
        public async Task<bool> ConfirmAWR(Signature signature)
        {
            var so = _sharedStateService.GetModel<ServiceOrder>();
            if (so == null || so.SerialNumber.IsEmpty() || so.Id == Guid.Empty) return false;

            var sigArgs = new DocumentSignatureArgs()
            {
                Signature = signature,
                SerialNumber = so.SerialNumber,
                WorkItemRef = so.Id,
                WorkItemNumber = so.SONumber
            };

            var docRes = await _controllerAPIService.Post(sigArgs, "DocumentSignature", "SignServiceOrderCustomerCopy");
            if (docRes != null && docRes.IsSuccessStatusCode) return false;

            var soResp = await _controllerAPIService.Post(so, "ServiceOrder", "ApproveAWR");
            return soResp?.IsSuccessStatusCode ?? false;
        }

        public async Task<bool> SignSOInvoice(Signature signature)
        {
            var so = _sharedStateService.GetModel<ServiceOrder>();
            if (so == null || so.SerialNumber.IsEmpty() || so.Id == Guid.Empty) return false;

            var sigArgs = new DocumentSignatureArgs()
            {
                Signature = signature,
                SerialNumber = so.SerialNumber,
                WorkItemRef = so.Id,
                WorkItemNumber = so.SONumber
            };

            var docRes = await _controllerAPIService.Post(sigArgs, "DocumentSignature", "SignServiceOrderCustomerCopy");
            return docRes != null && docRes.IsSuccessStatusCode;
        }
        #endregion

        #region Appointment
        public async Task<bool> CheckInAppointment(Signature signature)
        {
            var appt = _sharedStateService.GetModel<Appointment>();
            if (appt == null || appt.SerialNumber.IsEmpty() || appt.Id == Guid.Empty) return false;

            var sigArgs = new DocumentSignatureArgs()
            {
                Signature = signature,
                SerialNumber = appt.SerialNumber,
                WorkItemRef = appt.Id,
                WorkItemNumber = appt.AppointmentNumber
            };

            var docRes = await _controllerAPIService.Post(sigArgs, "DocumentSignature", "SignAppointmentHardCopy");
            if (docRes == null || !docRes.IsSuccessStatusCode) return false;

            var apptResp = await _controllerAPIService.Post(appt, "Appointment", "CheckInAppointment");

            return apptResp?.IsSuccessStatusCode ?? false;
        }
        #endregion
    }
}
