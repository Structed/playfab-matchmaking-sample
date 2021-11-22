using PlayFab;
using PlayFab.MultiplayerModels;

namespace Client;

public class Player
{
    public readonly string customId;
    public readonly HttpClient httpClient;
    public readonly PlayFabAuthenticationContext context;
    public readonly PlayFabClientInstanceAPI clientApi;
    public readonly PlayFabMultiplayerInstanceAPI mpApi;

    public string mmTicketId;
    public string mmMatchId;
    public GetMatchmakingTicketResult ticket;
    public GetMatchResult match;

    public Player(string customId, PlayFabApiSettings settings)
    {
        this.customId = customId;
        httpClient = new HttpClient();
        context = new PlayFabAuthenticationContext();
        clientApi = new PlayFabClientInstanceAPI(settings, context);
        mpApi = new PlayFabMultiplayerInstanceAPI(settings, context);
    }
}