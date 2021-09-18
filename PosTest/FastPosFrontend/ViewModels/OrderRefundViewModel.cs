using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using Parser = FastPosFrontend.Helpers.Parser;

namespace FastPosFrontend.ViewModels
{
    public class OrderRefundViewModel:PropertyChangedBase
    {
        private readonly Parser _parser;
        public OrderRefundViewModel(CheckoutViewModel parent)
        {
            _parser = Parser.Instance;
            var repo = StateManager.GetService<Order, IOrderRepository>();
            Dictionary<string, string> criterias = new Dictionary<string, string>();
           
            //criterias.Add(nameof(Order.OrderTime), DateTime.Today.ToString("yyyy-MM-dd'T'HH:mm:ss"));
            //criterias.Add(nameof(Order.State), OrderState.Payed.ToString());

            var criteria = new OrderFilter() { OrderTime = DateTime.Today, State = OrderState.Payed };
            var paid = repo.GetByCriterias(criteria);
            _paidOrdersOfTheDay = new ObservableCollection<Order>(paid);
            PaidOrdersOfTheDay = new CollectionViewSource() { Source = _paidOrdersOfTheDay};
            PaidOrdersOfTheDay.Filter += PaidOrdersOfTheDay_Filter;
            Parent = parent;
            FilterCommand = new DelegateCommandBase(ApplySearchFilter);
            RefundOrderCommand = new DelegateCommandBase(RefundOrder,CanRefundOrder);
        }

        private void PaidOrdersOfTheDay_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = FilterPaidOrder(e.Item);
            return;
        }

        private Order _selectedPaidOrder;

        public Order SelectedPaidOrder
        {
            get { return _selectedPaidOrder; }
            set 
            {
                Set(ref _selectedPaidOrder, value);
                (RefundOrderCommand as DelegateCommandBase).RaiseCanExecuteChanged();
            }
        }


        public ObservableCollection<Order> _paidOrdersOfTheDay { get; set; }

        public CollectionViewSource PaidOrdersOfTheDay { get; set; }


        private string _selectedCriteria = OrderFilterCriteria.PRICE;

        public string SelectedCriteria
        {
            get { return _selectedCriteria; }
            set { Set(ref _selectedCriteria,value); }
        }

        private string _filterText;

        public string FilterText
        {
            get { return _filterText; }
            set { Set(ref _filterText, value); }
        }

        public ICommand FilterCommand { get; set; }

        public ICommand RefundOrderCommand { get; set; }

        private CheckoutViewModel _parent;

        public CheckoutViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        public void AddPaidOrder(Order order)
        {
            _paidOrdersOfTheDay.Add(order);
            PaidOrdersOfTheDay.View.Refresh();
        }

        public void RefundOrder(object o)
        {
            SelectedPaidOrder.State = OrderState.Refunded;
            StateManager.Save(SelectedPaidOrder);
            _paidOrdersOfTheDay.Remove(SelectedPaidOrder);
            SelectedPaidOrder = null;
        }

        public bool CanRefundOrder(object o) => SelectedPaidOrder != null;

        public void ApplySearchFilter( object o)
        {
            PaidOrdersOfTheDay.View.Refresh();
        }

        public bool FilterPaidOrder(object obj)
        {
            if (obj is Order order)
            {
                if (string.IsNullOrEmpty(FilterText) || string.IsNullOrWhiteSpace(FilterText)) return true;

                if (SelectedCriteria == OrderFilterCriteria.PRICE)
                {
                    
                   var  (min, max, equivalent) = _parser.ParseDecimalValues(FilterText);

                    return FilterByPrice(order.NewTotal,min:min,max:max, equivalent: equivalent);
                }

                if (SelectedCriteria == OrderFilterCriteria.ITEM_COUNT)
                {

                    var (min, max, equivalent) = _parser.ParseIntegerValues(FilterText);

                    return FilterByItemCount(order.OrderItems.Count, min: min, max: max, equivalent: equivalent);
                }

                if (SelectedCriteria == OrderFilterCriteria.TIME)
                {
                    var (min, max, equivalent) = _parser.ParseTimeSpanValues(FilterText);
                    var time = TimeSpan.Parse(order.OrderTime.TimeOfDay.ToString(@"hh\:mm"));
                    return FilterByTime(time, min,max,equivalent);
                }
            }

            return false;
        } 


        public bool FilterByTime(TimeSpan time, TimeSpan? start = null, TimeSpan? end = null , TimeSpan? exact = null)
        {
            if ( start.HasValue && end.HasValue) return time > start && time < end;

            if (!start.HasValue && !end.HasValue && exact.HasValue) return time == exact;

            if (!start.HasValue && !end.HasValue) return true;

            if (!start.HasValue) return time < end;
           
            if (!end.HasValue) return time > start;

            return false;

        }

        public bool FilterByPrice(decimal price, decimal? min = null, decimal? max = null,decimal? equivalent = null)
        {
            if (min.HasValue && max.HasValue) return price > min && price < max;

            if (!min.HasValue && !max.HasValue && equivalent.HasValue) return price == equivalent;

            if (!min.HasValue && !max.HasValue ) return true;

            if (!min.HasValue) return price < max;

            if (!max.HasValue) return price > min;

            return false;
        }

        public bool FilterByItemCount(int itemCount, int? min, int? max,int? equivalent = null)
        {
            if (min.HasValue && max.HasValue) return itemCount > min && itemCount < max;

            if (!min.HasValue && !max.HasValue && equivalent.HasValue) return itemCount == equivalent;
            if (!min.HasValue && !max.HasValue) return true;

            if (!min.HasValue) return itemCount < max;

            if (!max.HasValue) return itemCount > min;

            return false;

        }

    }
}
