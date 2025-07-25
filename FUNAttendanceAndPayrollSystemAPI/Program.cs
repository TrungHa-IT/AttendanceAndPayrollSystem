
using BusinessObject.Models;
using DataTransferObject.EmailDTO;
using FUNAttendanceAndPayrollSystemAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.DateTimeSchedule;
using Repository.EmployeeRepository;
using System.Text;

namespace FUNAttendanceAndPayrollSystemAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ITimekeepingRepository, TimekeepingRepository>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVCClient",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7145/")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Services.AddDbContext<FunattendanceAndPayrollSystemContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Mycnn")));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                };
            });
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<PhotoService>();
            
            builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<EmailService>();

            builder.Services.AddAuthorization();


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
