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
        }
        public BindableCollection<Table> Tables { get; set; }
        public CheckoutViewModel Parent { get; set; }
    
        public void BackCommand()
        {
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
        }

        internal void Add(Table table)
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
    }
}
