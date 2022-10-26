namespace DI
{
    public interface IPausable
    {
        void OnApplicationPause();
        void OnApplicationResume(float pauseTime);
    }
}