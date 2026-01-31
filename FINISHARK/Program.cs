using FINISHARK.Data;
using FINISHARK.Interfaces;
using FINISHARK.Models;
using FINISHARK.Repos;
using FINISHARK.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
namespace FINISHARK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });



            builder
                .Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                        .Json
                        .ReferenceLoopHandling
                        .Ignore;
                });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultSignInScheme =
                options.DefaultScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
                };
            });

            // CORS: allow Flutter dev server on port 8000.
            // Uses configuration key "AllowedOrigins" if present, otherwise falls back to localhost:8000.
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                                 ?? new[] { "http://localhost:8000", "https://localhost:8000" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFlutterDev", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        // Tokens (JWT) are used in this project; do NOT enable .AllowCredentials() unless using cookie auth.
                });
            });

            builder.Services.AddScoped<IStockRepo, StockRepository>();
            builder.Services.AddScoped<ICommentRepo, CommentRepo>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPortfolioRepo, PortfolioRepo>();

            var app = builder.Build();

            // Seed roles and admin user at startup
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                
                // Seed roles
                var roles = new[] { "User", "Admin" };
                foreach (var roleName in roles)
                {
                    var exists = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
                    if (!exists)
                    {
                        roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                    }
                }

                // Seed admin user
                var adminEmail = "admin@finishark.com";
                var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
                
                if (adminUser == null)
                {
                    // Create admin user
                    adminUser = new AppUser
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    
                    var createResult = userManager.CreateAsync(adminUser, "Admin123!@#").GetAwaiter().GetResult();
                    
                    if (createResult.Succeeded)
                    {
                        // Assign admin role
                        var adminRole = roleManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
                        if (adminRole != null)
                        {
                            userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                        }
                    }
                }
                else
                {
                    // Ensure existing admin user has admin role
                    var isInAdminRole = userManager.IsInRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                    if (!isInAdminRole)
                    {
                        var adminRole = roleManager.FindByNameAsync("Admin").GetAwaiter().GetResult();
                        if (adminRole != null)
                        {
                            userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                        }
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Apply CORS BEFORE authentication so preflight requests are handled.
            app.UseCors("AllowFlutterDev");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
