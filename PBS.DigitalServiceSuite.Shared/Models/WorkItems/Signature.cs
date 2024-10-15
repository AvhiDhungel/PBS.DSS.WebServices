using PBS.DigitalServiceSuite.Shared.Enums;

namespace PBS.DigitalServiceSuite.Shared.Models.WorkItems
{
    public class Signature
    {
        public SignatureActionTypes ActionTypes { get; set; }
        public byte[]? Bytes { get; set; } = null;

        public bool HasSignature => Bytes != null && Bytes.Length > 0;

        public Signature() { }
        public Signature(SignatureActionTypes actionTypes) => ActionTypes = actionTypes;
    }
}
