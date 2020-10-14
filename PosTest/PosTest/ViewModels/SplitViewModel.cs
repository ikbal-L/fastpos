using Caliburn.Micro;
using System;
using System.Linq;

using ServiceInterface.Model;
using ServiceInterface.StaticValues;

namespace PosTest.ViewModels
{
    public class SplitViewModel : PropertyChangedBase
    {
        private bool _isDiscChecked;
        private Order _currentOrder;
        private Order _splitedOrder;
        CheckoutViewModel Parent;
        private string _numericZone;
        private bool _isItemPriceChecked;
        private bool _isTotalPriceChecked;
        private bool _commandSwitched;
        private bool _isPercentKeyAllowed;
        private bool _isPayementChecked;

        private decimal givenAmount;
        private decimal returnedAmount;

        public SplitViewModel()
        {

        }

        public decimal GivenAmount
        {
            get=> givenAmount;
            set {
                givenAmount = value;
                NotifyOfPropertyChange(() => GivenAmount); }
        }

        
        public decimal ReturnedAmount
        {
            get => returnedAmount;
            set { returnedAmount = value; NotifyOfPropertyChange(() => ReturnedAmount); }
        }


        public SplitViewModel(CheckoutViewModel parent)
        {
            Parent = parent;
            SplittedOrder = new Order();
            CurrentOrder = new Order();
            if (Parent != null)
            {
                CopyCurrentOrderWithSeparationOfQuantities();
            }
            IsPayementChecked = true;
        }

        private void CopyCurrentOrderWithSeparationOfQuantities()
        {
            
            Parent.CurrentOrder.OrderItems.ToList().ForEach(
                        oitem =>
                        {
                            if (oitem.Quantity <= 1)
                            {
                                CurrentOrder.AddOrderItem(oitem);
                            }
                            else
                            {
                                float qty = oitem.Quantity;
                                while (qty >= 1)
                                {
                                    CurrentOrder.AddOrderItem(oitem.Product, oitem.UnitPrice, false, 1, false);
                                    qty--;
                                }
                                if (qty > 0)
                                {
                                    CurrentOrder.AddOrderItem(oitem.Product, oitem.UnitPrice, false, qty, false);
                                }
                            }
                        }
                    );
        } 
        private void CopyBackToCurrentOrderWithGroupingOfQuantities()
        {
            Parent.CurrentOrder.OrderItems.Clear();
            
            CurrentOrder.OrderItems.ToList().ForEach(
                        oitem =>
                        {
                            Parent.CurrentOrder.AddOrderItem(oitem);
                        }
                    );
        }

        #region Properties
        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;
                NotifyOfPropertyChange(() => CurrentOrder);

            }
        }

        public Order SplittedOrder
        {
            get { return _splitedOrder; }
            set
            {
                _splitedOrder = value;
                NotifyOfPropertyChange(() => SplittedOrder);
            }
        }

        public string NumericZone
        {
            get => _numericZone;
            set
            {
                _numericZone = value;
                //if (!CommandSwitched)
                //{
                //    KeypadEnteredValueChanged(ref _numericZone);
                //}
                NotifyOfPropertyChange();
            }
        }
        public bool IsItemPriceChecked
        {
            get => _isItemPriceChecked;
            set
            {
                _isItemPriceChecked = _isItemPriceChecked == true ? false : value;
                CommandSwitched = true;
                NotifyOfPropertyChange();
            }
        }
        public bool IsTotalPriceChecked
        {
            get => _isTotalPriceChecked;
            set
            {
                _isTotalPriceChecked = _isTotalPriceChecked == true ? false : value;
                CommandSwitched = true;
                NotifyOfPropertyChange();
            }
        }

        public bool IsDiscChecked
        {
            get => _isDiscChecked;
            set
            {
                _isDiscChecked = _isDiscChecked == true ? false : value;
                CommandSwitched = true;
                NotifyOfPropertyChange();
            }
        }

        public bool IsPayementChecked
        {
            get => _isPayementChecked;
            set
            {
                _isPayementChecked = _isPayementChecked == true ? false : value; ;
                CommandSwitched = true;
                NotifyOfPropertyChange();
            }
        }

        public bool CommandSwitched
        {
            get => _commandSwitched;
            set
            {
                _commandSwitched = value;
                NotifyOfPropertyChange();
            }
        }        

        public bool IsPercentKeyAllowed
        {
            get => _isPercentKeyAllowed;
            set
            {
                _isPercentKeyAllowed = value;
                NotifyOfPropertyChange();
            }
        }
        #endregion
        public void KeypadEnteredValueChanged(ref string numericZone)
        {
            if (IsItemPriceChecked)
            {
                return;
            }
            if (_isTotalPriceChecked)
            {
                PriceAction(ref numericZone, SplittedOrder);
                return;
            }
            if (IsDiscChecked)
            {
                DiscAction(ref numericZone, SplittedOrder);
                return;
            }
        }

        public static void PriceAction(ref string priceStr, Order order)
        {
            decimal price;
            if (priceStr == "")
            {
                order.DiscountAmount = order.Total;
                return;
            }
            try
            {
                price = Convert.ToDecimal(priceStr);
            }
            catch (Exception)
            {
                priceStr = "";
                return;
            }
            if (price < 0)
            {
                priceStr = "";
                return;
            }
            //CurrentOrder.NewTotal
            var newTotal = price;
            if (newTotal <= order.Total || ActionConfig.AllowNewTotalGreaterThanToal)
            {
                order.DiscountAmount = order.Total - newTotal;// CurrentOrder.NewTotal;
                order.DiscountPercentage = order.DiscountAmount * 100 / order.Total;
                
            }
            else
            {
                priceStr = priceStr.Remove(priceStr.Length - 1, 1);
                ToastNotification.Notify("New price less than the total price");
            }
            priceStr = "";

        }
        public  void DiscAction(ref string discStr, Order order)
        {
            var discountPercent = 0m;
            var discount = 0m;
            var isPercentage = false;

            if (discStr.Contains("%"))
            {
                isPercentage = true;
                if (discStr.Remove(discStr.Length - 1) == "")
                {
                    return;
                }
                discountPercent = Convert.ToDecimal(discStr.Remove(discStr.Length - 1));
                if (discountPercent > 100)
                {
                    discStr = string.Empty;
                    return;
                }
                discount = order.NewTotal * discountPercent / 100;
            }
            else
            {
                try
                {
                    discount = Convert.ToDecimal(discStr);

                }
                catch (Exception)
                {

                    discount = 0;
                }
            }
            if (discount < 0)
            {
                discStr = "";
                return;
            }

            if (discount > order.Total)
            {
                discStr = discStr.Remove(discStr.Length - 1, 1);
                ToastNotification.Notify("Discount bigger than total");
                return;
            }
            if (!isPercentage)
            {
                order.DiscountAmount = discount;
            }
            else
            {
                order.DiscountPercentage = discountPercent;
            }
        }

        public void PayementAction()
        {
            decimal payedAmount;
            try
            {
                payedAmount = Convert.ToDecimal(NumericZone);

            }
            catch (Exception)
            {
                ToastNotification.Notify("Payed Amoount non valid");
                return;
            }
            if (payedAmount < 0)
            {
                NumericZone = "";
                return;
            }
            if (SplittedOrder == null)
            {
                ToastNotification.Notify("Add products before ...");
                return;
            }

            if (payedAmount < SplittedOrder.NewTotal)
            {
                //NumericZone = "";
                //Use Local to select message according to UI language
                ToastNotification.Notify("Payed amount lower than total");
                //CurrentOrder.DiscountAmount = 0;
                return;
            }

            SplittedOrder.GivenAmount = payedAmount;
            SplittedOrder.ReturnedAmount = SplittedOrder.NewTotal - payedAmount;
            SplittedOrder.State = OrderState.Payed;
            SaveSplittedOrder();
            NumericZone = "";
        }

        public void ActionKeyboard(ActionButton cmd)
        {
            if (string.IsNullOrEmpty(NumericZone)) return;
            switch (cmd)
            {
                case ActionButton.Price:
                    {
                        //if (IsTotalPriceChecked)
                        //{
                        //    IsPercentKeyAllowed = false;
                        //    NumericZone = SplittedOrder?.NewTotal.ToString();

                        //}
                        //else
                        //{
                        //    NumericZone = "";
                        //}
                        
                        if (!NumericZone.Contains("%"))
                        {
                            PriceAction(ref _numericZone, SplittedOrder); 
                        }
                        else
                        {
                            DiscAction(ref _numericZone,SplittedOrder);
                        }
                            NumericZone = _numericZone;
                        break;
                    }

                case ActionButton.Disc:
                    {
                        if (IsDiscChecked)
                        {
                            IsPercentKeyAllowed = true;
                            NumericZone = SplittedOrder?.DiscountAmount.ToString();
                         }
                        else
                        {
                            NumericZone = "";
                        }
                        break;
                    }
                case ActionButton.Itemprice:
                    {
                        if (IsItemPriceChecked)
                        {
                            IsPercentKeyAllowed = false;
                            NumericZone = SplittedOrder?.SelectedOrderItem?.UnitPrice.ToString();
                        }
                        else
                        {
                            NumericZone = "";
                        }
                        break;
                    }
                case ActionButton.Payment:
                    
                    
                    
                    PayementAction();
                    GivenAmount = SplittedOrder.GivenAmount;
                    ReturnedAmount = SplittedOrder.ReturnedAmount;
                    if (CurrentOrder.OrderItems.Count == 0)
                    {
                        Parent.RemoveCurrentOrderForOrdersList();
                        //BackFromSplitCommand();
                    }
                    break;
            }
        }






       #region Split Commands
        public void BackFromSplitCommand()
        {

            //if (SplitedOrder == null)
            //{
            //    Parent.IsDialogOpen = false;
            //    return;
            //}
            //if (SplitedOrder.OrderItems == null)
            //{
            //    Parent.IsDialogOpen = false;
            //    return;
            //}
            //if (SplitedOrder.OrderItems.Count() == 0)
            //{
            //    Parent.IsDialogOpen = false;
            //    return;
            //}
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
            if (Parent.CurrentOrder != null)
            {
                CopyBackToCurrentOrderWithGroupingOfQuantities();
            }
            
        }

        public void AddSplittedItemsCommand()
        {
            if (SplittedOrder is null) ;
        }

        public void ClearSplittedOrder()
        {
            if (SplittedOrder != null && SplittedOrder.OrderItems != null)
            {
                SplittedOrder.OrderItems.Clear();
                CurrentOrder.SelectedOrderItem = null;
            }
        }

        private void SaveSplittedOrder()
        {
            int? resp1=200;
            if (Parent.CurrentOrder.Id == null)
            {
                 Parent.CurrentOrder.State = OrderState.Splitted;
                 resp1 = Parent.SaveOrder(Parent.CurrentOrder);
            }
            switch (resp1)
            {
                case 200:
                    SplittedOrder.SplittedFrom = Parent.CurrentOrder;
                    var resp2 = Parent.SaveOrder(SplittedOrder);

                    switch (resp2)
                    {
                        case 200:
                            CurrentOrder.OrderItems.RemoveRange(SplittedOrder.OrderItems);
                            Parent.CurrentOrder.OrderItems.RemoveRange(SplittedOrder.OrderItems);
                            SplittedOrder.OrderItems.Clear();
                            SplittedOrder.Id = null;
                            break;

                        case null: break;

                        default:
                            ToastNotification.ErrorNotification((int)resp2);
                            break;
                    }

                    break;

                case null: break;

                default:
                    ToastNotification.ErrorNotification((int)resp1);
                    break;
            }
            
        }
        #endregion

    }
}