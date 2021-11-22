using PlayFab.ClientModels;

namespace Client;

public class Group
{
    public List<Player> Players { get; }
    public Player Leader { get; }

    public Group(List<Player> players, Player leader)
    {
        Players = players;
        Leader = leader;
    }
}