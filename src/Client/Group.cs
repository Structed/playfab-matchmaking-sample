using EntityKey = PlayFab.MultiplayerModels.EntityKey;

namespace Client;

public class Group
{
    private List<Player> Players { get; }
    private Player Leader { get; }

    public Group(List<Player> players, Player leader)
    {
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

        string ticketState = "";
        while (ticketState is not ("Matched" or "Canceled"))
        {
            ticketState = await this.Leader.GetTicketState(ticketId);
            await Task.Delay(6000);
        }
    }
}