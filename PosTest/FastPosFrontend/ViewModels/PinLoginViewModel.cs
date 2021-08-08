using System.Windows.Controls;
using Caliburn.Micro;
using ServiceInterface.Interface;

namespace FastPosFrontend.ViewModels
{
    public class PinLoginViewModel : Screen
    {
        private string _pinCode;
        private readonly IProductService _productsService;
        private readonly ICategoryService _categoriesService;
        private readonly IOrderService _orderService;
        private readonly IWaiterService _waiterService;
        private readonly IDelivereyService _delivereyService;
        private readonly ICustomerService _customerService;

        public PinLoginViewModel(
            //IProductService productsService,
            //ICategoryService categoriesService,
            //IOrderService orderService,
            //IWaiterService waiterService,
            //IDelivereyService delivereyService,
            //ICustomerService customerService
            )
        {
            
            //_productsService = productsService;
            //_categoriesService = categoriesService;
            //_orderService = orderService;
            //_waiterService = waiterService;
            //_delivereyService = delivereyService;
            //_customerService = customerService;
        }

        public void PinKeyPadAction(object sender)
        {
            string cmd = (string)(sender as Button).Tag;
            switch (cmd)
            {
                case "0":
                    PinCode += "0";
                    
                    break;
                case "1":
                    PinCode += "1";
                    break;
                case "2":
                    PinCode += "2";
                    break;
                case "3":
                    PinCode += "3";
                    break;
                case "4":
                    PinCode += "4";
                    break;
                case "5":
                    PinCode += "5";
                    break;
                case "6":
                    PinCode += "6";
                    break;
                case "7":
                    PinCode += "7";
                    break;
                case "8":
                    PinCode += "8";
                    break;
                case "9":
                    PinCode += "9";
                    break;
                case "enter": 
                        Login();
                    break;
                case "backspace":
                    PinCode =PinCode?.Remove(PinCode.Length - 1, 1); 
                    break;
            }
        }

        public string PinCode
        {
            get => _pinCode;
            set => Set(ref _pinCode, value);
        }

        public void Login()
        {
            Checkout();
        }

        public void Checkout()
        {
            //IsDialogOpen = false;
            CheckoutViewModel checkoutViewModel =
                new CheckoutViewModel(
                );

            checkoutViewModel.Parent = Parent;
            (Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }

        
    }

    
}
