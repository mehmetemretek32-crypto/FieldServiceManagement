using FluentValidation;
using FluentValidation.AspNetCore;
using FSM.Application;
using FSM.Application.Interfaces;
using FSM.Application.Services;
using FSM.Application.Validators;
using FSM.Domain.Interfaces;
using FSM.Infrastructure.Repositories;
using FSM.WebAPI.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. VERİTABANI VE ALTYAPI (INFRASTRUCTURE) KAYITLARI ---
builder.Services.AddDbContext<FSM.Infrastructure.Context.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. İŞ MANTIĞI VE DEPO KAYITLARI (DEPENDENCY INJECTION) ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITechnicianService, TechnicianService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// --- 3. CONTROLLER (GARSON) VE SWAGGER (VİTRİN) KAYITLARI ---
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // O meşhur yeşil arayüzün mimarı!

// Kapıdaki Bodyguard'ı (Validator) sisteme tanıtıyoruz
builder.Services.AddValidatorsFromAssemblyContaining<CreateTechnicianDtoValidator>();
builder.Services.AddApplicationServices();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// --- 4. HTTP İSTEK HATTI (PIPELINE) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     // Arka planda haritayı çıkarır
    app.UseSwaggerUI();   // Haritayı yeşil, tıklanabilir bir web sitesine çevirir
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();