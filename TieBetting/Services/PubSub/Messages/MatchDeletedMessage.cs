namespace TieBetting.Services.PubSub.Messages;

public class MatchDeletedMessage : MessageBase
{
    public MatchDeletedMessage(string matchId)
    {
        MatchId = matchId;
    }

    public string MatchId { get; }
}