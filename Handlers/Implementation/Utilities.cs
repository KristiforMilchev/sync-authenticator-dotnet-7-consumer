using LoginSample.Models;

namespace LoginSample.Handlers.Implementation;

public class Utilities
{
    public static List<Account> RequestedTwoFactorPairs { get; set; } = new List<Account>();
    public static List<TwoFactorConfirmation> ExpectingTwoFactorSignature { get; set; } = new List<TwoFactorConfirmation>();
    public static string GetOsSavePath()
    {
        var result = string.Empty;
        string userName = Environment.UserName;
            
        switch (GetSystemOs())
        {
            case 1:
                result = $@"C:\Users\{userName}\syncdata";
                break;

            case 2:
                result = $@"/home/{userName}/syncdata";
                break;

            case 3:
                result = $@"/home/{userName}/syncdata";
                break;
        }

        if (!Directory.Exists(result))
            Directory.CreateDirectory(result);
            
            
        return result;
    }
    
    static int GetSystemOs()
    {

        if (OperatingSystem.IsWindows())
            return 1;
        if (OperatingSystem.IsLinux())
            return 2;
        if (OperatingSystem.IsMacOS())
            return 3;

        return 0;
    }
}