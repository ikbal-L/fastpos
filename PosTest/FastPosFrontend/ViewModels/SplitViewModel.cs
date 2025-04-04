﻿using System;
using System.Linq;
using FastPosFrontend.Enums;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service.StateManager;

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
            
            _parent.CurrentOrder.OrderItems.Where(oi=>oi.State!= OrderItemState.Removed).ToList().ForEach(
                        oitem =>
                        {
                            if (oitem.Quantity == 1)
                            {
                                CurrentOrder.AddOrderItem(oitem);
                            }
                            else
                            {
                                float qty = oitem.Quantity;
                                while (qty > 0)
                                {
                                    CurrentOrder.AddOrderItem(oitem.Product, false, 1, false);
                                    qty--;
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

            if (!decimal.TryParse(NumericZone,out var payedAmount))
            {
                ToastNotification.Notify("Payed Amount non valid", NotificationType.Warning);
                NumericZone = "";
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

                ToastNotification.Notify("Payed amount lower than total", NotificationType.Warning);

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







        public void BackFromSplitCommand()
        {

            
            _parent.CanSplitOrder = false;
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

            var canSave = _parent.CurrentOrder.Id.HasValue || StateManager.Save(_parent.CurrentOrder);

            if (canSave)
            {
                _parent.CurrentOrder.State = OrderState.Splitted;

                RemoveOrderItems();



                if (StateManager.Save(_parent.CurrentOrder))
                {
                    SplittedOrder.SplittedFrom = _parent.CurrentOrder;



                    if (StateManager.Save(SplittedOrder))
                    {
                        _parent?.PrintDocument(PrintSource.CheckoutSplit);
                        GivenAmount = SplittedOrder.GivenAmount;
                        ReturnedAmount = SplittedOrder.ReturnedAmount;
                        RemoveOrderItemsFromSplitViewOrder();
                        SplittedOrder = new Order();
                        SplittedOrder.NotifyOfPropertyChange(() => SplittedOrder.NewTotal);
                    }

                }

            }



        }

    }
}