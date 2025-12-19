using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCodeApi.Data;
using ProductCodeApi.Models;
using ProductCodeApi.Services;
using System.Text.RegularExpressions;

namespace ProductCodeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCodesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BarcodeService _barcodeService;

    public ProductCodesController(AppDbContext context, BarcodeService barcodeService)
    {
        _context = context;
        _barcodeService = barcodeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductCode>>> GetProductCodes()
    {
        return await _context.ProductCodes.OrderBy(p => p.Id).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ProductCode>> PostProductCode(ProductCode productCode)
    {
        if (!IsValidCode(productCode.Code))
        {
            return BadRequest("รูปแบบรหัสสินค้าต้องเป็นตัวเลขหรือตัวอักษรใหญ่ 16 หลัก ในรูปแบบ xxxx-xxxx-xxxx-xxxx");
        }

        _context.ProductCodes.Add(productCode);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProductCodes), new { id = productCode.Id }, productCode);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductCode(int id)
    {
        var productCode = await _context.ProductCodes.FindAsync(id);
        if (productCode == null)
        {
            return NotFound();
        }

        _context.ProductCodes.Remove(productCode);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("barcode/{code}")]
    public IActionResult GetBarcode(string code)
    {
        if (!IsValidCode(code))
        {
            return BadRequest("รหัสไม่ถูกต้อง");
        }

        var imageBytes = _barcodeService.GenerateCode39(code);
        return File(imageBytes, "image/png");
    }

    private static bool IsValidCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        const string pattern = @"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$";
        return Regex.IsMatch(code.Trim(), pattern);  // เพิ่ม .Trim() เผื่อมี space ซ่อน
    }
}