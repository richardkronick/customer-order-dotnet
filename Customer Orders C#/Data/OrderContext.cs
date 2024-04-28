using Customer_Orders_C_.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Orders_C_.Data
{
    public class OrderContext(DbContextOptions<OrderContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
