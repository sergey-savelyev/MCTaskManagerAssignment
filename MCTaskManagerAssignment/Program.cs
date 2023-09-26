using MCTaskManagerAssignment.Models;
using MCTaskManagerAssignment.Repositories;
using MCTaskManagerAssignment.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddDbContext<TodoListContext>(options => 
{
    options.UseLazyLoadingProxies().UseMySQL(builder.Configuration["ConnectionStrings:DefaultConnection"] ?? throw new Exception("Failed to initialize: connection string cannot be null"));
});

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoListContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
