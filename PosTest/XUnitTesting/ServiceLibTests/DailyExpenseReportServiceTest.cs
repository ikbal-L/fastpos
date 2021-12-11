using System;
using System.Collections.Generic;
using System.Linq;
using FastPosFrontend.ViewModels;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class DailyExpenseReportServiceTest
    {
        private readonly ICollection<Order> _orders;
        private readonly IAuthentification _authService = new RestAuthentification();

        public DailyExpenseReportServiceTest()
        {
            var resp = _authService.Authenticate("admin", "admin", new Terminal {Id = 1}, new Annex {Id = 1});
            var orderRepo = new OrderRepository();

            _orders = orderRepo.GetAll().state.ToList();
        }

        [Fact]
        public void CreateReport()
        {
            var data = new
            {
                CashRegisterInitialAmount = 5000m,
                CashRegisterActualAmount = 5330m,
                Expenses = Array.Empty<decimal>()
            };
            var d = new DailyExpenseReportInputData()
            {
                CashRegisterActualAmount = 5030,
                CashRegisterInitialAmount = 5000,
                Expenses = new Dictionary<string, decimal>()
                {
                    {"Buy x",100 },
                    {"Buy y",100 },
                    {"Buy z",100 },
                }
            };
            
            var api = new RestApi();
            var result =
                GenericRest.PostThing<DailyEarningsReport>(api.Action("dailyExpenseReport", EndPoint.SAVE), d);
            Assert.Equal(201, result.status);
            Assert.NotNull(result.Item2);
            Assert.Equal(d.CashRegisterActualAmount,result.Item2.CashRegisterExpectedAmount);
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

      
    }
}