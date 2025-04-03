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
            var vendors = await _context.Vendors.ToListAsync();
            return Ok(new { result = "success", vendors });
        }

        // 新增
        [HttpPost]
        public async Task<ActionResult<Vendor>> AddVendor([FromBody] Vendor newVendor)
        {
            if (newVendor == null)
            {
                return BadRequest(new { result = "failure", message = "Invalid vendor data." });
            }

            _context.Vendors.Add(newVendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllVendors), new { id = newVendor.VendorId }, new { result = "success", vendor = newVendor });
        }

        // 編輯
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVendor(int id, [FromBody] Vendor updatedVendor)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "failure", message = "Vendor not found." });
            }

            vendor.Name = updatedVendor.Name;
            vendor.Type = updatedVendor.Type;
            vendor.Account = updatedVendor.Account;
            vendor.Notes = updatedVendor.Notes;

            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();

            return Ok(new { result = "success", vendor });
        }

        // 刪除
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "failure", message = "Vendor not found." });
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent(); // NoContent does not typically have a body, so you can omit the result here.
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
            return Ok(new { result = "success", vendors });
        }

        // 區間單號
        [HttpGet("{id}/batchnumbers")]
        public async Task<ActionResult<BatchNumber>> GetVendorBatchNumber(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "failure", message = "Vendor not found." });
            }

            var batchNumber = await _context.BatchNumbers
                                             .FirstOrDefaultAsync(b => b.VendorId == id);

            if (batchNumber == null)
            {
                return NotFound(new { result = "failure", message = "Batch number not found." });
            }

            return Ok(new { result = "success", batchNumber });
        }

        [HttpPost("{id}/batchnumbers")]
        public async Task<IActionResult> SetVendorBatchNumber(int id, [FromBody] BatchNumber batchNumberData)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "failure", message = "Vendor not found." });
            }

            var batchNumber = await _context.BatchNumbers
                                             .FirstOrDefaultAsync(b => b.VendorId == id);

            if (batchNumber == null)
            {
                batchNumber = new BatchNumber
                {
                    VendorId = id,
                    StartDate = batchNumberData.StartDate,
                    EndDate = batchNumberData.EndDate,
                    Quantity = batchNumberData.Quantity
                };

                _context.BatchNumbers.Add(batchNumber);
            }
            else
            {
                batchNumber.StartDate = batchNumberData.StartDate;
                batchNumber.EndDate = batchNumberData.EndDate;
                batchNumber.Quantity = batchNumberData.Quantity;

                _context.BatchNumbers.Update(batchNumber);
            }

            await _context.SaveChangesAsync();
            return Ok(new { result = "success", batchNumber });
        }
    }
}
