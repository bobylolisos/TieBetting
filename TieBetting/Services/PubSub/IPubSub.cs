namespace TieBetting.Services.PubSub;

public interface IPubSub<TMessage> : IPubSub, IRecipient<TMessage> where TMessage : MessageBase
{
}

public interface IPubSub
{
    void RegisterMessages();
    void UnregisterMessages();

}