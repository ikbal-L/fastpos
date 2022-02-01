using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels
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

            TablesView = CollectionViewSource.GetDefaultView(Parent.Tables);
            TablesView.SortDescriptions.Add(new SortDescription("Number",ListSortDirection.Ascending));
            TablesView.CollectionChanged += TablesViewChanged;

            Tables = new BindableCollection<Table>(TablesView.Cast<Table>());
        }



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
                if (table.Orders!= null&& !table.Orders.IsEmpty)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
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
                    if (table.Orders!= null)
                    {
                        count += table.Orders.Cast<Order>().Count(); 
                    }
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
            Parent.CanSplitOrder = false;
            Parent.DialogViewModel = null;
        }

        public void ShowOrder(Order order)
        {
            Parent?.ShowOrder(order);
        }
    }
}
