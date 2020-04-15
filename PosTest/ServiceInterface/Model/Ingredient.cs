namespace ServiceInterface.Model
{
    public class Ingredient
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public decimal Quantity { get; set; }
    }
}