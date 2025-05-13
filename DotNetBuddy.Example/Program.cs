using DotNetBuddy.Example;
using DotNetBuddy.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBuddy<DatabaseContext>(); // Buddy: Default setup

builder.Services.AddControllers();

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

// Expose Program class for integration tests. This pattern allows test projects to reference
// the Program class while preventing direct instantiation.
namespace DotNetBuddy.Example
{
    public abstract class Program;
}