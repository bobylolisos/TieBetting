namespace TieBetting.Services.PubSub.Messages;

public class TeamUpdatedMessage : MessageBase
{
    public TeamUpdatedMessage(string teamName)
    {
        TeamName = teamName;
    }

    public string TeamName { get; }
}