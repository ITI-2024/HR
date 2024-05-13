

using HR.Controllers;
using HR.Helper;
using HR.Migrations;
using HR.Models;
using HR.Repository;
using HR.Seed;
using HR.serviec;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.Text;

namespace HR
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
      
            builder.Services.AddControllers();

            builder.Services.AddDbContext<HRDbcontext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Mycon")), ServiceLifetime.Scoped);
            builder.Services.AddIdentity<ApplictionUsers, IdentityRole>().AddEntityFrameworkStores<HRDbcontext>()
           .AddDefaultTokenProviders().AddRoles<IdentityRole>();
           builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IAuthServies, AuthSerives>();
            builder.Services.AddScoped<IRoleNameRepository, RoleNameRepository>();
          
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))

                };



            });
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("MyPolicy", corsOptPolicy =>
                {
                    corsOptPolicy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            // Swagger Configuration
            builder.Services.AddSwaggerGen(swagger =>
            {
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                    }
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplictionUsers>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var hrDbContext = scope.ServiceProvider.GetRequiredService<HRDbcontext>();

                // Call SeedAsync method with both parameters
                await DefaultRole.SeedAsync(roleManager);
                // await DefaultRole.SeedAsync(roleManager, hrDbContext);
                await UserSeed.SeedBasicUserAsync(userManager, roleManager);
                
     
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCors("MyPolicy");

            app.MapControllers();

            app.Run(); 

        }
    }
}
