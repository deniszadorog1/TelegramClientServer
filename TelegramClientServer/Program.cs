using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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


            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, HeaderUserIdProvider>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<SignalRHubs.MainHub>("/chatHub");

            app.Run();
        }
    }
}
