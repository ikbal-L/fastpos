using Caliburn.Micro;
using System.Collections.Generic;
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
        

        public Table()
        {
            
            
        }
        public Table(IEnumerable<Order> orders)
        {
            OrderViewSource = new CollectionViewSource();
            OrderViewSource.Source = orders;
            OrderViewSource.Filter += TableOrderFilter;
            Orders = OrderViewSource.View;// CollectionViewSource.GetDefaultView(Parent.Orders);
            OrderViewSource.Filter += (s, e) =>
            {
                Order order = e.Item as Order;
                if (order != null)
                {
                    if (order.Table == this)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
            };
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
                OrderViewSource = new CollectionViewSource();
                OrderViewSource.Source = _allOrders;
                OrderViewSource.Filter += TableOrderFilter;
                Orders = OrderViewSource.View;// CollectionViewSource.GetDefaultView(Parent.Orders);
            }
        }
        public void TableOrderFilter(object sender, FilterEventArgs e)
        {
            Order order = e.Item as Order;
            if (order != null)
            {
                // Filter out products with price 25 or above
                if (order.Table == this)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
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

        public int RemoveOrder(Order order, ref bool isRemoved)
        {
            if (TableOrders == null)
            {
                return 0;
            }
            isRemoved = TableOrders.Remove(order);
            return TableOrders.Count;
        }
    }
}
