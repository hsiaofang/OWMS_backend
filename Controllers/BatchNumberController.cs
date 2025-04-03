using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OWMS.Data;
using OWMS.Models;
using OWMS.Requests;

namespace OWMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntervalOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IntervalOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public ActionResult CreateIntervalOrder()
        {
            string batchCode = "B" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var intervalOrder = new BatchNumber
            {
                BatchCode = batchCode,
                CreatedAt = DateTime.Now,
            };

            _context.BatchNumbers.Add(intervalOrder);
            _context.SaveChanges();

            return Ok(new { result = "success", batchCode });
        }
    }
}