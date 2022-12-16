using System.Security.Claims;
using LoginSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Signer;
using Nethereum.Web3;

namespace LoginSample.Handlers.Implementation;

public class Authentication : IAuthentication
{
    public async Task<bool> Authenticate(ClaimsPrincipal User, HttpContext httpContext, Account account)
    {
        if (!account.IsTwoFactorEnabled)
        {
                     
            //Save vital login detail for later authenication 
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, account.Email));

         

            //Register the principal and authorize the user to use the system.
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true,
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);
            return false;
        }
        else
        {
            Utilities.ExpectingTwoFactorSignature.Add(new Account
            {
                Email = account.Email,
                
            });
            return true;
        }
    }

    public async Task<bool> SignTwoFactor(TwoFactorRequest request, HttpContext httpContent, ClaimsPrincipal user)
    {
        var existingUser = Utilities.ExpectingTwoFactorSignature.FirstOrDefault(x => x.Email == request.Email);
        if (existingUser == null)
            return false;
        
        var signer = new EthereumMessageSigner();
        var verify = signer.EncodeUTF8AndEcRecover(request.Message, request.Signed);
        
        // //Verify that the requester owns the account.
        if (verify != existingUser.TwoFactorKey)
            return false;
             
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, existingUser.Email));
 
             

        //Register the principal and authorize the user to use the system.
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.Now.AddDays(1),
            IsPersistent = true,
        };

        await httpContent.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);
        return true;
    }

    public bool PairTwoFactor(string email, string message, string signature)
    {
        var twoFaPairRequested= Utilities.RequestedTwoFactorPairs.FirstOrDefault(x=> x.Email == email);
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
        return $"Signature request: {Guid.NewGuid().ToString()}"; // Generates a message to be signed by web3 account.
    }

    public string IsAuthenticated(ClaimsPrincipal user)
    {
        if (user.Claims.FirstOrDefault() == null)
            return null;

        return user.Claims.ElementAt(1).Value;
         
    }

        
}