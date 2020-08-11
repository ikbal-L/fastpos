﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Table : PropertyChangedBase
    {
        private int _number;
        private IEnumerable<Order> _allOrders;

        public Table()
        {

        }
        public Table(IEnumerable<Order> orders)
        {
            OrderViewSource = new CollectionViewSource();
            OrderViewSource.Source = orders;
            OrderViewSource.Filter += TableOrderFilter;
            Orders = OrderViewSource.View;// CollectionViewSource.GetDefaultView(Parent.Orders);
        }

        [DataMember]
        public long?  Id { get; set; }

        [DataMember]
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

        public CollectionViewSource OrderViewSource { get; set; }

        public ICollectionView Orders
        {
            get;
            set;
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
