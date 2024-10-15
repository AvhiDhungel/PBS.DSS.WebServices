using PBS.Blazor.ClientFramework.Services;

namespace PBS.DigitalServiceSuite.Client.ServiceExtensions
{
    public static partial class SharedStateServiceExtensions
    {
        #region Validation
        public static void SetInvalidServiceOrderError(this SharedStateService<SharedState> s) => s.AddError(Resources.UnableToLocationServiceOrder);
        public static void SetInvalidAppointmentError(this SharedStateService<SharedState> s) => s.AddError(Resources.UnableToLocationAppointment);

        public static bool ValidateServiceOrderRef(this SharedStateService<SharedState> s, string workItemRef, out Guid parsedWorkItemRef)
        {
            var isValid = ValidationHelper.ValidateWorkItemRef(workItemRef, out parsedWorkItemRef);
            if (!isValid) s.AddError(Resources.UnableToLocationServiceOrder);

            return isValid;
        }

        public static bool ValidateAppointmentRef(this SharedStateService<SharedState> s, string workItemRef, out Guid parsedWorkItemRef)
        {
            var isValid = ValidationHelper.ValidateWorkItemRef(workItemRef, out parsedWorkItemRef);
            if (!isValid) s.AddError(Resources.UnableToLocationAppointment);

            return isValid;
        }
        #endregion

        #region Signatures
        public static void SetSignatureRequired(this SharedStateService<SharedState> s, DocumentTypes t, SignatureActionTypes a)
        {
            s.SharedState.Signatures[t] = new Signature(a);
        }

        public static void SetSignature(this SharedStateService<SharedState> s, DocumentTypes t, byte[] sig)
        {
            s.GetSignature(t).Bytes = sig; s.SetSignature(t, s.GetSignature(t));
        }

        public static void SetSignature(this SharedStateService<SharedState> s, DocumentTypes t, Signature sig)
        {
            s.SharedState.Signatures[t] = sig;
        }

        public static bool RequiresSignature(this SharedStateService<SharedState> s, DocumentTypes t)
        {
            if (s.SharedState.Signatures.Count == 0) return false;
            if (!s.SharedState.Signatures.TryGetValue(t, out Signature? sig)) return false;

            return sig == null || !sig.HasSignature;
        }

        public static Signature GetSignature(this SharedStateService<SharedState> s, DocumentTypes t)
        {
            if (!s.SharedState.Signatures.TryGetValue(t, out Signature? sig)) return new();
            return sig ?? new();
        }
        #endregion

        #region Models
        public static async Task SaveModelToSession(this SharedStateService<SharedState> s, ServiceOrder model)
        {
            SetModel(s, model);
            await s.SaveToSession();
        }

        public static async Task SaveModelToSession(this SharedStateService<SharedState> s, Appointment model)
        {
            SetModel(s, model);
            await s.SaveToSession();
        }

        public static void SetModel(this SharedStateService<SharedState> s, ServiceOrder model)
        {
            s.SharedState.Model = model;
            s.SharedState.WorkItemRef = model.Id;
            s.SharedState.WorkItemType = WorkItemTypes.ServiceOrder;
        }

        public static void SetModel(this SharedStateService<SharedState> s, Appointment model)
        {
            s.SharedState.Model = model;
            s.SharedState.WorkItemRef = model.Id;
            s.SharedState.WorkItemType = WorkItemTypes.Appointment;
        }
        #endregion

        #region Banner
        public static bool HasBanner(this SharedStateService<SharedState> s) => s.SharedState.Banner.HasValue();
        public static string GetBanner(this SharedStateService<SharedState> s) => s.SharedState.Banner;
        public static void SetBanner(this SharedStateService<SharedState> s, string banner)
        {
            s.SharedState.Banner = banner;
            s.RefreshMainLayout();
        }
        #endregion
    }
}
