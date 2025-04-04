using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using OWMS.Data;
using OWMS.Models;
using QRCoder;
using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore.Storage.Json;
using OfficeOpenXml;

namespace OWMS.Controllers
{
    [Route("/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
        [FromQuery] string? name,
        [FromQuery] string? vendor,
        [FromQuery] string? counter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
            {
            try
            {
                // 取得所有的廠商
                var vendors = await _context.Vendors.ToListAsync();

                // 取得所有的櫃號
                var counters = await _context.Counters.ToListAsync();

                // 查詢產品資料
                var products = _context.Products
                    .Include(p => p.Vendor)
                    .Include(p => p.Counter)
                    .AsQueryable();

                // 篩選條件
                if (!string.IsNullOrEmpty(name))
                {
                    products = products.Where(p => p.ProductName.Contains(name));
                }
                if (!string.IsNullOrEmpty(vendor))
                {
                    products = products.Where(p => p.Vendor != null && p.Vendor.Name.Contains(vendor));
                }
                if (!string.IsNullOrEmpty(counter))
                {
                    products = products.Where(p => p.Counter != null && p.Counter.Name.Contains(counter));
                }

                // 計算總產品數量
                int totalProducts = await products.CountAsync();

                // 分頁
                var pagedProducts = await products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 返回結果
                return Ok(new
                {
                    result = "success",
                    message = "Products retrieved successfully.",
                    totalProducts,
                    totalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                    currentPage = pageNumber,
                    pageSize,
                    products = pagedProducts,
                    vendors = vendors.Select(v => v.Name).ToList(), // 返回所有廠商名稱
                    counters = counters.Select(c => c.Name).ToList() // 返回所有櫃號名稱
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { result = "error", message = "Internal server error", details = ex.Message });
            }
        }


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetProductById(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound(new { result = "error", message = "Product not found." });
        //    }
        //    return Ok(new { result = "success", message = "Product retrieved successfully.", product });
        //}

        // 新增
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var maxFileSize = 5 * 1024 * 1024;
                    var fileExtension = Path.GetExtension(product.ImageFile.FileName).ToLower();
                    if (!allowedImageExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new { result = "error", message = "Invalid image file. Supported formats are .jpg, .jpeg, .png, .gif." });
                    }
                    if (product.ImageFile.Length > maxFileSize)
                    {
                        return BadRequest(new { result = "error", message = "File size should not exceed 5MB." });
                    }

                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newFileName);
                    using (var file = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(file);
                    }

                    product.PhotoUrl = $"/images/{newFileName}";
                }

                //product.QRCode = GenerateQRCode(product.ProductName);
                product.CreatedAt = DateTime.UtcNow;
                _context.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, new
                {
                    result = "success",
                    message = "Product created successfully.",
                    product,
                    qrCode = product.QRCode
                });
            }

            return BadRequest(new { result = "error", message = "Invalid input data.", details = ModelState });
        }

        //// 修改
        //[HttpPatch("edit/{id}")]
        //public async Task<IActionResult> EditProduct(int id, [FromBody] ProductRequest updatedProduct)
        //{
        //    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        //    if (product == null
        //        )
        //    {
        //        return NotFound(new { result = "error", message = "Product not found." });
        //    }

        //    if (!string.IsNullOrEmpty(updatedProduct.ProductName))
        //    {
        //        product.ProductName = updatedProduct.ProductName;
        //    }

        //    if (updatedProduct.Price.HasValue)
        //    {
        //        product.Price = updatedProduct.Price.Value;
        //    }

        //    if (updatedProduct.VendorId.HasValue)
        //    {
        //        var vendorExist = await _context.Vendors.AnyAsync(v => v.VendorId == updatedProduct.VendorId.Value);
        //        if (!vendorExist)
        //        {
        //            return BadRequest(new { result = "error", message = "Invalid VendorId." });
        //        }
        //        product.VendorId = updatedProduct.VendorId.Value;
        //    }

        //    if (updatedProduct.CounterId.HasValue)
        //    {
        //        var counterExist = await _context.Counters.AnyAsync(c => c.CounterId == updatedProduct.CounterId.Value);
        //        if (!counterExist)
        //        {
        //            return BadRequest(new { result = "error", message = "Invalid CounterId." });
        //        }
        //        product.CounterId = updatedProduct.CounterId.Value;
        //    }

        //    _context.Products.Update(product);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { result = "success", message = "Product updated successfully.", product });
        //}

        //// 刪除
        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound(new { result = "error", message = "Product not found." });
        //    }

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", product.PhotoUrl.TrimStart('/'));
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.Delete(filePath);
        //    }

        //    _context.Products.Remove(product);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private string GenerateQRCode(string data)
        //{
        //    using (var qrGenerator = new QRCodeGenerator())
        //    {
        //        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

        //        using (var qrCode = new PngByteQRCode(qrCodeData))
        //        {
        //            byte[] byteArray = qrCode.GetGraphic(20);
        //            return Convert.ToBase64String(byteArray);
        //        }
        //    }
        //}

        //[HttpPost("generate-qrcode/batch")]
        //public async Task<IActionResult> GenerateBatchQRCode([FromBody] List<int> productIds)
        //{
        //    var products = await _context.Products
        //                                .Where(p => productIds.Contains(p.Id))
        //                                .ToListAsync();

        //    if (products == null || products.Count == 0)
        //    {
        //        return NotFound(new { result = "error", message = "No products found for the given IDs." });
        //    }

        //    var qrCodes = new List<byte[]>();

        //    var baseUrl = $"{Request.Scheme}://{Request.Host}";

        //    using (var qrGenerator = new QRCodeGenerator())
        //    {
        //        foreach (var product in products)
        //        {
        //            var qrUrl = $"{baseUrl}/product/{product.Id}";

        //            var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);
        //            using (var qrCode = new PngByteQRCode(qrCodeData))
        //            {
        //                var qrImage = qrCode.GetGraphic(20);
        //                qrCodes.Add(qrImage);
        //            }
        //        }
        //    }

        //    // 打包 ZIP 檔案
        //    using var memoryStream = new MemoryStream();
        //    using (var zipArchive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
        //    {
        //        for (int i = 0; i < qrCodes.Count; i++)
        //        {
        //            var entry = zipArchive.CreateEntry($"QRCode_{products[i].ProductName}.png", System.IO.Compression.CompressionLevel.Fastest);
        //            using (var entryStream = entry.Open())
        //            {
        //                entryStream.Write(qrCodes[i], 0, qrCodes[i].Length);
        //            }
        //        }
        //    }

        //    return File(memoryStream.ToArray(), "application/zip", "QRCodes.zip");
        //}

        // 匯入Excel 
        //[HttpPost("import")]
        //public async Task<IActionResult> ImportExcel(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest(new { result = "error", message = "No file uploaded." });
        //    }

        //    try
        //    {
        //        using (var package = new ExcelPackage(file.OpenReadStream()))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0];
        //            var rowCount = worksheet.Dimension.Rows;

        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                var productName = worksheet.Cells[row, 2].Text;
        //                var price = worksheet.Cells[row, 3].Value;

        //                int parsedPrice = 0;

        //                if (price is decimal || price is double)
        //                {
        //                    parsedPrice = (int)Math.Floor(Convert.ToDecimal(price));
        //                }

        //                var qrCode = worksheet.Cells[row, 4].Text;

        //                if (string.IsNullOrWhiteSpace(productName))
        //                {
        //                    continue;
        //                }

        //                var product = new Product
        //                {
        //                    ProductName = productName,
        //                    Price = parsedPrice,
        //                    QRCode = qrCode,
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                _context.Products.Add(product);
        //            }

        //            await _context.SaveChangesAsync();
        //            return Ok(new { result = "success", message = "Products imported successfully." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { result = "error", message = "Internal server error", details = ex.Message });
        //    }
        //}
    }
}
