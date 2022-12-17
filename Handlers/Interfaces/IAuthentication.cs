using System.Security.Claims;
using LoginSample.Models;
using Nethereum.Web3;

namespace LoginSample.Handlers;

public interface IAuthentication
{
    public string IsPending(ClaimsPrincipal user);
    public string IsAuthenticated(ClaimsPrincipal claims);
    public Task<bool> CheckConfirmed(HttpContext httpContent, ClaimsPrincipal user, Account account);
    public Task<bool> Authenticate(ClaimsPrincipal User, HttpContext httpContext, Account web3);
    public Task<bool> SignTwoFactor(TwoFactorRequest request);
    public bool PairTwoFactor(string email, string message, string signature);
    public Task<bool> Disconnect(HttpContext httpContext);
    public string GenerateSignatureRequest();
}