namespace MockDoor.Data.Models.Headers
{
    public class RequestHeader : BaseHeader
    {
        public string Value { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ServiceRequestID { get; set; }
    }
}
