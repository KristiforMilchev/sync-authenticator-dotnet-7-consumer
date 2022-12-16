 
function Disconnect() {
    localStorage.removeItem("WEB3_CONNECT_CACHED_PROVIDER");
    localStorage.removeItem("account");

    $.ajax({
        method: "GET",
        contentType: "application/json",
        url: "/Home/DisconnectSession",
    }).done(async function (session) {
 
        window.location.href = "/";

    });
}

 
function RegisterAccount()
{

    var volumeDTO = {
        Name: $("#rName").val(),
        Email: $("#rEmail").val(),
        Password: $("#rPassword").val()
    }

    $.ajax({
        method: "POST",
        contentType: "application/json",
        url: "/Home/Register",
        data: JSON.stringify(volumeDTO)
    }).done(function (msg) {
        // SetAccount(selectedAccount);
        if (msg) {
            alert("Account created, please login.")
            window.location.reload();
        }
        else
            alert("failed");
    });
}
 
function Login()
{
    var volumeDTO = {

        Email: $("#lEmail").val(),
        Password: $("#lPassword").val()
    }

    $.ajax({
        method: "POST",
        contentType: "application/json",
        url: "/Home/Login",
        data: JSON.stringify(volumeDTO)
    }).done(function (msg) {
        // SetAccount(selectedAccount);
        if (msg.accountFound && !msg.pendingTwoFactor) 
            window.location = "/home/account";
        else if(msg.accountFound && msg.pendingTwoFactor)
            window.location = "/home/confirm";
        else if(!msg.accountFound)
            alert("Wrong username or password");
    });
}
