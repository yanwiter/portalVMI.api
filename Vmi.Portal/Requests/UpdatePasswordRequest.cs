namespace Vmi.Portal.Requests;

public class UpdatePasswordRequest
{
    public string Token { get; set; }
    public string email { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
