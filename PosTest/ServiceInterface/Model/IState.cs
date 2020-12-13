namespace ServiceInterface.Model
{
    public interface IState<TIdentifier> where TIdentifier:struct
    {
        public TIdentifier? Id { get; set; }
    }
}