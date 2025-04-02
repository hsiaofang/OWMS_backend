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

        // 新增
        [HttpPost]
        public async Task<ActionResult<Vendor>> AddVendor([FromBody] Vendor newVendor)
        {
            if (newVendor == null)
            {
                return BadRequest("Invalid vendor data.");
            }

            _context.Vendors.Add(newVendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllVendors), new { id = newVendor.VendorId }, newVendor);
        }

        // 編輯
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVendor(int id, [FromBody] Vendor updatedVendor)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            vendor.Name = updatedVendor.Name;
            vendor.Type = updatedVendor.Type;
            vendor.Account = updatedVendor.Account;
            vendor.Notes = updatedVendor.Notes;

            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();

            return Ok(vendor);
        }

        // 刪除
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
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

        // 區間單號
        [HttpGet("{id}/interval")]
        public async Task<ActionResult<IntervalNumber>> GetVendorInterval(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            var interval = await _context.IntervalNumbers
                                          .FirstOrDefaultAsync(i => i.VendorId == id);

            if (interval == null)
            {
                return NotFound("Interval settings not found.");
            }

            return Ok(interval);
        }


        [HttpPost("{id}/interval")]
        public async Task<IActionResult> SetVendorInterval(int id, [FromBody] IntervalNumber intervalData)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            var interval = await _context.IntervalNumbers
                                          .FirstOrDefaultAsync(i => i.VendorId == id);

            if (interval == null)
            {
                interval = new IntervalNumber
                {
                    VendorId = id,
                    StartDate = intervalData.StartDate,
                    EndDate = intervalData.EndDate,
                    Quantity = intervalData.Quantity
                };

                _context.IntervalNumbers.Add(interval);
            }
            else
            {
                interval.StartDate = intervalData.StartDate;
                interval.EndDate = intervalData.EndDate;
                interval.Quantity = intervalData.Quantity;

                _context.IntervalNumbers.Update(interval);
            }

            await _context.SaveChangesAsync();
            return Ok(interval);
        }
    }
}