namespace Services
{
    /// <summary>
    /// Should be used only for readonly fields
    /// </summary>
    public static class Inject
    {
        public static T Service<T>() => ProjectContext.GetService<T>();
    }
}
