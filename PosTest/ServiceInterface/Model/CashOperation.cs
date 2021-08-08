using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
   public class CashOperation
    {
  
        private long _id;

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private decimal _amount;

        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public long PaymentId;
    }
}
