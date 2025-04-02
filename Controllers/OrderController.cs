//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OWMS.Data;
//using OWMS.Models;
//namespace OWMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrdersController : ControllerBase
//    {
//        private readonly ApplicationDbContext _ctx;

//        public OrdersController(ApplicationDbContext context)
//        {
//            _ctx = context;
//        }

//        // GET: api/Orders
//        [HttpGet]
//        public async Task<IActionResult> GetOrders()
//        {
//            var orders = await _ctx.Orders
//                .Include(o => o.IntervalNumber)
//                .Include(o => o.Vendor)
//                .Select(o => new
//                {
//                    o.OrderId,
//                    IntervalNumber = o.IntervalNumber.Id,
//                    o.WarehouseEntryDate,
//                    VendorName = o.Vendor.Name,
//                    o.OrderDate
//                })
//                .ToListAsync();

//            return Ok(orders);
//        }

//        // GET: api/Orders/5
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetOrder(int id)
//        {
//            var order = await _ctx.Orders
//                .Include(o => o.IntervalNumber)
//                .Include(o => o.Vendor)
//                .FirstOrDefaultAsync(o => o.OrderId == id);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            return Ok(new
//            {
//                order.OrderId,
//                IntervalNumber = order.IntervalNumber.Id,
//                order.WarehouseEntryDate,
//                VendorName = order.Vendor.Name,
//                order.OrderDate
//            });
//        }

//        // POST: api/Orders
//        [HttpPost]
//        public async Task<IActionResult> CreateOrder([FromBody] Order order)
//        {
//            if (order == null)
//            {
//                return BadRequest("Invalid data.");
//            }

//            _ctx.Orders.Add(order);
//            await _ctx.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
//        }

//        // PUT: api/Orders/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
//        {
//            if (id != order.OrderId)
//            {
//                return BadRequest();
//            }

//            _ctx.Entry(order).State = EntityState.Modified;
//            await _ctx.SaveChangesAsync();

//            return NoContent();
//        }

//        // DELETE: api/Orders/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteOrder(int id)
//        {
//            var order = await _ctx.Orders.FindAsync(id);
//            if (order == null)
//            {
//                return NotFound();
//            }

//            _ctx.Orders.Remove(order);
//            await _ctx.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
