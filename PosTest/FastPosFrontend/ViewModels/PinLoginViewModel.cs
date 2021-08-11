using System.Windows.Controls;
using Caliburn.Micro;
using ServiceInterface.Interface;

namespace FastPosFrontend.ViewModels
{
    public class PinLoginViewModel : Screen
    {
        private string _pinCode;
        
        public PinLoginViewModel(
      
            )
        {
            
      
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
