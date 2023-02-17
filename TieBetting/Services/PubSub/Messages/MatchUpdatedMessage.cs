namespace TieBetting.Services.PubSub.Messages;

public class MatchUpdatedMessage : MessageBase
{
    public MatchUpdatedMessage(string matchId)
    {
        MatchId = matchId;
    }

    public string MatchId { get; }
}