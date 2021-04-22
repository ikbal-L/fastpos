using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class DailyExpenseReportServiceTest
    {
        private readonly ICollection<Order> _orders;
        private readonly Authentification _authService = new Authentification();

        public DailyExpenseReportServiceTest()
        {
            var resp = _authService.Authenticate("admin", "admin", new Terminal {Id = 1}, new Annex {Id = 1});
            var orderRepo = new OrderBaseRepository();

            _orders = orderRepo.Get().state.ToList();
        }

        [Fact]
        public void CreateReport()
        {
            var data = new
            {
                CashRegisterInitialAmount = 5000m,
                CashRegisterActualAmount = 5700,
                Expenses = Array.Empty<decimal>()
            };
            var api = new RestApis();
            var result =
                GenericRest.PostThing<DailyExpenseReport>(api.Action("dailyExpenseReport", EndPoint.Save), data);
            Assert.Equal(200, result.status);
            Assert.NotNull(result.Item2);
        }


        [Fact]
        public void OrdersNotNull()
        {
            Assert.NotNull(_orders);
        }

        [Fact]
        public void OrdersNotEmpty()
        {
            Assert.NotEmpty(_orders);
        }

        /// <remarks>
        /// For the tests below to succeed create an order which has today's date 
        /// </remarks>>
        [Fact]
        public void
            GenerateDailyExpenseReport_OrdersNotNull_DailyExpenseReportCashPaymentsCountEqualsPayedOrdersOfTheDayCount()
        {
            var report = _orders.GenerateDailyExpenseReport(5000);

            var ordersOfTheDay = _orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
            var payedOrdersOfTheDay =
                ordersOfTheDay.Where(o => o.Type != OrderType.Delivery && o.State == OrderState.Payed);
            Assert.Equal(payedOrdersOfTheDay.Count(), report.CashPayments.Count);
            Assert.NotEqual(0, report.CashPayments.Count);
        }

        [Fact]
        public void
            GenerateDailyExpenseReport_OrdersNotNull_DailyExpenseReportDeliveryPaymentsCountEqualsDeliveredOrdersOfTheDayCount()
        {
            var report = _orders.GenerateDailyExpenseReport(5000);

            var ordersOfTheDay = _orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
            var deliveredOrdersOfTheDay =
                ordersOfTheDay.Where(o => o.Type == OrderType.Delivery && o.State == OrderState.Delivered);
            Assert.Equal(report.DeliveryPayments.Count, deliveredOrdersOfTheDay.Count());
            Assert.NotEqual(0, report.DeliveryPayments.Count);
        }

        [Fact]
        public void
            GenerateDailyExpenseReport_OrdersNotNull_DailyExpenseReportCashRegisterDepositedAmountEqualsSumOfPayedOrdersGivenAmount()
        {
            var report = _orders.GenerateDailyExpenseReport(5000);

            var ordersOfTheDay = _orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
            var payedOrdersOfTheDay =
                ordersOfTheDay.Where(o => o.Type != OrderType.Delivery && o.State == OrderState.Payed);
            Assert.Equal(report.CashRegisterDepositedAmount, payedOrdersOfTheDay.Sum(o => o.GivenAmount));
            Assert.NotEqual(0, report.CashRegisterDepositedAmount);
        }

        [Fact]
        public void
            GenerateDailyExpenseReport_OrdersNotNull_DailyExpenseReportCashRegisterWithDrawnAmountEqualsSumOfPayedOrdersReturnedAmount()
        {
            var report = _orders.GenerateDailyExpenseReport(5000);

            var ordersOfTheDay = _orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
            var payedOrdersOfTheDay =
                ordersOfTheDay.Where(o => o.Type != OrderType.Delivery && o.State == OrderState.Payed);
            Assert.Equal(report.CashRegisterWithDrawnAmount, payedOrdersOfTheDay.Sum(o => o.ReturnedAmount));
            Assert.NotEqual(0, report.CashRegisterWithDrawnAmount);
        }
    }
}