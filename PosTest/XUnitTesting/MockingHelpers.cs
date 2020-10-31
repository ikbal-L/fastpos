using System.Collections.Generic;
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
    }
}