using Customer_Orders_C_.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer_Orders_C_.Models
{
    public class Order
    {
        private Order() { }

        public Order(string id, string customerId, OrderDetail details, decimal total, OrderStatus status)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
            Details = details ?? throw new ArgumentNullException(nameof(details));
            Total = total;
            Status = status;
        }

        [Key]
        public string Id { get; init; }

        [ForeignKey("Customer")]
        public string CustomerId { get; init; }

        public OrderDetail Details { get; set; }

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; }
    }
}
