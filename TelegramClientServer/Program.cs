using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TelegramClientServer.SignalRHubs;
using TelegramLib.Services;
using TelegramClientServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using TelegramClientServer.Interfaces;
using Microsoft.Extensions.FileProviders;
using TelegramClientServer.Middlewares;

namespace TelegramClientServer
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
                });

            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => { });


            builder.Services.AddSignalR();

            builder.Services.AddHostedService<ScheduledMessageService>();//To Check Schedule Messages
            builder.Services.AddSingleton<IUserIdProvider, HeaderUserIdProvider>();//Basic for signalR usage
            builder.Services.AddSingleton<IHashPassword, PasswordHasherService>();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddHostedService<MessageDispatcher>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true, // Проверка подписи
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpLogging(options => { });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
     
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT token in format: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();              

            app.UseHttpLogging();

            /*            app.Use(async (context, next) =>
                        {
                            context.Request.Headers.Append("Test", "Some test value");
                            await next();
                        });

                        app.Use(async (context, next) =>
                        {
                            var header = context.Request.Headers["Test"];
                            Console.WriteLine(header);

                            await next();

                            await context.Response.WriteAsync(header.ToString());
                        });*/


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseStaticFiles();


            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();


            app.MapHub<SignalRHubs.MainHub>("/chatHub");

            //minimal API Test
            app.MapGet("/MinAPI", () => "Min api Test");
            
            app.Run();
        }
    }
}
