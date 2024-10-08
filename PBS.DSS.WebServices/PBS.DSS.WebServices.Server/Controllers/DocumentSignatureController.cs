using Microsoft.AspNetCore.Mvc;
using ConnectDocuments = PBS.ConnectHub.Library.Messages.Documents;
using PBS.ConnectHub.Library;
using PBS.Blazor.ServerFramework;
using PBS.Blazor.ServerFramework.Integrations;
using PBS.DSS.Shared.Args;

namespace PBS.DSS.WebServices.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentSignatureController : Controller
    {
        [HttpPost]
        [Route("SignAppointmentHardCopy")]
        public async Task<ActionResult> SignAppointmentHardCopy(DocumentSignatureArgs args)
        {
            var msg = new ConnectReceiveMessage<DocumentSignatureArgs>(args, args.SerialNumber, "FetchAppointment");

            msg.LogMessage($"Attempting to Sign Appointment Hard Copy for Serial {args.SerialNumber} Appt: {args.WorkItemNumber} with the following arguments:");
            msg.LogSerialized(args);
            msg.LogNewLine();

            if (args.Signature.Bytes == null || !args.Signature.HasSignature)
            {
                msg.LogMessage("No signature bytes found, bailing out of signature attempt.");
                msg.ErrorMessage = "No Signature Found";
                msg.HasError = true;

                return msg.GetResult();
            }

            try
            {
                using var cl = await ConnectHubIntegration.GetConnectHubClient(args.SerialNumber, (x) => ReceiveDocumentSigningResponse(x, msg, args.SerialNumber));

                var req = new ConnectDocuments.DocumentSigningRequest()
                {
                    WorkItemId = args.WorkItemRef,
                    Department = ConnectDocuments.Products.APPOINTMENT,
                    DocumentType = ConnectDocuments.DocumentTypes.HARDCOPY,
                };

                var connectSig = new ConnectDocuments.Signature();
                connectSig.Name = "AppointmentHardCopy";
                connectSig.Content = args.Signature.Bytes;

                req.Signatures.Add(connectSig);

                await cl.SendToServer(req);

                msg.WaitForCompletion();
            }
            catch (Exception ex) { msg.LogException(ex); }
            finally { msg.UpdateLog(); }

            return msg.GetResult();
        }

        #region Connect Message Handlers
        private static void ReceiveDocumentSigningResponse(MessageHeaderV2 msgHeader, ConnectReceiveMessage<DocumentSignatureArgs> msg, string serial)
        {
            msg.LogMessageHeader(msgHeader);

            if (msgHeader.IsConnectResponseMatch(typeof(ConnectDocuments.DocumentSigningResponse)))
            {
                var resp = msgHeader.RecieveMessage<ConnectDocuments.DocumentSigningResponse>();

                msg.LogSerializedWithMessage(resp, "Connect Hub Response:");

                if (resp == null) { msg.HasError = true; msg.ErrorMessage = "Connect Document Signature Request Failed"; return; }

                msg.LogSerializedWithMessage(msg.Object, "Transcribed Response Message:");
            }

            msg.HasCompleted = true;
        }
        #endregion
    }
}
