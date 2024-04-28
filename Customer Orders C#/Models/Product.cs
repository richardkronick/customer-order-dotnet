using System.ComponentModel.DataAnnotations;

namespace Customer_Orders_C_.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; } = default!;

        public decimal Price { get; set; }

        public int NumInStock { get; set; }
    }
}
