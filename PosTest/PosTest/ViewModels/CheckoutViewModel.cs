using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceInterface.Model;

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {
        public BindableCollection<Product> Products { get; set; }

        public void Close()
        {
            LoginViewModel toActivateViewModel = new LoginViewModel();
            toActivateViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(toActivateViewModel);
        }
    }
}
