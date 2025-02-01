using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using Microsoft.OpenApi.Models;
using RoadMonitoringSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ������ ������������ ���� ����� (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16)
{
    var randomBytes = new byte[32];
    using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomBytes);
    }
    jwtKey = Convert.ToBase64String(randomBytes);
    Console.WriteLine("����������� ���������� JWT-����: " + jwtKey);
}
var key = Encoding.UTF8.GetBytes(jwtKey);

// ������ �������������� ����� JWT
builder.Services.AddSingleton<JwtService>();
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
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
// ������ ����������
builder.Services.AddControllers();

builder.Services.AddScoped<IRoadSectionService, RoadSectionService>();
builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
});


// ����������� Swagger � ��������� JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Road Monitoring System API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "������ JWT-����� � ������ Bearer {�����}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        //dbContext.Database.CanConnect();
        dbContext.Database.GetDbConnection().Open();
        Console.WriteLine("ϳ��������� �� ���� ����� ������.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"������� ���������� �� ���� �����: {ex.Message}");
        throw;
    }
}

// ������������� Swagger ����� � ����� ��������
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Road Monitoring System API v1");
        c.RoutePrefix = string.Empty; // ������ Swagger ��������� �� ��������� URL
    });
}

// ������������ �������������
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
