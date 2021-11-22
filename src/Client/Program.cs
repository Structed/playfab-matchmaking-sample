// See https://aka.ms/new-console-template for more information

using Client;
using PlayFab;
using PlayFab.ClientModels;
using Spectre.Console;

var titleId = "60852"; // Group Matchmaking
var playFabApiSettings = new PlayFabApiSettings
{
    TitleId = titleId
};

try
{
    var player = new Player(Guid.NewGuid().ToString(), playFabApiSettings);
    await Login(player);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, 
        ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
        ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
}



async Task Login(Player player)
{
    var loginRequest = new LoginWithCustomIDRequest()
    {
        CustomId = player.customId,
        CreateAccount = true
    };
    PlayFabResult<LoginResult> login = await player.clientApi.LoginWithCustomIDAsync(loginRequest);
    LoginResult loginResult = VerifyPlayFabCall(login, "Login failed");
    AnsiConsole.MarkupLine($"[green]Logged in player {login.Result.PlayFabId}, CustomId={loginRequest.CustomId}[/]");
}

static TResult VerifyPlayFabCall<TResult>(PlayFabResult<TResult> playFabResult, string throwMsg) where TResult : PlayFab.Internal.PlayFabResultCommon
{
    if (playFabResult.Error != null)
    {
        AnsiConsole.MarkupLine($"[red]{playFabResult.Error.GenerateErrorReport()}[/]");
        throw new Exception($"{throwMsg} HttpStatus={playFabResult.Error.HttpStatus}");
    }
    return playFabResult.Result;
}