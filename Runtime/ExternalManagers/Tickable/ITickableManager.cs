namespace DI
{
    public interface ITickableManager
    {
        void AddListener(ITickable tickable);
        void RemoveListener(ITickable tickable);
    }
}