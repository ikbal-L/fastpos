using System.Runtime.Serialization;
using Caliburn.Micro;

namespace ServiceInterface.Model
{
    public class Ranked : PropertyChangedBase
    {
        private int? _rank;

        [DataMember]
        public int? Rank
        {
            get => _rank;
            set
            {
                _rank = value;
                NotifyOfPropertyChange();
            }
        }
    }
}