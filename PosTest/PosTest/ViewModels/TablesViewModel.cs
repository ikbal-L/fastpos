using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class TablesViewModel : PropertyChangedBase
    {
        private Table _selectedTable;

        public TablesViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            IsFullView = false;
            TablesViewSource = new CollectionViewSource();
            TablesViewSource.Source = Parent.Tables;
            TablesViewSource.Filter += TablesFilter;
            //TablesView = TablesViewSource.View;
            TablesView = CollectionViewSource.GetDefaultView(Parent.Tables);
            TablesView.SortDescriptions.Add(new SortDescription("Number",ListSortDirection.Ascending));
            TablesView.CollectionChanged += TablesViewChanged;
            //TablesView.CurrentChanged += TablesViewCurrentChanged;
            Tables = new BindableCollection<Table>(TablesView.Cast<Table>());
        }

        //private void TablesViewCurrentChanged(object sender, EventArgs e)
        //{

        //}

        private void TablesViewChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Table table in TablesView)
            {
                if (!Tables.Contains(table))
                {
                    Tables.Add(table);
                }
            }
            if (TablesView.Cast<Table>().Count() != Tables.Count)
            {
                List<Table> toRemove = new List<Table>();
                foreach (var table in Tables)
                {
                    if (!TablesView.Contains(table))
                    {
                        toRemove.Add(table);
                    }
                }
                Tables.RemoveRange(toRemove);
            }
            NotifyOfPropertyChange(() => OrderCount);

        }

        public void TablesFilter(object sender, FilterEventArgs e)
        {
            Table table = e.Item as Table;
            if (table != null)
            {
                // Filter out products with price 25 or above
                if (table.Orders.Cast<Order>().Count() > 0)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        public void RefreshTables()
        {
            //foreach (Table table in TablesView)
            //{
            //    if (!Tables.Contains(table))
            //    {
            //        Tables.Add(table);
            //    }
            //}
            //if (TablesView.Cast<Table>().Count() != Tables.Count)
            //{
            //    List<Table> toRemove = new List<Table>();
            //    foreach (var table in Tables)
            //    {
            //        if (!TablesView.Contains(table))
            //        {
            //            toRemove.Add(table);
            //        }
            //    }
            //    Tables.RemoveRange(toRemove);
            //}
        }
        public CollectionViewSource TablesViewSource { get; set; }

        public ICollectionView TablesView { get; set; }
        public BindableCollection<Table> Tables { get; set; }
        public CheckoutViewModel Parent { get; set; }
        public bool IsFullView { get; set; }
        public int OrderCount 
        {
            get
            {
                var count = 0;
                foreach (Table table in TablesView)
                {
                    count += table.Orders.Cast<Order>().Count();
                }
                return count;
            }
        }

        public Table SelectedTable 
        { 
            get => _selectedTable;
            set 
            {
                _selectedTable = value;
                NotifyOfPropertyChange();
            }
        }


        public void BackCommand()
        {
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
        }

        public void OrderSelectionChanged(Order order)
        {

        }
    }
}
