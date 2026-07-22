using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using TelegramClientServer.Interfaces;
using TelegramClientServer.Services;
using TelegramClientServer.SignalRHubs;

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

            builder.Services.AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHostedService<ScheduledMessageService>();//To Check Schedule Messages
            builder.Services.AddSingleton<IUserIdProvider, HeaderUserIdProvider>();//Basic for signalR usage
            builder.Services.AddSingleton<IHashPassword, PasswordHasherService>();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            builder.Services.AddScoped<IFController, ClientPropsService>();

            builder.Services.AddHostedService<MessageDispatcher>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
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
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                });

            //builder.Services.AddHttpLogging(options => { });

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
            //app.UseMiddleware<ExceptionHandlingMiddleware>();              
            //app.UseHttpLogging();

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


            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.MapHub<SignalRHubs.MainHub>("/chatHub");

            //minimal API Test
            //app.MapGet("/MinAPI", () => "Min api Test");

            app.Run();
        }
    }
}
