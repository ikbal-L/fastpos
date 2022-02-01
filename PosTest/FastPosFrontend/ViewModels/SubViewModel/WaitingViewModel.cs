using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels.SubViewModel
{
    public class WaitingViewModel : CheckoutSubViewModel
    {


        public WaitingViewModel(CheckoutViewModel parentViewModel) : base(parentViewModel)
        {

            
        }

        public override bool FilterOrderByType(Order order)
        {
            OrderState?[] filteredStates = { OrderState.Canceled, OrderState.Payed, OrderState.Removed };

            return order.Type == OrderType.InWaiting && !filteredStates.Contains(order.State);
        }
    }


    public abstract class SubViewModel<T>:PropertyChangedBase where T:INotifyPropertyChanged
    {

        public SubViewModel(T parentViewModel)
        {
            ParentViewModel = parentViewModel;
        }
        public T ParentViewModel { get; set; }
    }

    public abstract class CheckoutSubViewModel : SubViewModel<CheckoutViewModel>
    {
        protected CheckoutSubViewModel(CheckoutViewModel parentViewModel) : base(parentViewModel)
        {
            OrderViewSource = new CollectionViewSource();
            OrderViewSource.Source = ParentViewModel.Orders;
            OrderViewSource.Filter += OrderTypeFilter;
            Orders = OrderViewSource.View;
        }

        private void OrderTypeFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is Order order)
            {
                e.Accepted = FilterOrderByType(order);
                return;
            }
            e.Accepted = false;
        }
        public abstract bool FilterOrderByType(Order order);

        public CollectionViewSource OrderViewSource { get; set; }

      

        private Order _selectedOrder;
        private ICollectionView _orders;

        public ICollectionView Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                NotifyOfPropertyChange();
            }
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                NotifyOfPropertyChange();
            }
        }

        public int OrderCount => Orders.Cast<Order>().Count();

        public void ShowOrder(Order order)
        {
            ParentViewModel?.ShowOrder(order);
        }
    }
}
