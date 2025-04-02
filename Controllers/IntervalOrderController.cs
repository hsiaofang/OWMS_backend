//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OWMS.Data;
//using OWMS.Models;

//namespace OWMS.Controllers
//{
//     [Route("api/[controller]")]
//    [ApiController]
//    public class IntervalOrderController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public IntervalOrderController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("create")]
//        public ActionResult CreateIntervalOrder()
//        {
//            string batchCode = "B" + DateTime.Now.ToString("yyyyMMddHHmmss");

//            var intervalOrder = new IntervalOrder
//            {
//                BatchCode = batchCode,
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            };

//            _context.IntervalOrders.Add(intervalOrder);
//            _context.SaveChanges();

//            return Ok(new { message = "區間單號創建成功", batchCode });
//        }

//        [HttpPost("assign-orders")]
//        public ActionResult AssignOrders([FromBody] AssignOrdersRequest request)
//        {
//            var intervalOrder = _context.IntervalOrders
//                .FirstOrDefault(b => b.BatchCode == request.BatchCode);

//            if (intervalOrder == null)
//            {
//                return NotFound(new { message = "區間單號不存在" });
//            }

//            foreach (var orderId in request.OrderIds)
//            {
//                var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
//                if (order != null)
//                {
//                    order.IntervalOrderId = intervalOrder.Id;
//                    _context.Orders.Update(order);
//                }
//            }

//            _context.SaveChanges();

//            return Ok(new { message = "訂單成功分配到區間單號", batchCode = intervalOrder.BatchCode });
//        }
//    }

//    public class AssignOrdersRequest
//    {
//        public string BatchCode { get; set; }
//        public List<int> OrderIds { get; set; }
//    }
//}
