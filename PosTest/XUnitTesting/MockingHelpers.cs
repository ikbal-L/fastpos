using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Castle.Components.DictionaryAdapter.Xml;
using ServiceInterface.Model;

namespace XUnitTesting
{
    public class MockingHelpers
    {
        public static IEnumerable<Product> GetAllProducts()
        {
            yield return new Product() {Id = 5, Rank = 6};
            yield return new Product() {Id = 2, Rank = 18};
            yield return new Product() {Id = 17, Rank = 6};
            yield return new Product() {Id = 23, Rank = 27};
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
            var order = new Order();
            DateTime from = DateTime.Now;
            DateTime to = from.AddMinutes(30).AddMilliseconds(500);
            foreach (var product in GetAllProducts())
            {
                yield return new OrderItem()
                {
                    Product = product,
                    Quantity = ran.Next(0, 10),
                    UnitPrice = product.Price,
                    TimeStamp = RandomDateTimeValue(from,to),
                    Order = order,
                    State = (OrderItemState)GetRandomEnumValueFromEnumType(typeof(OrderItemState))
                };
            }
        }

        public static object GetRandomEnumValueFromEnumType(Type enumType)
        {
            Random random = new Random();

            Array values = enumType.GetEnumValues();
            int index = random.Next(values.Length);
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
    }
}