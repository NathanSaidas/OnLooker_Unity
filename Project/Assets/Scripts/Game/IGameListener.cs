namespace Gem
{
    public interface IGameListener
    {
        void OnGamePaused();
        void OnGameUnpaused();
        void OnGameReset();
    }
}