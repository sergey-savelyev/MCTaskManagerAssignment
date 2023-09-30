using MCGAssignment.TodoList.Repositories;
using MCGAssignment.TodoList.Services;
using Microsoft.EntityFrameworkCore;

const string AllowCorsPolicyName = "_allowCorsPolicy";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowCorsPolicyName, policy  =>
    {
        policy
            .WithOrigins("http://localhost:3939")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskActionLogger, TaskActionLogger>();
builder.Services.AddScoped<ITaskActionLogService, TaskActionLogService>();

builder.Services.AddDbContext<TodoListContext>(options => 
{
    options.UseLazyLoadingProxies().UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Failed to initialize: connection string cannot be null"));
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

app.UseCors(AllowCorsPolicyName);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
