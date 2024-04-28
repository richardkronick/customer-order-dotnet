using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer_Orders_C_.Models
{
    public class OrderDetail
    {
        private OrderDetail() { }

        public OrderDetail(string id, List<Product> products, int quantity)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Products = products ?? throw new ArgumentNullException(nameof(products));
            Quantity = quantity;
            CreatedAt = DateTime.Now;
        }

        [Key]
        public string Id { get; init; }

        public List<Product> Products { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
