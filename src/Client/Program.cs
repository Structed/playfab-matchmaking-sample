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

var players = new List<Player>();
var playerPlayFabIds = new List<string>();

try
{
    for (int i = 0; i < 3; i++)
    {
        string customId = Guid.NewGuid().ToString();
        var player = new Player(customId, playFabApiSettings);
        await Login(player);
        players.Add(player);
        playerPlayFabIds.Add(player.PlayFabId);
    }
    
    var group = new Group(players, players.First());
    await BefriendPlayers(players, playerPlayFabIds);

    group.MakeMatch();
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

async Task BefriendPlayers(List<Player> playerList, List<string> playerPlayFabIdList)
{
    foreach (var player in playerList)
    {
        var friendIds = playerPlayFabIdList.Except(new[] { player.PlayFabId });
        foreach (var friendId in friendIds)
        {
            await player.AddFriend(FriendIdType.PlayFabId, friendId);
        }
    }
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