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
        }
        public BindableCollection<Table> Tables { get; set; }
        public CheckoutViewModel Parent { get; set; }
    
        public void BackCommand()
        {
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
        }
    }
}
