using cmsAuth.Application.Common.Settings;
using cmsAuth.Application.Interfaces;
using cmsAuth.Infrastructure.Persistance;
using cmsAuth.Infrastructure.Repositories;
using cmsAuth.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var redisOptionsConfiguration = builder.Configuration["Redis:Connection"];

builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
    redisOptions.Configuration = redisOptionsConfiguration;
});

builder.Services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>(sp =>
{
    var jwtSettings = sp.GetRequiredService<IOptions<JwtSettings>>().Value;
    return new JwtTokenProvider(jwtSettings);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
