using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProductCodeApi.Controllers;
using ProductCodeApi.Data;
using ProductCodeApi.Models;
using ProductCodeApi.Services;

namespace ProductCodeApi.Tests;

public class ProductCodesControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BarcodeService _barcodeService;

    public ProductCodesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _barcodeService = new BarcodeService();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetProductCodes_ReturnsAllCodes()
    {
        // Arrange
        _context.ProductCodes.Add(new ProductCode { Code = "ABCD-1234-EFGH-5678" });
        await _context.SaveChangesAsync();
        var controller = new ProductCodesController(_context, _barcodeService);

        // Act
        var result = await controller.GetProductCodes();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductCode>>>(result);
        var codes = Assert.IsAssignableFrom<IEnumerable<ProductCode>>(actionResult.Value);
        Assert.Single(codes);
    }

    [Fact]
    public async Task PostProductCode_ValidCode_AddsAndReturnsCreated()
    {
        // Arrange
        var controller = new ProductCodesController(_context, _barcodeService);
        var newCode = new ProductCode { Code = "ABCD-1234-EFGH-5678" };

        // Act
        var result = await controller.PostProductCode(newCode);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetProductCodes", createdResult.ActionName);
        Assert.Equal(1, await _context.ProductCodes.CountAsync());
    }

    [Fact]
    public async Task PostProductCode_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var controller = new ProductCodesController(_context, _barcodeService);
        var invalidCode = new ProductCode { Code = "wrong-format" };

        // Act
        var result = await controller.PostProductCode(invalidCode);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProductCode_ExistingId_RemovesFromDb()
    {
        // Arrange
        var item = new ProductCode { Code = "DELETE-TEST-1234-5678" };
        _context.ProductCodes.Add(item);
        await _context.SaveChangesAsync();
        var controller = new ProductCodesController(_context, _barcodeService);

        // Act
        var result = await controller.DeleteProductCode(item.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Empty(_context.ProductCodes);
    }

    [Fact]
    public async Task DeleteProductCode_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var controller = new ProductCodesController(_context, _barcodeService);

        // Act
        var result = await controller.DeleteProductCode(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetBarcode_ValidCode_ReturnsPngFileWithContent()
    {
        // Arrange
        var controller = new ProductCodesController(_context, _barcodeService);

        // Act
        var result = controller.GetBarcode("ABCD-1234-EFGH-5678");

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("image/png", fileResult.ContentType);
        Assert.NotEmpty(fileResult.FileContents);
        Assert.True(fileResult.FileContents.Length > 1000); // barcode จริงต้องมีขนาดพอสมควร
    }
}