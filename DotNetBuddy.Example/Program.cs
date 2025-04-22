using DotNetBuddy.Example;
using DotNetBuddy.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuddy<DatabaseContext>(builder.Configuration); // Buddy: Default setup

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

app.UseBuddyExceptions(); // Buddy: handle exceptions

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();