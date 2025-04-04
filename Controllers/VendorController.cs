using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OWMS.Data;
using OWMS.Models;
using BCrypt.Net;
<<<<<<< HEAD
=======
using Newtonsoft.Json;
>>>>>>> 614672c (fix: 廠商跟區間單號之間的關聯)

namespace OWMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public VendorController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetAllVendors(int page = 1, int pageSize = 10)
        {

            var vendors = await _context.Vendors.ToListAsync();


            return Ok(new
            {
                result = "success",

                vendors
            });
        }
        [HttpGet("counter")]
        async Task<ActionResult<IEnumerable<Counter>>> GetAllCounters()
        {
            var counters = await _context.Counters.ToListAsync();


            return Ok(new
            {
                result = "success",
                counters
            });
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Vendor>>> GetAllVendors(int page = 1, int pageSize = 10)
        //{
        //    var response = await _context.Vendors
        //        .Include(v => v.BatchNumbers)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var totalVendors = await _context.Vendors.CountAsync();

        //    var totalPages = (int)Math.Ceiling((double)totalVendors / pageSize);

        //    return Ok(new
        //    {
        //        result = "success",
        //        vendors = response.Select(v => new
        //        {
        //            v.VendorId,
        //            v.Name,
        //            v.Type,
        //            v.Account,
        //            v.Password,
        //            v.Notes,
        //            BatchNumbers = v.BatchNumbers.Select(b => new
        //            {
        //                b.Id,
        //                b.BatchCode,
        //                b.StartDate,
        //                b.EndDate,
        //                b.Quantity,
        //                b.CreatedAt,
        //                b.Notes
        //            }).ToList()
        //        }),
        //        totalVendors,
        //        totalPages,
        //        currentPage = page
        //    });
        //    Console.WriteLine(JsonConvert.SerializeObject(response)); 
        //    return Ok(response);
        //}

>>>>>>> 614672c(fix: 廠商跟區間單號之間的關聯)
        // 新增廠商同時新增區間單號
        [HttpPost]
        public async Task<ActionResult> CreateVendor([FromBody] VendorRequest model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var vendor = new Vendor
                    {
                        Name = model.Name,
                        Type = model.Type,
                        Account = model.Account,
                        Password = model.Password,
                        Notes = model.Notes
                    };

                    _context.Vendors.Add(vendor);
                    await _context.SaveChangesAsync();

                    foreach (var batch in model.BatchNumbers)
                    {
                        var batchNumber = new BatchNumber
                        {
                            BatchCode = batch.BatchCode,
                            StartDate = batch.StartDate,
                            EndDate = batch.EndDate,
                            VendorId = vendor.VendorId,
                            Quantity = batch.Quantity,
                            Notes = batch.Notes
                        };

                        _context.BatchNumbers.Add(batchNumber);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { result = "success", message = "Vendor and Batch Numbers added successfully." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { result = "error", message = "Error occurred while adding vendor and batch numbers.", details = ex.Message });
            }
        }


        // 編輯廠商
        [HttpPut("{id}")]
        public async Task<IActionResult> EditVendor(int id, [FromBody] Vendor updatedVendor)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "error", message = "Vendor not found." });
            }

            vendor.Name = updatedVendor.Name;
            vendor.Type = updatedVendor.Type;
            vendor.Account = updatedVendor.Account;
            vendor.Notes = updatedVendor.Notes;

            _context.Vendors.Update(vendor);
            await _context.SaveChangesAsync();

            return Ok(new { result = "success", vendor });
        }

        // 刪除廠商
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors
                                        .Include(v => v.BatchNumbers)
                                        .FirstOrDefaultAsync(v => v.VendorId == id);

            if (vendor == null)
            {
                return NotFound(new { result = "error", message = "Vendor not found." });
            }

            if (vendor.BatchNumbers.Any())
            {
                return BadRequest(new { result = "error", message = "Cannot delete vendor with associated batch numbers." });
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // 搜尋廠商
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

        // 查詢區間單號
        [HttpGet("{id}/batchnumbers")]
        public async Task<ActionResult<IEnumerable<BatchNumber>>> GetBatchNumbers(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "error", message = "Vendor not found." });
            }

            var batchNumbers = await _context.BatchNumbers
                                              .Where(b => b.VendorId == id)
                                              .ToListAsync();

            if (!batchNumbers.Any())
            {
                return NotFound(new { result = "error", message = "No batch numbers found for this vendor." });
            }

            return Ok(new { result = "success", batchNumbers });
        }

        // 設定區間單號
        [HttpPost("{id}/batchnumbers")]
        public async Task<IActionResult> SetBatchNumber(int id, [FromBody] BatchNumber batchNumberData)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound(new { result = "error", message = "Vendor not found." });
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
