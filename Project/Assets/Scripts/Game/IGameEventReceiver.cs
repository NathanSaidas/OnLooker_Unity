#region CHANGE LOG
/* October,31,2014 - Nathan Hanlan, Added and implemented the interface IGameEventReceiver
 * 
 */
#endregion CHANGE LOG
namespace Gem
{
    public interface IGameEventReceiver
    {
        void ReceiveEvent(ref GameEventData aData);
    }
}