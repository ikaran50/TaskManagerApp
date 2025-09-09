using Microsoft.EntityFrameworkCore;
using TaskManagement.Server.Data;


var builder = WebApplication.CreateBuilder(args);


// Services
var connectionString = builder.Configuration.GetConnectionString("Default")
?? "Data Source=taskmanager.db";


builder.Services.AddDbContext<AppDbContext>(opt =>
opt.UseSqlite(connectionString));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// CORS for Vite dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    policy.WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod());
});


var app = builder.Build();


// Apply migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}


app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.MapControllers();


app.Run();


