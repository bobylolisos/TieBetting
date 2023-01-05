namespace TieBetting.Services.PubSub.Messages;

public class MatchRateChangedMessage : MessageBase
{
    public MatchRateChangedMessage(decimal? rate)
    {
        Rate = rate;
    }

    public decimal? Rate { get; }
}