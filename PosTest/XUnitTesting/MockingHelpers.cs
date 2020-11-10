using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using Castle.Components.DictionaryAdapter.Xml;
using PosTest.Helpers;
using ServiceInterface.Model;

namespace XUnitTesting
{
    public class MockingHelpers
    {
        public static IEnumerable<Product> GetAllProducts()
        {
            yield return new Product() {Id = 05, Name = "Product01", Rank = 06};
            yield return new Product() {Id = 02, Name = "Product02", Rank = 18};
            yield return new Product() {Id = 17, Name = "Product03", Rank = 19};
            yield return new Product() {Id = 23, Name = "Product04", Rank = 27};
            yield return new Product() {Id = 78, Name = "Product05", Rank = 03};
            yield return new Product() {Id = 14, Name = "Product06", Rank = 16};
            yield return new Product() {Id = 13, Name = "Product07", Rank = 28};
            yield return new Product() {Id = 01, Name = "Product08", Rank = 04};
            yield return new Product() {Id = 06, Name = "Product09", Rank = 05};
            yield return new Product() {Id = 48, Name = "Product10", Rank = 24};
            yield return new Product() {Id = 73, Name = "Product11", Rank = 15};
            yield return new Product() {Id = 65, Name = "Product12", Rank = 07};
        }

        public static IEnumerable<Category> GetAllCategories()
        {
            yield return new Category() {Id = 05, Rank = 08, Name = "Cat1"};
            yield return new Category() {Id = 02, Rank = 18, Name = "Cat2"};
            yield return new Category() {Id = 17, Rank = 06, Name = "Cat3"};
            yield return new Category() {Id = 23, Rank = 27, Name = "Cat4"};
            yield return new Category() {Id = 08, Rank = 24, Name = "Cat5"};
            yield return new Category() {Id = 19, Rank = null, Name = "Cat6"};
            yield return new Category() {Id = 14, Rank = null, Name = "Cat7"};
        }

        public static IEnumerable<OrderItem> GetOrderItems()
        {
            Random ran = new Random();
            var order = new Order(){OrderItems = new BindableCollection<OrderItem>(new OrderItem[]{})};
            DateTime from = DateTime.Now;
            DateTime to = from.AddMinutes(30).AddMilliseconds(500);
            foreach (var product in GetAllProducts())
            {
                var (timeStamp, state) = GenerateRandomTimeStampAndState(from, to);
                yield return new OrderItem()
                {
                    Product = product,
                    Order = order,
                    Quantity = ran.Next(0, 10),
                    UnitPrice = product.Price,
                    TimeStamp = timeStamp,
                    State = state
                };
            }
        }
        public static void ModifySomeItems(List<OrderItem> orderItems, Dictionary<int, OrderItem> diff)
        {
            orderItems.Where(oi=> oi.TimeStamp==null).ToList().ForEach(oi=> OrderManagementHelper.TrackItemForChange(oi,diff));
            //Modify item 1
            var item1 = orderItems[0];
            OrderManagementHelper.TrackItemForChange(item1, diff);
            item1.Quantity++;
            //Modify item 2 
            var item2 = orderItems[1];
            OrderManagementHelper.TrackItemForChange(item2, diff);
            item2.Quantity--;
            //Modify item 3 
            var item3 = orderItems[2];
            OrderManagementHelper.TrackItemForChange(item3, diff);
            item3.State = OrderItemState.Removed;
            //Modify item 1
            var item4 = orderItems[3];
            item4.TimeStamp = null;
            item4.State = OrderItemState.Added;
            OrderManagementHelper.TrackItemForChange(item4, diff);
            item4.Quantity++;
        }
        public static object GetRandomEnumValueFromEnumType(Type enumType)
        {
            Random random = new Random();

            Array values = enumType.GetEnumValues();
            int index = random.Next(0,values.Length);
            return values.GetValue(index);
        }

        public static DateTime RandomDateTime(DateTime from,DateTime to)
        {
            Random rnd = new Random();
            var range = to - from;

            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }

        public static DateTime? RandomDateTimeValue(DateTime from ,DateTime to)
        {
            object[] value = {null, RandomDateTime(from, to)};
            Random rnd = new Random();
            var index =rnd.Next(value.Length);
            return (DateTime?)value[index];

        }

        public static (DateTime?, OrderItemState) GenerateRandomTimeStampAndState(DateTime from,DateTime to)
        {
            var state = (OrderItemState) GetRandomEnumValueFromEnumType(typeof(OrderItemState));
            var timeStamp = state != OrderItemState.Added ? (DateTime?)RandomDateTime(from, to) : null;
            return (timeStamp, state);
        }
    }
}