using CloudinaryDotNet;
using EruKampusSpor.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
            


        };
    });

builder.Services.Configure<JwtAyarlari>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
});

// CORS yap�land�rmas�n� ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://192.168.43.135") // Buraya arkada��n�z�n IP adresini veya frontend'in adresini ekleyin.
        //policy.AllowAnyOrigin() // T�m k�kenlerden gelen istekleri kabul eder.
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cloudinary konfig�rasyonu
var cloudinaryAccount = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);


// Cloudinary konfservis eklemesi
builder.Services.AddSingleton(cloudinary);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQL Server connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);




//// Kestrel server yap�land�rmas�n� g�ncelleyin
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Listen(System.Net.IPAddress.Any, 5000);  // 5000 portunu t�m IP'lerden dinler
//});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, 5000); // HTTP
    options.Listen(System.Net.IPAddress.Any, 5001, listenOptions => // HTTPS
    {
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

// CORS'� etkinle�tir
app.UseCors("AllowAll");





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // https redirection'� a��k b�rak�yoruz
app.UseAuthorization(); // Authorization middleware'�n� ekliyoruz

app.MapControllers(); // Controller'lar� e�liyoruz

app.Run();
