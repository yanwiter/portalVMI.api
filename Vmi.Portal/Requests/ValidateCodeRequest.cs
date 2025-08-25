namespace Vmi.Portal.Requests
{
    public class ValidateCodeRequest
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
