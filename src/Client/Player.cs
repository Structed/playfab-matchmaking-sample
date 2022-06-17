using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Spectre.Console;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;

namespace Client;

public class Player
{
    public readonly string customId;
    private readonly string queueName;
    public readonly HttpClient httpClient;
    public readonly PlayFabAuthenticationContext context;
    public readonly PlayFabClientInstanceAPI clientApi;
    public readonly PlayFabMultiplayerInstanceAPI mpApi;

    public string mmTicketId;
    public string mmMatchId;
    public GetMatchmakingTicketResult ticket;
    public GetMatchResult match;

    public string PlayFabId => this.context.PlayFabId;

    public Player(string customId, PlayFabApiSettings settings, string queueName = "4v4")
    {
        this.customId = customId;
        this.queueName = queueName;
        httpClient = new HttpClient();
        context = new PlayFabAuthenticationContext();
        clientApi = new PlayFabClientInstanceAPI(settings, context);
        mpApi = new PlayFabMultiplayerInstanceAPI(settings, context);
    }
    
    /// <summary>
    /// Creates a new matchmaking ticket for group
    /// </summary>
    /// <returns>A TicketID</returns>
    public async Task<string> CreateGroupMatchmakingTicket(List<EntityKey> teamPlayerEntities)
    {
        var request = new CreateMatchmakingTicketRequest
        {
            // The ticket creator specifies their own player attributes.
            Creator = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = this.context.EntityId,
                    Type = this.context.EntityType
                }
            },

            // Cancel matchmaking if a match is not found after 120 seconds.
            GiveUpAfterSeconds = 60,   // rule eval only every 10 seconds

            // The name of the queue to submit the ticket into.
            QueueName = this.queueName,
            
            MembersToMatchWith = teamPlayerEntities
        };

        var response = await this.mpApi.CreateMatchmakingTicketAsync(request);
        if (response is null)
        {
            AnsiConsole.MarkupLine("[red]failed[/]");
        }
        return response.Result.TicketId;
    }

    public async void JoinMatchmakingTicket(string ticketId)
    {
        var request = new JoinMatchmakingTicketRequest
        {
            TicketId = ticketId,
            QueueName = this.queueName,
            Member = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = this.context.EntityId,
                    Type = this.context.EntityType,
                }
            }
        };
        await this.mpApi.JoinMatchmakingTicketAsync(request);
    }
    
    public async Task<GetMatchmakingTicketResult> GetTicketState(string ticketId)
    {
        var request = new GetMatchmakingTicketRequest
        {
            TicketId = ticketId,
            QueueName = this.queueName,
        };

        var ticket = await this.mpApi.GetMatchmakingTicketAsync(request);

        if (ticket.Error != null)
        {
            throw new Exception(ticket.Error.GenerateErrorReport());
        }

        Console.WriteLine($"Status of Ticket ID {ticket.Result.TicketId}: {ticket.Result.Status}");
        return ticket.Result;
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
        return this.clientApi.AddFriendAsync(request);
    }
}