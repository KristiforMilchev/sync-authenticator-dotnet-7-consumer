using System.Diagnostics;
using LoginSample.Handlers;
using LoginSample.Handlers.Implementation;
using Microsoft.AspNetCore.Mvc;
using LoginSample.Models;
using Nethereum.Web3;
using Newtonsoft.Json;

namespace LoginSample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private  IAuthentication Authentication { get; set; }
    
    public HomeController(ILogger<HomeController> logger, IAuthentication authentication)
    {
        _logger = logger;
        Authentication = authentication;
    }

    public IActionResult Index()
    {
        var account = Authentication.IsAuthenticated(User);

        ViewData["FullAddress"] = account;

        return View();
    }
    
    public IActionResult Account()
    {
        var account = Authentication.IsAuthenticated(User);
        if(string.IsNullOrEmpty(account))
            return Redirect("/home/Error");
 
        ViewData["FullAddress"] = account;

        return View();
    }


    public IActionResult Privacy()
    {
        var account = Authentication.IsAuthenticated(User);
        if (string.IsNullOrEmpty(account))
            return Redirect("/home/Error");

        
        return View();
    }

    [HttpGet]
    public string GetSignatureMessage()
    {
        return Authentication.GenerateSignatureRequest();
    }

    [HttpGet]
    public string EnableTwoFa()
    {
        var exist = Authentication.IsAuthenticated(User);
        if (string.IsNullOrEmpty(exist))
            return string.Empty;
        var dbContext = new DatabaseContext();
        var user = dbContext.Single(exist);
        
        Utilities.RequestedTwoFactorPairs.Add(user);
        var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
        
        var url =  location.Authority;
        return JsonConvert.SerializeObject(new TwoFaRegisterCode
        {
            url =$"http://{url}",
            email = exist,
            message = "Hello World"
        });
    }
    
    [HttpGet]
    public async Task<bool> DisconnectSession()
    {
        return await Authentication.Disconnect(HttpContext);
    }
    
    [HttpPost]
    public async Task<AuthRespose> Login([FromBody] RegisterDto dto)
    {
        var dbContext = new DatabaseContext();
        var exists = dbContext.Single(dto.Email);
        var userLogged = Authentication.IsAuthenticated(User);
        
        if (exists == null)
            return new AuthRespose
            {
                AccountFound = false,
                PendingTwoFactor = false
            };
        
        if(!string.IsNullOrEmpty(userLogged) && exists.IsTwoFactorEnabled == false)
            return new AuthRespose
            {
                AccountFound = true,
                PendingTwoFactor = false
            };
        
        var result = await Authentication.Authenticate(User, HttpContext,exists);

        return new AuthRespose
        {
            AccountFound = true,
            PendingTwoFactor = result
        };
    }
    
    [HttpPost]
    public async Task<bool> Register([FromBody] RegisterDto dto)
    {
        var dbContext = new DatabaseContext();
        var exists = dbContext.Single(dto.Email);

        if (exists != null)
            return false;

        dbContext.Create(new Account
        {
            Email = dto.Email,
            Name = dto.Email,
            Password = dto.Password,
            IsTwoFactorEnabled = false
        });

        return true;
    }
    
    [HttpPost]
    public async Task<bool> Pair([FromBody] TwoFactorRequest dto)
    {
        var account = Utilities.RequestedTwoFactorPairs.FirstOrDefault(x => x.Email == dto.Email);
        if (account == null)
            return false;
        

        var result = Authentication.PairTwoFactor(account.Email, dto.Message, dto.Signed);
        await DisconnectSession();
        return true;
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}