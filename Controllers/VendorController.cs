using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OWMS.Data;
using OWMS.Models;

namespace OWMS.Controllers
{
    [Route("api/vendor")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetAllVendors()
        {
            return await _context.Vendors.ToListAsync();
        }
        // 編輯
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVendor(int id, [FromBody] Vendor updatedVendor)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            vendor.Name = updatedVendor.Name;
            vendor.Type = updatedVendor.Type;
            vendor.Notes = updatedVendor.Notes;

            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();

            return Ok(vendor);
        }

        // 搜尋
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Vendor>>> SearchVendors(
            [FromQuery] string vendorName,
            [FromQuery] string contact)
        {
            IQueryable<Vendor> query = _context.Vendors;

            if (!string.IsNullOrEmpty(vendorName))
            {
                query = query.Where(v => v.Name.Contains(vendorName));
            }

            if (!string.IsNullOrEmpty(contact))
            {
                query = query.Where(v => v.Account.Contains(contact));
            }

            var vendors = await query.ToListAsync();
            return Ok(vendors);
        }
    }

}
