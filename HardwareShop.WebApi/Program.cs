using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
namespace Hardware.Web;
public class Result
{
    public Result(string name) {
        Name = name;
    }
    public String Name { get; set; }
}
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
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

        app.MapGet("/", ([FromQuery]String? index) =>
        {
            return Results.Json(new Result(index ?? "Hello"));
        }).WithTags("Home").WithDisplayName("Index");

        // app.UseHttpsRedirection();



        app.Run();
    }
}

