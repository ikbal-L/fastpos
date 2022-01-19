using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using Action = System.Action;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Table : PropertyChangedBase,IState<long>
    {
        private int _number;
        private IEnumerable<Order> _allOrders;
        private Order _selectedOrder;
        private ObservableCollection<Order> _orders;


        
        public Table(IEnumerable<Order> orders = null)
        {
            _orders = new ObservableCollection<Order>(orders??Array.Empty<Order>() );
            OrderViewSource = new CollectionViewSource() { Source = _orders };
            OrderViewSource.Filter += (_, e) => 
            e.Accepted = e.Item is Order order && order.Table == this;
            Orders = OrderViewSource.View;
        }

        [DataMember]
        public long?  Id { get; set; }

        [DataMember]
        [Required]
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                NotifyOfPropertyChange(()=> Number);
            }
        }

        [DataMember]
        public int Seats { get; set; }

        public Place Place { get; set; }

        [DataMember]
        public Place PlaceId { get; set; }

        [DataMember]
        public bool IsVirtual { get; set; } = false;

        public BindableCollection<Order> TableOrders { get; set; }
        public BindableCollection<Table> AllTables { get; set; }

        public CollectionViewSource OrderViewSource { get; set; }

        public ICollectionView Orders
        {
            get;
            set;
        }

        public Action SelectionChanged { get; set; }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                NotifyOfPropertyChange();
            }
        }

        public int OrdersCount
        {
            get => Orders.Cast<Order>().Count();
        }

        public IEnumerable<Order> AllOrders
        { 
            get => _allOrders;
            set
            {
                _allOrders = value;
                _orders = new ObservableCollection<Order>(_allOrders);
                OrderViewSource.Source = _orders;
                Orders = OrderViewSource.View;
                Orders.Refresh();
            }
        }

        public void AddOrder(Order order)
        {
            if (TableOrders == null)
            {
                TableOrders = new BindableCollection<Order>();
            }
            TableOrders.Add(order);
        }
    }
}
