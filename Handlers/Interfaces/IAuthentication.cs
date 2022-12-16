using System.Security.Claims;
using LoginSample.Models;
using Nethereum.Web3;

namespace LoginSample.Handlers;

public interface IAuthentication
{
    public string IsAuthenticated(ClaimsPrincipal claims);
    public Task<bool> Authenticate(ClaimsPrincipal User, HttpContext httpContext, Account web3);
    public Task<bool> SignTwoFactor(TwoFactorRequest request, HttpContext httpContent, ClaimsPrincipal user);
    public bool PairTwoFactor(string email, string message, string signature);
    public Task<bool> Disconnect(HttpContext httpContext);
    public string GenerateSignatureRequest();
}