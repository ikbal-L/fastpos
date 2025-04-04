﻿using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels
{
    public class OpenTableViewModel: Screen
    {
        private Order _selectedOrder;

        public ICollection<Order> TableOrders { get;set;}

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                NotifyOfPropertyChange(() => SelectedOrder);
            }
        }

        public OpenTableViewModel(IList<Order> orders, ServiceInterface.Model.Table table)
        {
            TableOrders = orders.Where<Order>(o => o.Table.Id == table.Id).ToList();
        }


        public void OpenSelectedCommand()
        {
            //Action To Return Chechout View
            throw new NotImplementedException();
        }

    }
}
