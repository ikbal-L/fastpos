namespace ServiceInterface.Model
{
    public interface IState<ID> where ID:struct
    {
        public ID? Id { get; set; }
    }
}