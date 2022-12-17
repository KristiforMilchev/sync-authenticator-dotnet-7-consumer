namespace LoginSample.Models;

public class TwoFactorConfirmation
{
    public Account Account { get; set; }
    public bool Confirmed { get; set; }
}