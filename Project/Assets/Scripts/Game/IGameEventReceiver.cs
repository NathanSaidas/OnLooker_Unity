namespace Gem
{
    public interface IGameEventReceiver
    {
        void ReceiveEvent(ref GameEventData aData);
    }
}