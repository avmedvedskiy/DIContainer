namespace DI
{
    public interface IPauseManager
    {
        void AddListener(IPausable handler);
        void RemoveListener(IPausable handler);
    }
}