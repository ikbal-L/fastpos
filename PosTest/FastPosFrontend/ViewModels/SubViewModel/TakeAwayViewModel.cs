using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels.SubViewModel
{
    public class TakeawayViewModel : CheckoutSubViewModel
    {


        public TakeawayViewModel(CheckoutViewModel parentViewModel) : base(parentViewModel)
        {

        }

        public override bool FilterOrderByType(Order order)
        {
            OrderState?[] filteredStates = { OrderState.Canceled, OrderState.Payed, OrderState.Removed };
            return order.Type == OrderType.TakeAway && !filteredStates.Contains((order.State));
        }
    }
}
