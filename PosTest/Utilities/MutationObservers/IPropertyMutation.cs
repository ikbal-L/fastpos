namespace Utilities.Mutation
{
    public interface IPropertyMutation
    {
        object Committed { get; set; }
        object Initial { get; set; }
        bool IsCommitted { get; set; }
        string PropertyName { get; }
    }
}