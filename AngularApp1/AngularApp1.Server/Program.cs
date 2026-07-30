using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── DI: Infrastructure ──────────────────────────────────────────────────────
builder.Services.AddSingleton<AngularApp1.Server.Data.DapperContext>();

// ── DI: Repositories ────────────────────────────────────────────────────────
builder.Services.AddScoped<AngularApp1.Server.Repositories.CompanyRepository>();
builder.Services.AddScoped<AngularApp1.Server.Repositories.IStoreRepository, AngularApp1.Server.Repositories.StoreRepository>();
builder.Services.AddScoped<AngularApp1.Server.Repositories.IBankRepository, AngularApp1.Server.Repositories.BankRepository>();
builder.Services.AddScoped<AngularApp1.Server.Repositories.IUserRepository, AngularApp1.Server.Repositories.UserRepository>();
builder.Services.AddScoped<AngularApp1.Server.Repositories.IRoleRepository, AngularApp1.Server.Repositories.RoleRepository>();
builder.Services.AddScoped<AngularApp1.Server.Repositories.IConsignationRepository, AngularApp1.Server.Repositories.ConsignationRepository>();

// ── HTTP Clients ────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<AngularApp1.Server.Services.IOcrService, AngularApp1.Server.Services.OcrService>();

// ── DI: Services ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<AngularApp1.Server.Services.CompanyService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IStoreService, AngularApp1.Server.Services.StoreService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IBankService, AngularApp1.Server.Services.BankService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IUserService, AngularApp1.Server.Services.UserService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IRoleService, AngularApp1.Server.Services.RoleService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IOcrService, AngularApp1.Server.Services.OcrService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IConsignationService, AngularApp1.Server.Services.ConsignationService>();
builder.Services.AddScoped<AngularApp1.Server.Services.IAuthService, AngularApp1.Server.Services.AuthService>();

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // No tolerance — token expires exactly on time
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header. Example: \"Bearer {token}\""
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ORDER MATTERS: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
