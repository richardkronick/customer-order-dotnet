using Customer_Orders_C_.Common.Enums;
using Customer_Orders_C_.Data;
using Customer_Orders_C_.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


// successfully send requests and add tests




/*
 * TODO:
 *  - Create separate request objects, such as OrderRequest that doesn't include the id, and assign an guid within the route handler
 *  - Add more robust validation for order details and the products
 *  - Add logging and potentially some alerts
 *  - Use an auth token for authentication and authorization
 *  - Add validation to ensure that the number of items ordered is in fact in stock and handle a shortage
 *  - Add validation to only allow updates for certain order statuses
 *  - Add validation that the customer exists, and maybe to ensure we have all relevant data - or maybe we want to assume at this point we have it all?
 *  - Account for sales taxes
 *  - Consider alternatives to initializing the models - default values
 *  - Explore adding service classes, validation classes
 *  - Make test default methods more flexible
 *  - Add tests to cover all code paths
 *  - Remove instances of 'var' and replace with explicit types
 */

namespace Customer_Orders_C_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(OrderContext context) : ControllerBase
    {
        private readonly OrderContext _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var validationResult = ValidateOrderCreation(order);
            if (validationResult != null)
            {
                return validationResult;
            }

            _context.Orders.Add(order);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the order. Please try again later.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(string id)
        {
            Order? order = await GetOrderByOrderId(id);
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("ByCustomer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomer(string customerId)
        {
            List<Order>? orders = await _context.Orders
                                                .Where(o => o.CustomerId == customerId)
                                                .Include(o => o.Details)
                                                    .ThenInclude(d => d.Products)
                                                .ToListAsync();
            if (orders.Count == 0)
            {
                return NotFound();
            }

            return orders;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order updatedOrder)
        {
            if (updatedOrder == null)
            {
                return BadRequest("Updated order cannot be null.");
            }

            Order? existingOrder = await GetOrderByOrderId(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            if (existingOrder.CustomerId != updatedOrder.CustomerId)
            {
                return BadRequest("Cannot change the customer ID of an order.");
            }

            existingOrder.Status = updatedOrder.Status;
            existingOrder.Total = updatedOrder.Total;
            UpdateOrderDetails(existingOrder.Details, updatedOrder.Details);
            _context.Entry(existingOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }

            return NoContent();
        }

        private BadRequestObjectResult? ValidateOrderCreation(Order order)
        {
            if (order == null)
            {
                return BadRequest("Order data is required.");
            }

            if (order.Total < 0)
            {
                return BadRequest("Total must be a non-negative value.");
            }

            if (!Enum.IsDefined(typeof(OrderStatus), order.Status))
            {
                return BadRequest("Invalid order status provided.");
            }

            if (order.Details == null || !ValidateOrderDetails(order.Details))
            {
                return BadRequest("Invalid order details.");
            }

            return null;
        }

        private static bool ValidateOrderDetails(OrderDetail details)
        {
            return details != null && details.Quantity > 0 && details.Products.Count > 0;
        }

        private async Task<Order?> GetOrderByOrderId(string id)
        {
            return await _context.Orders
                                 .Include(o => o.Details)
                                    .ThenInclude(d => d.Products)
                                 .SingleOrDefaultAsync(o => o.Id == id);
        }

        private void UpdateOrderDetails(OrderDetail existingDetails, OrderDetail updatedDetails)
        {
            if (existingDetails == null || updatedDetails == null)
            {
                throw new ArgumentNullException(nameof(updatedDetails), "Updated details cannot be null");
            }

            var existingProducts = existingDetails.Products.ToDictionary(p => p.Id);

            foreach (var product in updatedDetails.Products)
            {
                if (existingProducts.TryGetValue(product.Id, out var existingProduct))
                {
                    existingProduct.Price = product.Price;
                    existingProduct.NumInStock = product.NumInStock;
                }
                else
                {
                    // If not already tracked by the context, find and add
                    var dbProduct = _context.Products.Find(product.Id);
                    if (dbProduct != null)
                    {
                        existingDetails.Products.Add(dbProduct);
                    }
                }
            }

            existingDetails.Quantity = updatedDetails.Quantity;
            existingDetails.UpdatedAt = DateTime.UtcNow;
        }
    }
}
