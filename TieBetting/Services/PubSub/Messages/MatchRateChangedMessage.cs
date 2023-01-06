namespace TieBetting.Services.PubSub.Messages;

public class MatchRateChangedMessage : MessageBase
{
    public MatchRateChangedMessage(double? rate)
    {
        Rate = rate;
    }

    public double? Rate { get; }
}