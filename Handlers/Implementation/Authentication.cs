using System.Security.Claims;
using System.Text;
using LoginSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;


namespace LoginSample.Handlers.Implementation;

public class Authentication : IAuthentication
{
    public async Task<bool> Authenticate(ClaimsPrincipal User, HttpContext httpContext, Account account)
    {
        //Save vital login detail for later authenication 
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
            ClaimTypes.Role);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, account.Email));
        var result = false;
        if (account.IsTwoFactorEnabled)
        {
            Utilities.ExpectingTwoFactorSignature.Add(new TwoFactorConfirmation
            {
                Account = account,
                Confirmed = false

            });
            result = true;
            identity.AddClaim(new Claim(ClaimTypes.Name, "0"));

        }
        else
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, "1"));

        }

        //Register the principal and authorize the user to use the system.
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.Now.AddDays(1),
            IsPersistent = true,
        };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal),
            authProperties);
        return result;
    }

    public async Task<bool> SignTwoFactor(TwoFactorRequest request)
    {
        var existingUser = Utilities.ExpectingTwoFactorSignature.FirstOrDefault(x => x.Account.Email == request.Email);
        if (existingUser == null)
            return false;

         var signer = new EthereumMessageSigner();

         
         var verify = signer.EncodeUTF8AndEcRecover(request.Message, request.Signed);
         //var verify = signer.EncodeUTF8AndEcRecover(request.Message, request.Signed);
        
        // //Verify that the requester owns the account.
         if (verify != existingUser.Account.TwoFactorKey)
             return false;

        Utilities.ExpectingTwoFactorSignature.FirstOrDefault(x => x.Account.Email == request.Email).Confirmed = true;
        return true;
    }

    public async Task<bool> CheckConfirmed(HttpContext httpContent, ClaimsPrincipal user, Account account)
    {
        if (!Utilities.ExpectingTwoFactorSignature.FirstOrDefault(x => x.Account.Email == account.Email)!.Confirmed)
            return false;
        
        var result = await Disconnect(httpContent);

        //Save vital login detail for later authentication 
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
            ClaimTypes.Role);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, account.Email));
        identity.AddClaim(new Claim(ClaimTypes.Name, "1"));

        //Register the principal and authorize the user to use the system.
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.Now.AddDays(1),
            IsPersistent = true,
        };

        await httpContent.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal),
            authProperties);
        return true;
    }

    public bool PairTwoFactor(string email, string message, string signature)
    {
        var twoFaPairRequested = Utilities.RequestedTwoFactorPairs.FirstOrDefault(x => x.Email == email);
        if (twoFaPairRequested == null)
            return false;

        var signer = new EthereumMessageSigner();
  
        var verify = signer.EncodeUTF8AndEcRecover(message, signature);
        var dbContext = new DatabaseContext();
        var getUser = dbContext.Single(email);
        if (getUser == null)
            return false;

        getUser.IsTwoFactorEnabled = true;
        getUser.TwoFactorKey = verify;
        dbContext.Update(getUser);

        return true;
    }

    public async Task<bool> Disconnect(HttpContext httpContext)
    {
        await httpContext.SignOutAsync();
        return true;
    }

    public string GenerateSignatureRequest()
    {
        // Generates a message to be signed by web3 account.
        return Guid.NewGuid().ToString().Replace("-","");
    }

    public string IsAuthenticated(ClaimsPrincipal user)
    {
        if (user.Claims.FirstOrDefault() == null)
            return String.Empty;

        return user.Claims.ElementAt(2).Value == "0" ? string.Empty : user.Claims.ElementAt(1).Value;
    }

    public string IsPending(ClaimsPrincipal user)
    {
        if (user.Claims.FirstOrDefault() == null)
            return String.Empty;

        return user.Claims.ElementAt(1).Value;
    }


}