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

// CORS yapýlandýrmasýný ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://192.168.43.135") // Buraya arkadaþýnýzýn IP adresini veya frontend'in adresini ekleyin.
        //policy.AllowAnyOrigin() // Tüm kökenlerden gelen istekleri kabul eder.
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Cloudinary konfigürasyonu
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




//// Kestrel server yapýlandýrmasýný güncelleyin
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Listen(System.Net.IPAddress.Any, 5000);  // 5000 portunu tüm IP'lerden dinler
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

// CORS'ý etkinleþtir
app.UseCors("AllowAll");





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // https redirection'ý açýk býrakýyoruz
app.UseAuthorization(); // Authorization middleware'ýný ekliyoruz

app.MapControllers(); // Controller'larý eþliyoruz

app.Run();
