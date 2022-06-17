using PlayFab.MultiplayerModels;
using Serilog.Core;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;

namespace Client;

public class Group
{
    private readonly Logger logger;
    private List<Player> Players { get; }
    private Player Leader { get; }

    public Group(List<Player> players, Player leader, Logger logger)
    {
        this.logger = logger;
        Players = players;
        Leader = leader;
    }

    public async Task MakeMatch()
    {
        var otherPLayers = Players.Where(player => player.PlayFabId != Leader.PlayFabId);

        var entityKeys = new List<EntityKey>();
        foreach (var player in otherPLayers)
        {
            entityKeys.Add(new EntityKey
            {
                Id = player.context.EntityId,
                Type = player.context.EntityType
            });
        }
        
        var ticketId = await this.Leader.CreateGroupMatchmakingTicket(entityKeys);
        foreach (var player in Players)
        {
            player.JoinMatchmakingTicket(ticketId);
        }

        string ticketStatus = "";
        GetMatchmakingTicketResult ticket = new GetMatchmakingTicketResult(); 
        while (ticketStatus is not ("Matched" or "Canceled"))
        {
            ticket = await this.Leader.GetTicketState(ticketId);
            ticketStatus = ticket.Status;
            await Task.Delay(6000);
        }

        if (ticketStatus == "Matched")
        {
            GetMatchRequest getMatchRequest = new GetMatchRequest
            {
                MatchId = ticket.MatchId,
                QueueName = ticket.QueueName
            };
            var match = await this.Leader.mpApi.GetMatchAsync(getMatchRequest);
            logger.Information("Match: {@Match}", match.Result);
        }
    }
}