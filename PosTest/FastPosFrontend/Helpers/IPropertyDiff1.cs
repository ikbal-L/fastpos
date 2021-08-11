namespace FastPosFrontend.Helpers
{
    public interface IPropertyDiff
    {
        void Invoke(object obj, PropertyMutation mutation);
    }
}