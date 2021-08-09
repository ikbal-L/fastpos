﻿using System;
using System.Linq;
using FastPosFrontend.Enums;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;

namespace FastPosFrontend.ViewModels
{
    public class SplitViewModel : DialogContent
    {
        private bool _isDiscChecked;
        private Order _currentOrder;
        private Order _splitedOrder;
        CheckoutViewModel _parent;
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
            _parent = parent;
            SplittedOrder = new Order();
            CurrentOrder = new Order();
            if (_parent != null)
            {
                CopyCurrentOrderWithSeparationOfQuantities();
            }
            IsPayementChecked = true;
        }

        private void CopyCurrentOrderWithSeparationOfQuantities()
        {
            
            _parent.CurrentOrder.OrderItems.ToList().ForEach(
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
                                    CurrentOrder.AddOrderItem(oitem.Product, false, 1, false);
                                    qty--;
                                }
                                if (qty > 0)
                                {
                                    CurrentOrder.AddOrderItem(oitem.Product, false, qty, false);
                                }
                            }
                        }
                    );
        } 
        private void CopyBackToCurrentOrderWithGroupingOfQuantities()
        {
            _parent.CurrentOrder.OrderItems.Clear();
            
            CurrentOrder.OrderItems.ToList().ForEach(
                        oitem =>
                        {
                            _parent.CurrentOrder.AddOrderItem(oitem);
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
            if (!order.OrderItems.Any())
            {
                ToastNotification.Notify("Add Order items First!");
                return;
            }
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
                ToastNotification.Notify("New price less than the total price",NotificationType.Warning);
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
                ToastNotification.Notify("Discount bigger than total", NotificationType.Warning);
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
                ToastNotification.Notify("Payed Amount non valid", NotificationType.Warning);
                return;
            }
            if (payedAmount < 0)
            {
                NumericZone = "";
                return;
            }
            if (SplittedOrder == null)
            {
                ToastNotification.Notify("Add products before ...", NotificationType.Warning);
                return;
            }

            if (payedAmount < SplittedOrder.NewTotal)
            {
                //NumericZone = "";
                //Use Local to select message according to UI language
                ToastNotification.Notify("Payed amount lower than total", NotificationType.Warning);
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
            if (cmd == ActionButton.CopyToNumericZone)
            {
                NumericZone = SplittedOrder.NewTotal + "";
                return;
            }

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
                        if (!SplittedOrder.OrderItems.Any())
                        {
                            ToastNotification.Notify("Add items First!");
                            return;
                        }

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
                        if (!SplittedOrder.OrderItems.Any())
                        {
                            ToastNotification.Notify("Add items First!");
                            return;
                        }

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
                        if (!SplittedOrder.OrderItems.Any())
                        {
                            ToastNotification.Notify("Add items First!");
                            return;
                        }

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

                    if (!SplittedOrder.OrderItems.Any())
                    {
                        ToastNotification.Notify("Add items First!");
                        return;
                    }
                    
                    PayementAction();
                    
                    if (CurrentOrder.OrderItems.Count == 0)
                    {
                        _parent.RemoveCurrentOrderForOrdersList();
                        //BackFromSplitCommand();
                    }
                    break;
            }
        }






       #region Split Commands
        public void BackFromSplitCommand()
        {

            
            _parent.IsDialogOpen = false;
            _parent.DialogViewModel = null;
            CurrentOrder = null;
            if (_parent.CurrentOrder?.OrderItems!= null)
            {
                _parent.OrderItemsCollectionViewSource.Source = _parent.CurrentOrder.OrderItems;
            }

            Host.Close(this);
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

        private void RemoveOrderItems()
        {
            _parent.CurrentOrder.OrderItems.ToList().ForEach(item =>
            {
                var itemCount = SplittedOrder.OrderItems.Count(orderItem => orderItem.ProductId == item.ProductId);
                if (itemCount <= 0) return;
                if (itemCount < item.Quantity)
                {
                    item.Quantity -= itemCount;
                }
                else 
                {
                    _parent.CurrentOrder.OrderItems.Remove(item);
                }

            });
        }
        private void RemoveOrderItemsFromSplitViewOrder()
        {
            SplittedOrder.OrderItems.ToList().ForEach(item =>
            {
                var i = CurrentOrder.OrderItems.FirstOrDefault(oi => SplittedOrder.OrderItems.Any(spi=>spi.ProductId == oi.ProductId));
                CurrentOrder.OrderItems.Remove(i);
            });
        }
        private void SaveSplittedOrder()
        {
            bool resp1=false;
            if (_parent.CurrentOrder.Id == null)
            {
                 _parent.CurrentOrder.State = OrderState.Splitted;
                 var parentCurrentOrder = _parent.CurrentOrder;

                //TODO Revise Handling Order items of Split order
                resp1 = _parent.SaveOrder( ref parentCurrentOrder);
                 _parent.CurrentOrder = parentCurrentOrder;
                 _parent.NotifyOfPropertyChange(() => _parent.CurrentOrder);
            }
            switch (resp1|| _parent.CurrentOrder.Id != null)
            {
                case true:
                    _parent.CurrentOrder.State = OrderState.Splitted;
                    //Parent.CurrentOrder.OrderItems.RemoveRange(Parent.CurrentOrder.OrderItems
                    //    .Where(x => SplittedOrder.OrderItems.Any(i => i.ProductId == x.ProductId))
                    //    .ToList());
                    RemoveOrderItems();
                    
                    var parentCurrentOrder = _parent.CurrentOrder;
                    var resp = _parent.SaveOrder(ref parentCurrentOrder);
                    
                    if (resp)
                    {
<<<<<<< HEAD
                        SplittedOrder.SplittedFrom = _parent.CurrentOrder;
=======
                        SplittedOrder.SplittedFrom = Parent.CurrentOrder;
                        Parent?.SetSplitOrderNumber();
>>>>>>> 37cb1aeaa4e09e2418262597c5db17f97bc2e1b7

                        var resp2 = _parent.SaveOrder(ref _splitedOrder);
                        SplittedOrder = _splitedOrder;
                        NotifyOfPropertyChange(() => SplittedOrder);

                        switch (resp2)
                        {
                            case true:
                                
                                Parent?.PrintDocument(PrintSource.CheckoutSplit);
                                GivenAmount = SplittedOrder.GivenAmount;
                                ReturnedAmount = SplittedOrder.ReturnedAmount;
                                RemoveOrderItemsFromSplitViewOrder();
                                //TODO Fix Issue! Remove range not removing SplittedOrder.OrderItems from Parent.CurrentOrder.OrderItems

                                //SplittedOrder.OrderItems.Clear();
                                //SplittedOrder.NotifyOfPropertyChange(() => SplittedOrder.NewTotal);
                                //SplittedOrder.Id = null;
                                SplittedOrder = new Order();
                                NotifyOfPropertyChange(() => SplittedOrder);
                                SplittedOrder.NotifyOfPropertyChange(() => SplittedOrder.NewTotal);

                                break;


                            default:
                                //false
                                ToastNotification.ErrorNotification(0);
                                break;
                        }
                    }

                    break;


                default:
                    // false or null
                    ToastNotification.ErrorNotification(0);
                    break;
            }
            
        }
        #endregion


        

    }
}