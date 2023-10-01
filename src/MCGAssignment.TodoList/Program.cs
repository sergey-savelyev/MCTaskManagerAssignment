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
builder.Services.AddScoped<ITaskActionLogService, TaskActionLogService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    options.UseLazyLoadingProxies().UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Failed to initialize: connection string cannot be null"));
});

var app = builder.Build();

// In normal life it's not the best practice: initializing the schema on startup. 
// I'd rather do it via migrations and use special endpoint called by Azure Functions (or aws alternatives).
using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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
