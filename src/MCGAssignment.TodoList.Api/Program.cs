using MCGAssignment.TodoList.Infrastructure;

const string AllowCorsPolicyName = "_allowCorsPolicy";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowCorsPolicyName, policy  =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// add infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Ensure created.
// In normal life it's not the best practice: initializing the schema on startup. 
// I'd rather do it via migrations and use special endpoint called by Azure Functions (or aws alternatives).
app.Services.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowCorsPolicyName);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
