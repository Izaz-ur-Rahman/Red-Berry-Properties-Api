using Microsoft.EntityFrameworkCore;
using RedBerryApi.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RedBerryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
using var scope = builder.Services.BuildServiceProvider().CreateScope();
var db = scope.ServiceProvider.GetRequiredService<RedBerryDbContext>();
try
{
    db.Database.CanConnect();
    Console.WriteLine("DB Connection Successful!");
}
catch (Exception ex)
{
    Console.WriteLine("DB Connection Failed: " + ex.Message);
}

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

app.Run();
