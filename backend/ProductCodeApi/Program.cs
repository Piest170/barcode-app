using Microsoft.EntityFrameworkCore;
using ProductCodeApi.Data;
using ProductCodeApi.Models;
using ProductCodeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productcodes.db"));
builder.Services.AddScoped<BarcodeService>();

// Add CORS policy
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll",
//         policy => policy.AllowAnyOrigin()
//                         .AllowAnyMethod()
//                         .AllowAnyHeader());
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.UseAuthorization();
app.MapControllers();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.ProductCodes.Any())
    {
        db.ProductCodes.AddRange(
            new ProductCode { Code = "ABCD-1234-EFGH-5678", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new ProductCode { Code = "WXYZ-9876-UVWX-4321", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new ProductCode { Code = "TEST-0000-AAAA-BBBB", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new ProductCode { Code = "PROD-1111-2222-3333", CreatedAt = DateTime.UtcNow }
        );
        db.SaveChanges();
    }
}

app.Run();