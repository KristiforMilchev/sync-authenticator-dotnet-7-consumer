namespace LoginSample.Models;

public class TwoFactorRequest
{
    public string Email { get; set; }
    public string Message { get; set; }
    public string Signed { get; set; }
}