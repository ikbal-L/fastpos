using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    public class TablesViewModel : PropertyChangedBase
    {
        public TablesViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            Tables = new BindableCollection<Table>();
            IsFullView = false;
        }
        public BindableCollection<Table> Tables { get; set; }
        public CheckoutViewModel Parent { get; set; }
        public bool IsFullView { get; set; }
    
        public void BackCommand()
        {
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
        }

        internal void AddIfNotExists(Table table)
        {
            if (!Tables.Any(t => t==table))
            {
                Tables.Add(table);
            }
        }

        internal void RemoveTable(Table table)
        {
            if (Tables == null || Tables.Count==0)
            {
                return;
            }
            Tables.Remove(table);
        }

        internal void RemoveOrder(Order currentOrder)
        {
            Table toRemove=null;
            foreach (var table in Tables)
            {
                var isremoved = false;
                var count = table.RemoveOrder(currentOrder, ref isremoved);
                if (count == 0 && isremoved)
                {
                    toRemove = table;
                }
            }
            RemoveTable(toRemove);
        }
    }
}
