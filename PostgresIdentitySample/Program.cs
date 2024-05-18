using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostgresIdentitySample.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Pgsql Identity Db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Identity"));
});
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


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

app.MapIdentityApi<IdentityUser>();

// Auto migrate
using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Checking for migrations.");
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (db != null && db.Database.GetPendingMigrations().Any())
    {
        Console.WriteLine("Applying migrations.");
        db.Database.Migrate();
    }
}

app.Run();
