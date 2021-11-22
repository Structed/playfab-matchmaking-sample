using PlayFab;
using PlayFab.ClientModels;
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

    public string PlayFabId => this.context.PlayFabId;

    public Player(string customId, PlayFabApiSettings settings)
    {
        this.customId = customId;
        httpClient = new HttpClient();
        context = new PlayFabAuthenticationContext();
        clientApi = new PlayFabClientInstanceAPI(settings, context);
        mpApi = new PlayFabMultiplayerInstanceAPI(settings, context);
    }
    

    public Task AddFriend(FriendIdType idType, string friendId) {
        var request = new AddFriendRequest();
        switch (idType) {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        // this.clientInstanceApi.AddFriendAsync()
        return this.clientApi.AddFriendAsync(request);
    }
}