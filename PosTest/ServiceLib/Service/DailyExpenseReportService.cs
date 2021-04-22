using System;
using System.Collections.Generic;
using System.Linq;
using ServiceInterface.Model;

namespace ServiceLib.Service
{
    public static class DailyExpenseReportService
    {
        public static DailyExpenseReport GenerateDailyExpenseReport(this IEnumerable<Order> orders,decimal cashRegisterInitialAmount)
        {
            var ordersOfTheDay = orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
            var cashPayments = new Dictionary<string, decimal>();
            var deliveryPayments = new Dictionary<string, decimal>();
            var payedOrders = ordersOfTheDay.Where(o => o.Type != OrderType.Delivery && o.State == OrderState.Payed)
                .ToList();
            var deliveryOrders = ordersOfTheDay
                .Where(o => o.Type == OrderType.Delivery && o.State == OrderState.Delivered).ToList();
            payedOrders.ForEach(o => { cashPayments.Add($"{o.Id}", o.NewTotal); });
            deliveryOrders.ForEach(o => { deliveryPayments.Add($"{o.Id}", o.NewTotal); });
            var cashRegisterDepositedAmount = payedOrders.Select(o => o.GivenAmount).Sum();
            var cashRegisterWithDrawnAmount = payedOrders.Select(o => o.ReturnedAmount).Sum();
            var cashRegisterExpectedAmount =
                cashRegisterInitialAmount + 
                cashRegisterDepositedAmount - 
                cashRegisterWithDrawnAmount; //- expenses
            var report = new DailyExpenseReport()
            {
                IssuedDate = DateTime.Today,
                CashPayments = cashPayments,
                DeliveryPayments = deliveryPayments,
                CashRegisterInitialAmount = cashRegisterInitialAmount,
                CashRegisterDepositedAmount = cashRegisterDepositedAmount,
                CashRegisterWithDrawnAmount = cashRegisterWithDrawnAmount,
                CashRegisterExpectedAmount = cashRegisterExpectedAmount

            };

            return report;
        }
    }
}