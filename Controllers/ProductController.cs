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

namespace OWMS.Controllers
{
    [Route("api/products")]
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
            var products = _context.Products
                .Include(p => p.Vendor)
                .Include(p => p.Counter)
                .AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.ProductName.Contains(name));
            }
            // 搜尋廠商
            if (!string.IsNullOrEmpty(vendor))
            {
                products = products.Where(p => p.Vendor != null && p.Vendor.Name.Contains(vendor));
            }
            // 搜尋櫃號
            if (!string.IsNullOrEmpty(counter))
            {
                products = products.Where(p => p.Counter != null && p.Vendor.Name.Contains(counter));
            }
            int totalProducts = await products.CountAsync();
            // 分頁
            var pagedProducts = await products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { totalProducts, products = pagedProducts });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // 新增
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                // 上傳圖片
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    product.PhotoUrl = await SaveProductImage(product.ImageFile);
                }
                product.QRCode = GenerateQRCode(product.ProductName);
                product.CreatedAt = DateTime.UtcNow;
                _context.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllProducts), new { id = product.Id }, product);
            }
            return BadRequest(ModelState);
        }

        // 修改
        [HttpPut("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _context.Products
                                        .Include(p => p.Vendor)
                                        .Include(p => p.Counter)
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.ProductName = updatedProduct.ProductName;
            product.Price = updatedProduct.Price;
            product.Notes = updatedProduct.Notes;
            product.VendorId = updatedProduct.VendorId;
            product.CounterId = updatedProduct.CounterId;

            // product.QRCode = GenerateQRCode(updatedProduct.ProductName);

            // if (updatedProduct.ImageFile != null && updatedProduct.ImageFile.Length > 0)
            // {
            //     product.PhotoUrl = await SaveProductImage(updatedProduct.ImageFile);
            // }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // 刪除
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private async Task<string> SaveProductImage(IFormFile imageFile)
        {
            // 生成文件名
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newFileName);

            // 保存文件
            using (var file = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(file);
            }

            return $"/images/{newFileName}";
        }

        private string GenerateQRCode(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] byteArray = qrCode.GetGraphic(20);
                    return Convert.ToBase64String(byteArray);
                }
            }
        }
    }
}
