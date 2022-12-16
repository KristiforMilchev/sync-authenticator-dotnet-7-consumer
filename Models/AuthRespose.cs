namespace LoginSample.Models;

public class AuthRespose
{
    public bool AccountFound { get; set; }
    public bool PendingTwoFactor { get; set; }
}