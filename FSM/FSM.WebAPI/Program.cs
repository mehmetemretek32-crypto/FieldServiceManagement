using FSM.Application.Interfaces;
using FSM.Application.Services;
using FSM.Domain.Interfaces;
using FSM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI VE ALTYAPI (INFRASTRUCTURE) KAYITLARI ---
builder.Services.AddDbContext<FSM.Infrastructure.Context.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. İŞ MANTIĞI VE DEPO KAYITLARI (DEPENDENCY INJECTION) ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

// --- 3. CONTROLLER (GARSON) VE SWAGGER (VİTRİN) KAYITLARI ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // O meşhur yeşil arayüzün mimarı!

var app = builder.Build();

// --- 4. HTTP İSTEK HATTI (PIPELINE) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Arka planda haritayı çıkarır
    app.UseSwaggerUI(); // Haritayı yeşil, tıklanabilir bir web sitesine dönüştürür
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();