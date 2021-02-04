namespace ServiceInterface.Interface
{
    public interface IPath
    {

        IPathBuilder SubPath(string name);
        IPath Build();

    }

    public interface IPathBuilder : IPath
    {
        IPath Arg(object value);
    }
}