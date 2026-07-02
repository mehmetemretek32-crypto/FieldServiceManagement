using FluentValidation;
using FluentValidation.AspNetCore;
using FSM.Application;
using FSM.Application.Interfaces;
using FSM.Application.Validators;
using FSM.Domain.Interfaces;
using FSM.Infrastructure.Repositories;
using FSM.Infrastructure.Services;
using FSM.WebAPI.Hubs;
using FSM.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models; // Swagger modelleri için zorunlu kütüphane
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- JWT Ayarları ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// --- 1. VERİTABANI VE ALTYAPI (INFRASTRUCTURE) KAYITLARI ---
builder.Services.AddDbContext<FSM.Infrastructure.Context.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2. İŞ MANTIĞI VE DEPO KAYITLARI (DEPENDENCY INJECTION) ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// --- 3. CONTROLLER (GARSON), CORS VE SWAGGER (VİTRİN) KAYITLARI ---
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Her yerden erişime izin ver (Test için)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // SignalR için gereklidir
    });
});

// --- JWT Authentication Configuration ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? string.Empty))
    };
});
// JWT'nin rol bilgisini doğru yerden okuması için ekliyoruz
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = System.Security.Claims.ClaimTypes.Role;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();

// Kapıdaki Bodyguard'ı (Validator) sisteme tanıtıyoruz
builder.Services.AddValidatorsFromAssemblyContaining<CreateTechnicianCommandValidator>();
builder.Services.AddApplicationServices();

// --- SWAGGER JWT ENTEGRASYONU ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FSM API", Version = "v1" });

    // Swagger'a Bearer Token kullanılacağını bildiriyoruz
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token'ınızı buraya yapıştırın. Sadece ürettiğiniz o uzun şifreli metni girmeniz yeterlidir."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- 4. HTTP İSTEK HATTI (PIPELINE) ---

// ExceptionMiddleware en üstte olmalı ki altındaki tüm süreçlerdeki hataları yakalayabilsin
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     // Arka planda haritayı çıkarır
    app.UseSwaggerUI();   // Haritayı yeşil, tıklanabilir bir web sitesine çevirir
}

app.UseHttpsRedirection();

// DİKKAT: CORS her zaman Authentication ve Authorization'dan ÖNCE gelmelidir!
app.UseCors("AllowAll");

app.UseAuthentication(); // 1. Kimlik Doğrulama (Sisteme girebilir mi?)
app.UseAuthorization();  // 2. Yetki Doğrulama (Bu işlemi yapabilir mi?)

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();