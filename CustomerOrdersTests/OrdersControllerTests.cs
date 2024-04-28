using Customer_Orders_C_.Common.Enums;
using Customer_Orders_C_.Controllers;
using Customer_Orders_C_.Data;
using Customer_Orders_C_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrdersTests
{
    [TestClass]
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly OrderContext _context;

        public OrdersControllerTests()
        {
            DbContextOptions<OrderContext> options = new DbContextOptionsBuilder<OrderContext>()
                .UseInMemoryDatabase(databaseName: "TestOrderDb")
                .Options;

            _context = new OrderContext(options);
            _controller = new OrdersController(_context);
        }

        [TestInitialize]
        public void Initialize()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateOrder_ValidOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            Order order = GetOrderDefault();

            // Act
            IActionResult? result = await _controller.CreateOrder(order);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public async Task CreateOrder_InvalidTotal_ReturnsBadRequest()
        {
            // Arrange
            Order order = GetOrderDefault();
            order.Total = -1;

            // Act
            IActionResult? result = await _controller.CreateOrder(order);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetOrderById_OrderExists_ReturnsOrder()
        {
            // Arrange
            Order testOrder = GetOrderDefault();
            _context.Orders.Add(testOrder);
            await _context.SaveChangesAsync();

            // Act
            ActionResult<Order>? result = await _controller.GetOrderById(testOrder.Id);

            // Assert
            Order? order = result.Value;
            Assert.IsNotNull(order);
            Assert.AreEqual(testOrder.Id, order.Id);
        }

        [TestMethod]
        public async Task GetOrderById_OrderDoesNotExist_ReturnsNotFound()
        {
            // Arrange

            // Act
            ActionResult<Order>? result = await _controller.GetOrderById("Not a real id");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetOrdersByCustomer_CustomerHasMultipleOrders_ReturnsOrders()
        {
            // Arrange
            Order order = GetOrderDefault();
            List<Order> customerOrders = [order];

            _context.Orders.AddRange(customerOrders);
            _context.SaveChanges();

            // Act
            ActionResult<IEnumerable<Order>> result = await _controller.GetOrdersByCustomer(customerOrders[0].CustomerId);

            // Assert
            IEnumerable<Order>? orderResult = result.Value;
            Assert.IsNotNull(orderResult);
            List<Order> orders = orderResult.ToList();
            Assert.AreEqual(customerOrders.Count, orders?.Count);
        }

        [TestMethod]
        public async Task GetOrdersByCustomer_NoOrdersForCustomer_ReturnsNotFound()
        {
            // Arrange

            // Act
            var result = await _controller.GetOrdersByCustomer("Some unknown customer id");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateOrder_OrderDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            Order updatedOrder = GetOrderDefault();

            // Act
            IActionResult? result = await _controller.UpdateOrder("Not a real order id", updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateOrder_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            Order existingOrder = GetOrderDefault();
            _context.Orders.Add(existingOrder);
            await _context.SaveChangesAsync();

            Order updatedOrder = GetOrderDefault(true);
            updatedOrder.Total = 59.99m;

            // Act
            IActionResult? result = await _controller.UpdateOrder(existingOrder.Id, updatedOrder);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateOrder_NullOrder_ReturnsBadRequest()
        {
            // Act
            IActionResult? result = await _controller.UpdateOrder("Some random guid", null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }


        private static Product GetProductDefault(bool backupOrder = false)
        {
            return new Product
            {
                Id = backupOrder ? "P123" : "P122",
                Price = 19.99m,
                NumInStock = 100
            };
        }

        private static OrderDetail GetOrderDetailDefault(bool backupOrder = false)
        {
            return new OrderDetail(
                id: backupOrder ? "O456" : "0455",
                products: [GetProductDefault()],
                quantity: 2
            );

        }

        private static Order GetOrderDefault(bool backupOrder = false)
        {
            return new Order(
                id: backupOrder ? "O789" : "0788",
                customerId: "C098",
                details: GetOrderDetailDefault(backupOrder),
                total: 19.99m,
                status: OrderStatus.Pending
            );
        }
    }
}