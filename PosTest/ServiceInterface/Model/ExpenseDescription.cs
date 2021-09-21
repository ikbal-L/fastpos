using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    public class ExpenseDescription:IState<long>
    {
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
    